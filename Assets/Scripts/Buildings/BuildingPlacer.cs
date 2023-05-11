using Character;
using Managers;
using Pathfinding;
using SavingAndLoading.SavableObjects;
using SavingAndLoading.Structs;
using System.Collections.Generic;
using UnityEngine;
using UserInterface;
using Utils;
using Utils.Pooling;

namespace Buildings
{
	/// <summary>
	/// Used for placing new buildings in the world.
	/// </summary>
	public class BuildingPlacer : MonoBehaviour
	{
		/// <summary>
		/// Used as a collision mask against any buildings or obstacles.
		/// </summary>
		[SerializeField]
		private LayerMask _collisionMask;

		/// <summary>
		/// Holds all build data for each buildable building type.
		/// </summary>
		public List<BuildPlacerData> _buildData;

		/// <summary>
		/// Current Building Index.
		/// </summary>
		private int _currentIndex = 0;

		/// <summary>
		/// True if placer is colliding with any buildings or obstacles.
		/// </summary>
		private bool _colliding = false;

		// Colors displayed on building meshes to determine if there is a collision
		[SerializeField]
		private Color _successColor;
		[SerializeField]
		private Color _failColor;

		// Required Components
		private Player _owner;
		private BuildPlacerData _currentBuilding;
		private BoxCollider _boxCollider;
		private BoundsVisualizer _boundsVisualizer;
		private UnitTextDisplay _textDisplay;
		private SimpleCancelBuildingPlacer _simpleCallTimer;
		private SnapToGridMouseMovement _snapMovement;

		/// <summary>
		/// Called when gameobject has been pooled.
		/// </summary>
		/// <param name="player"></param>
		public void OnPooled(Player player)
		{
			_owner = player;

			//TODO:: Fix this
			if (_textDisplay != null && player.TwitchUser.Username != "")
			{
				if (player.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username)
					_textDisplay.SetDisplayText("");
				else
					_textDisplay.SetDisplayText(player.TwitchUser.Username);
			}

			if (player.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username)
			{
				_snapMovement.enabled = true;
				_snapMovement.OnPositionChanged += UpdateCollision;
			}
			else
				_snapMovement.enabled = false;

			_simpleCallTimer.SetPlayer(player);
		}

		/// <summary>
		/// Sets the current Building Index to change which building is being placed. <br/>
		/// Activates the model of the building that was indexed.
		/// </summary>
		/// <param name="index"></param>
		public void SetBuildingIndex(int index)
		{
			// Limit index between list bounds.
			if (index < 0)
				index = _buildData.Count - 1;

			if (index >= _buildData.Count)
				index = 0;

			// Hide previous building model.
			_buildData[_currentIndex].BuildingModel.SetActive(false);

			// Show next building model.
			_buildData[index].BuildingModel.SetActive(true);

			// Set current index and building to the new value.
			_currentIndex = index;
			_currentBuilding = _buildData[_currentIndex];

			// Update the bounds visualizer to the new building size.
			_boundsVisualizer.SetSize(_currentBuilding.BuildingSize);

			// Check if Probe Handler is set, otherwise set and store it
			if (_currentBuilding.ProbeManager == null)
				_currentBuilding.ProbeManager = _currentBuilding.BuildingModel.GetComponentInChildren<PlacementProbeHandler>();

			// Call update on the collision.
			UpdateCollision();
		}


		/// <summary>
		/// Sets the current Building Index by the type of building
		/// </summary>
		/// <param name="type"></param>
		public void SetBuildingByType(BuildingType type)
		{
			// Get the index of the building data by type
			int index = GetBuildingIndex(type);

			// Check that the index was valid
			if (index == -1)
			{
				Debug.LogError($"Attempted to set building to one it should not have been {type}");
			}
			else
				SetBuildingIndex(index);
		}

		public BuildingType GetBuildingType()
		{
			return _currentBuilding.BuildingType;
		}

		/// <summary>
		/// Returns the building data index by the Building Type. <br/>
		/// Returns -1 if data does not exist.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int GetBuildingIndex(BuildingType type)
		{
			// Loop through all the building data and return index if available.
			for (int i = 0; i < _buildData.Count; i++)
			{
				if (_buildData[i].BuildingType == type)
					return i;
			}

			return -1;
		}

		/// <summary>
		/// Moves the building placer by the specified Move Vector.
		/// </summary>
		/// <param name="moveVector"></param>
		public void MovePlacer(Vector3 moveVector)
		{
			transform.position += moveVector;
			UpdateCollision();
		}

		/// <summary>
		/// Rotates the placer on the spot by the rotation in multiples of 90 degrees.
		/// </summary>
		/// <param name="right"></param>
		/// <param name="amount"></param>
		public void RotatePlacer(bool right = true, int amount = 1)
		{
			transform.Rotate(new Vector3(0, (right ? 90 : -90) * amount, 0));
			UpdateCollision();
		}

		/// <summary>
		/// Returns True if building can be afforded.
		/// </summary>
		/// <returns></returns>
		public bool CanAfford()
		{
			return GameManager.Instance.BuildingManager.CanAffordToBuild(_currentBuilding.BuildingType);
		}

