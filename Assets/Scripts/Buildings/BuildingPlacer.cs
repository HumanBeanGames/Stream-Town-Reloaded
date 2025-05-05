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
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private LayerMask _collisionMask;
        public List<BuildPlacerData> _buildData;
        private int _currentIndex = 0;
        private bool _colliding = false;

        [SerializeField] private Color _successColor;
        [SerializeField] private Color _failColor;

        private Player _owner;
        private BuildPlacerData _currentBuilding;
        private BoxCollider _boxCollider;
        private BoundsVisualizer _boundsVisualizer;
        private UnitTextDisplay _textDisplay;
        private SimpleCancelBuildingPlacer _simpleCallTimer;
        private SnapToGridMouseMovement _snapMovement;

        public void OnPooled(Player player)
        {
            _owner = player;

            if (_textDisplay != null && player.TwitchUser.Username != "")
            {
                _textDisplay.SetDisplayText(player.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username ? "" : player.TwitchUser.Username);
            }

            bool isHostPlayer = (player.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username);
            bool shouldSnapToCursor = isHostPlayer && !GameManager.Instance.IdleMode;

            _snapMovement.enabled = shouldSnapToCursor;

            if (shouldSnapToCursor)
            {
                _snapMovement.OnPositionChanged += UpdateCollision;
            }
            else
            {
                _snapMovement.OnPositionChanged -= UpdateCollision;
            }

            _simpleCallTimer.SetPlayer(player);
        }

        public void UpdateSnapToCursorState()
        {
            bool isHostPlayer = (_owner != null && _owner.TwitchUser.Username == GameManager.Instance.UserPlayer.TwitchUser.Username);
            bool shouldSnapToCursor = isHostPlayer && !GameManager.Instance.IdleMode;

            _snapMovement.enabled = shouldSnapToCursor;

            if (shouldSnapToCursor)
                _snapMovement.OnPositionChanged += UpdateCollision;
            else
                _snapMovement.OnPositionChanged -= UpdateCollision;
        }

        public void SetBuildingIndex(int index)
        {
            if (index < 0) index = _buildData.Count - 1;
            if (index >= _buildData.Count) index = 0;

            _buildData[_currentIndex].BuildingModel.SetActive(false);
            _buildData[index].BuildingModel.SetActive(true);

            _currentIndex = index;
            _currentBuilding = _buildData[_currentIndex];
            _boundsVisualizer.SetSize(_currentBuilding.BuildingSize);

            if (_currentBuilding.ProbeManager == null)
                _currentBuilding.ProbeManager = _currentBuilding.BuildingModel.GetComponentInChildren<PlacementProbeHandler>();

            UpdateCollision();
        }

        public void SetBuildingByType(BuildingType type)
        {
            int index = GetBuildingIndex(type);
            if (index == -1)
            {
                Debug.LogError($"Attempted to set building to one it should not have been {type}");
            }
            else
            {
                SetBuildingIndex(index);
            }
        }

        public BuildingType GetBuildingType() => _currentBuilding.BuildingType;

        public int GetBuildingIndex(BuildingType type)
        {
            for (int i = 0; i < _buildData.Count; i++)
            {
                if (_buildData[i].BuildingType == type)
                    return i;
            }
            return -1;
        }

        public void MovePlacer(Vector3 moveVector)
        {
            transform.position += moveVector;
            UpdateCollision();
        }

        public void SetPlacerPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            UpdateCollision();
        }

        public void RotatePlacer(bool right = true, int amount = 1)
        {
            transform.Rotate(new Vector3(0, (right ? 90 : -90) * amount, 0));
            UpdateCollision();
        }

        public bool CanAfford() => GameManager.Instance.BuildingManager.CanAffordToBuild(_currentBuilding.BuildingType);

        public bool TrySpawnBuilding(out Vector3 placementPos, out string errorMessage, bool disableOnSpawn = true)
        {
            placementPos = Vector3.zero;
            if (_colliding)
            {
                errorMessage = "Invalid Location!";
                return false;
            }

            PoolableObject obj = ObjectPoolingManager.GetPooledObject(_currentBuilding.BuildingType.ToString());
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.gameObject.SetActive(true);

            GameManager.Instance.BuildingManager.OnBuiltNewBuilding(obj.GetComponent<BuildingBase>());
            placementPos = obj.transform.position;

            gameObject.SetActive(false);
            BoxCollider collider = obj.GetComponent<BoxCollider>();
            Vector3 center = obj.transform.position; center.y = 0;

            BuildingBase buildingBase = obj.GetComponent<BuildingBase>();
            buildingBase.FoliageRemoved = new List<PoolableObject>();

            for (int i = 0; i < (int)FoliageType.Count; i++)
            {
                List<PoolableObject> foliageObjects = ObjectPoolingManager.GetAllActiveObjectsOfTypeWithinBoxCollider(collider, center, ((FoliageType)i).ToString());
                foreach (var objFoliage in foliageObjects)
                {
                    objFoliage.gameObject.SetActive(false);
                    buildingBase.FoliageRemoved.Add(((SaveablFoliage)objFoliage.SaveableObject).PoolableObject);
                }
            }
            errorMessage = "";
            return true;
        }

        public void UpdateCollision()
        {
            if (_currentBuilding == null || _currentBuilding.ProbeManager == null)
                return;

            Vector3 halfExtents = new Vector3(_currentBuilding.BuildingSize.x * 0.45f, 1, _currentBuilding.BuildingSize.y * 0.45f);
            _colliding = Physics.BoxCast(transform.position + Vector3.up * 10, halfExtents, -Vector3.up, transform.rotation, 10, _collisionMask);

            if (!_colliding && !_currentBuilding.ProbeManager.AllProbesPassedCheck())
                _colliding = true;

            _boundsVisualizer.OnCollisionChange(_colliding, _failColor, _successColor);
            SetBuildingRenderer(_colliding);
            _simpleCallTimer.ResetTimer();
        }

        private void SetBuildingRenderer(bool colliding)
        {
            _currentBuilding.Renderer.material.SetColor("_boundsVisColor", colliding ? _failColor : _successColor);
        }

        public int GetBuildingIndex() => _currentIndex;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boundsVisualizer = GetComponent<BoundsVisualizer>();
            _textDisplay = GetComponentInChildren<UnitTextDisplay>();
            _simpleCallTimer = GetComponent<SimpleCancelBuildingPlacer>();
            _snapMovement = GetComponent<SnapToGridMouseMovement>(); // ← Add this line!

            for (int i = 1; i < _buildData.Count; i++)
                _buildData[i].BuildingModel.SetActive(false);

            _currentBuilding = _buildData[0];
        }

        private void OnDisable()
        {
            if (_owner == null) return;
        }

        private void OnDrawGizmos()
        {
            if (_currentBuilding == null) return;

            Gizmos.color = _colliding ? _failColor : _successColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_currentBuilding.BuildingSize.x, 1, _currentBuilding.BuildingSize.y));
        }
    }
}