		/// <summary>
		/// Attemps to spawn the building and returns the placer back to the pool.
		/// </summary>
		/// <param name="placementPos"></param>
		/// <param name="disableOnSpawn"></param>
		/// <returns></returns>
		public bool TrySpawnBuilding(out Vector3 placementPos, out string errorMessage, bool disableOnSpawn = true)
		{
			placementPos = Vector3.zero;

			// Check if building is colliding with an obstalce, if so, return out.
			if (_colliding)
			{
				errorMessage = "Invalid Location!";
				return false;
			}

			// A last check if the building can be afforded, if not, return out.
			//if (!CanAfford())
			//{
			//	errorMessage = "Can't Afford!";
			//	return false;
			//}

			// Get building from pooling manager and set it's position and rotation.
			PoolableObject obj = GameManager.Instance.PoolingManager.GetPooledObject(_currentBuilding.BuildingType.ToString());

			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			obj.gameObject.SetActive(true);

			// Add building to building dictionary.
			GameManager.Instance.BuildingManager.OnBuiltNewBuilding(obj.GetComponent<BuildingBase>());

			// Set player's last placement position to building's position
			placementPos = obj.transform.position;

			gameObject.SetActive(false);

			errorMessage = "";
			BoxCollider collider = obj.GetComponent<BoxCollider>();
			Vector3 center = obj.transform.position;
			center.y = 0;

			BuildingBase buildingBase = obj.GetComponent<BuildingBase>();
			buildingBase.FoliageRemoved = new List<PoolableObject>();

			// Removes foliage
			for (int i = 0; i < (int)FoliageType.Count; i++)
			{
				List<PoolableObject> foliageObjects = GameManager.Instance.PoolingManager.GetAllActiveObjectsOfTypeWithinBoxCollider(collider, center, ((FoliageType)i).ToString());
				for (int j = 0; j < foliageObjects.Count; j++)
				{
					foliageObjects[j].gameObject.SetActive(false);
					buildingBase.FoliageRemoved.Add(((SaveablFoliage)foliageObjects[j].SaveableObject).PoolableObject);
				}
			}
			return true;
		}

		/// <summary>
		/// Updates the collision of the building placer to determine if it can be placed or not.
		/// </summary>
		public void UpdateCollision()
		{
			if (_currentBuilding.ProbeManager == null)
				return;

			if (_currentBuilding == null)
				return;

			// Get the half extents of the building
			Vector3 halfExtents = Vector3.zero;
			halfExtents.x = _currentBuilding.BuildingSize.x * 0.45f;
			halfExtents.z = _currentBuilding.BuildingSize.y * 0.45f;

			// Box cast from above the building to the ground to see if it hits any obstacles or buildings.
			_colliding = (Physics.BoxCast(transform.position + Vector3.up * 10, halfExtents, -transform.up, transform.rotation, 10, _collisionMask));

			// If we aren't colliding with anything, also check that the building's probes passed their check.
				if (!_colliding && !_currentBuilding.ProbeManager.AllProbesPassedCheck())
					_colliding = true;
			// Check that the building doesn't block pathing to the world borders.
			if (!_colliding)
			{
				bool canPath = true;

				//TODO:: REIMPLEMENT BECAUSE IT SUCKS CURRENTLY - ALSO NEED TO PATH TO TOWNHALL
				//GraphNode here = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
				//// As soon as one path is valid, we can continue.
				//for (int i = 0; i < GameManager.Instance.PathProbes.Count; i++)
				//{
				//	if (GameManager.Instance.PathProbes[i].CanPathTo(here))
				//	{
				//		_colliding = false;
				//		canPath = true;

				//		break;
				//	}
				//}

				if (!canPath)
					_colliding = true;
			}
			// Set colour of the visualizer and buiding to show if there is a collision or not.
			_boundsVisualizer.OnCollisionChange(_colliding, _failColor, _successColor);
			SetBuildingRenderer(_colliding);
			_simpleCallTimer.ResetTimer();
		}

		/// <summary>
		/// Sets the material colour of the building to match whether there is a collision or not.
		/// </summary>
		/// <param name="_colliding"></param>
		private void SetBuildingRenderer(bool _colliding)
		{
			// Get all renderers
			_currentBuilding.Renderer.material.SetColor("_boundsVisColor", _colliding ? _failColor : _successColor);
		}

		/// <summary>
		/// Returns the current building's index.
		/// </summary>
		/// <returns></returns>
		public int GetBuildingIndex() => _currentIndex;

		// Unity Functions.
		public void Awake()
		{
			// Get Components
			_boxCollider = GetComponent<BoxCollider>();
			_boundsVisualizer = GetComponent<BoundsVisualizer>();
			_textDisplay = GetComponentInChildren<UnitTextDisplay>();
			_simpleCallTimer = GetComponent<SimpleCancelBuildingPlacer>();
			_snapMovement = GetComponent<SnapToGridMouseMovement>();

			for (int i = 1; i < _buildData.Count; i++)
			{
				_buildData[i].BuildingModel.SetActive(false);
			}

			_currentBuilding = _buildData[0];
		}

		private void OnDisable()
		{
			if (_owner == null)
				return;

			if (_owner.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username)
			{
				_snapMovement.enabled = false;
				_snapMovement.OnPositionChanged -= UpdateCollision;
			}
		}

		private void OnDrawGizmos()
		{
			if (_currentBuilding == null)
				return;

			if (_colliding)
				Gizmos.color = _failColor;
			else
				Gizmos.color = _successColor;

			// Draw bounding box of the building.
			Matrix4x4 prevMat = Gizmos.matrix;
			Matrix4x4 newMat = transform.localToWorldMatrix;
			Gizmos.matrix = newMat;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(_currentBuilding.BuildingSize.x, 1, _currentBuilding.BuildingSize.y));

			// Reset gizmo matrix.
			Gizmos.matrix = prevMat;
		}
	}
}