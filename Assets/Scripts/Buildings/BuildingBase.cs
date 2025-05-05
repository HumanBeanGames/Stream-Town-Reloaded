using Level;
using Managers;
using SavingAndLoading.SavableObjects;
using SavingAndLoading.Structs;
using Scriptables;
using System.Collections.Generic;
using Target;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UserInterface;
using Utils;
using Utils.Pooling;

namespace Buildings
{
	/// <summary>
	/// Base class for all Buildings, all buildings require this or an inherited type
	/// </summary>
	public class BuildingBase : MonoBehaviour
	{
		/// <summary>
		/// Determines if the building should be fully constructed when spawned.
		/// </summary>
		[SerializeField, Tooltip("Determines if the building should be fully constructed when spawned.")]
		protected bool _spawnBuilt = false;

		/// <summary>
		/// Enum dictating what type of building it is.
		/// </summary>
		[SerializeField]
		protected BuildingType _buildingType;

		/// <summary>
		/// Determines what target flags the building will have once it has finished construction.
		/// </summary>
		[SerializeField, Tooltip("Determines what target flags the building will have once it has finished construction.")]
		protected TargetMask _finishedTargetType = TargetMask.Building;

		/// <summary>
		/// Keeps track of current Building State.
		/// </summary>
		protected BuildingState _buildingState = BuildingState.Construction;

		/// <summary>
		/// What model index to display.
		/// </summary>
		protected int _modelHandlerIndex = 0;

		/// <summary>
		/// Determines if this building has a station component or not
		/// </summary>
		protected bool _hasStation = false;

		/// <summary>
		/// Keeps track of what stage the building is at
		/// </summary>
		protected int _currentBuildStage = 0;

		/// <summary>
		/// Determines if the Building Information has been Initialized
		/// </summary>
		protected bool _initialized = false; //TODO:: May not be requried anymore.

		/// <summary>
		/// Determines the percentage of health for construction to be finished with the first stage.
		/// </summary>
		protected const float HEALTH_PERCENT_FIRST_STAGE = 0.33f;
		/// <summary>
		/// Determines the percentage of health for the construction to be finished with the second stage.
		/// </summary>
		protected const float HEALTH_PERCENT_SECOND_STAGE = 0.66f;

		protected Age _age;

		// Events to subscribe to in inspector.
		[SerializeField]
		protected UnityEvent _onBuiltEvent;

		// References to Required Components.
		[SerializeField]
		protected BuildingModelHandler[] _modelHandler;
		protected HealthHandler _healthHandler;
		protected TargetableBuilding _targetable;
		protected Station _station;
		protected UpdateGraphBounds _graphUpdater;
		protected BuildingLevelHandler _levelHandler;
		protected BuildingDataScriptable _buildingData;
		protected UnitTextDisplay _displayText;
		protected BuildingDamageMaterialHandler _damageHandler;

		public List<PoolableObject> FoliageRemoved { get; set; }

		// Properties
		public TargetMask FinishedTargetType => _finishedTargetType;
		public BuildingType BuildingType => _buildingType;
		public BuildingLevelHandler LevelHandler => _levelHandler;
		public bool Built => _buildingState == BuildingState.Building;
		public UnitTextDisplay DisplayText => _displayText;
		public TargetableBuilding TargetableBuilding => _targetable;
		public HealthHandler HealthHandler => _healthHandler;
		public BuildingDamageMaterialHandler DamageHandler
		{
			get => _damageHandler;
			set => _damageHandler = value;
		}

		public Station Station => _station;

		public BuildingDataScriptable BuildingData
		{
			get => _buildingData;
			set => _buildingData = value;
		}

		public BuildingState BuildingState
		{
			get => _buildingState;
			set => _buildingState = value;
		}

		public void RestoreFoliage(bool data)
		{
			if (FoliageRemoved != null)
			{
				for (int i = 0; i < FoliageRemoved.Count; i++)
				{
					FoliageRemoved[i].gameObject.SetActive(true);
					//FoliageRemoved[i].transform.position = new Vector3(FoliageRemoved[i].transform.position.x, 100, FoliageRemoved[i].transform.position.x);
				}
				FoliageRemoved.Clear();
			}
		}

		public List<FoliageSaveData> GetRemovedFoliageData()
		{
			List<FoliageSaveData> data = new List<FoliageSaveData>();
			for (int i = 0; i < FoliageRemoved.Count; i++)
			{
				data.Add((FoliageSaveData)((SaveablFoliage)FoliageRemoved[i].SaveableObject).SaveData());
			}
			return data;
		}

		public void SetRemovedFoliage(List<FoliageSaveData> data)
		{
			List<PoolableObject> removedFoliage = new List<PoolableObject>();
			for (int i = 0; i < data.Count; i++)
			{
				PoolableObject obj = ObjectPoolingManager.GetPooledObject(data[i].FoliageType, false);
				((SaveablFoliage)(obj.SaveableObject)).LoadData((object)data[i]);
				removedFoliage.Add(obj);
				obj.gameObject.SetActive(false);
			}
			FoliageRemoved = removedFoliage;
		}

		/// <summary>
		/// Called when a building is Spawned
		/// </summary>
		public void OnSpawn()
		{
			// Loop through all building models and set them to invisible if they are not the currently active building.
			for (int i = 0; i < _modelHandler.Length; i++)
			{
				if (i == _modelHandlerIndex)
					continue;

				_modelHandler[i].gameObject.SetActive(false);
			}

			// Preemptively set Building State to Constructed
			_buildingState = BuildingState.Construction;

			// If building should be spawn as built...
			if (_spawnBuilt)
			{
				// ...Set it's health to max and call OnBuilt function.
				_healthHandler.SetHealth(_healthHandler.MaxHealth);
				OnBuilt();
			}
			else
			{
				// ...Else set building as a construction
				OnHealthConstruction(0);
				_healthHandler.SetHealth(1);

				// Set it's target type to Construction
				_targetable.SetTargetType(TargetMask.Construction);

				// Disable station if it has one
				if (_hasStation)
					_station.enabled = false;
			}

			_buildingData = BuildingManager.GetBuildingData(_buildingType);
			if (_levelHandler != null)
				_levelHandler.MaxLevel = BuildingManager.BuildingsMaxLevel[_buildingType];

			TechTreeManager.OnBuildingLevelIncreased += OnBuildingLevelIncreased;
			TechTreeManager.OnBuildingAgedUp += OnBuildingAged;
			OnBuildingAged(_buildingType);
			FoliageRemoved = new List<PoolableObject>();
		}

		/// <summary>
		/// Determines what stage the construction is based on the Health Percentage.
		/// </summary>
		/// <param name="value"></param>
		public virtual void OnHealthConstruction(float value)
		{
			// If no model handler is found, early out.
			if (_modelHandler == null)
				return;

			// If the value is 1.0f (or 100%), the construction is finished.
			if (value == 1.0f)
			{
				_currentBuildStage = 3;
				OnBuilt();
			}
			// Else if the value is above the 2nd stage requirement, we are in stage 2.
			else if (value > HEALTH_PERCENT_SECOND_STAGE)
			{
				_currentBuildStage = 2;
			}
			// Else if the value is above the 1st stage requirement, we are in stage 1.
			else if (value > HEALTH_PERCENT_FIRST_STAGE)
			{
				// Call the Graph Bounds to be updated and set.
				if (_currentBuildStage != 1 && _graphUpdater)
					_graphUpdater.SetGraphBounds();

				_currentBuildStage = 1;
			}
			// Else the construction has just begun.
			else if (value < HEALTH_PERCENT_FIRST_STAGE)
			{
				_currentBuildStage = 0;
			}

			// Update the currently visible model.
			UpdateActiveModel();
		}

		/// <summary>
		/// Removes a building from the game and returns it to the pool.
		/// </summary>
		public virtual void RemoveBuilding()
		{
			_healthHandler.SetHealth(0);
		}

		/// <summary>
		/// Called when a construction has finished being built.
		/// </summary>
		public virtual void OnBuilt()
		{
			// Inform the model handler that construction has finished.
			_modelHandler[_modelHandlerIndex].OnFinishedConstruction();

			// Set building's target type appropriately.
			_targetable.SetTargetType(_finishedTargetType);

			// Enable station if we have one.
			if (_hasStation)
				_station.enabled = true;

			// Set building state correctly and invoke the On Build Event.
			_buildingState = BuildingState.Building;
			_onBuiltEvent.Invoke();
			EventManager.BuildingBuilt?.Invoke(_buildingType);
		}

		public void OnLoadedBuiltBuilding()
		{
			_currentBuildStage = 3;

			// Inform the model handler that construction has finished.
			_modelHandler[_modelHandlerIndex].OnFinishedConstruction();

			// Enable station if we have one.
			if (_hasStation)
				_station.enabled = true;
		}

		/// <summary>
		/// Sets the index of which model to use and activates it.
		/// </summary>
		/// <param name="index"></param>
		public virtual void SetModelHandlerIndex(int index)
		{
			if (_buildingType == BuildingType.Wall)
				index += (4 * (int)BuildingManager.BuildingAges[_buildingType]);
			if (index >= _modelHandler.Length)
				index = _modelHandler.Length - 1;

			_modelHandler[_modelHandlerIndex].gameObject.SetActive(false);
			_modelHandler[index].gameObject.SetActive(true);
			_modelHandlerIndex = index;
			UpdateActiveModel();
		}


		/// <summary>
		/// Called when a building has been destroyed.
		/// </summary>
		public void OnBuildingDestroyed()
		{
			BuildingManager.OnBuildingRemoved(this);
		}

		/// <summary>
		/// Updates which model is currently visible based on the current build stage.
		/// </summary>
		protected virtual void UpdateActiveModel()
		{
			if (_currentBuildStage == 0 && _buildingState == BuildingState.Construction)
			{
				_modelHandler[_modelHandlerIndex].OnConstructionStart();
			}
			else if (_currentBuildStage == 1 && _buildingState == BuildingState.Construction)
			{
				_modelHandler[_modelHandlerIndex].OnStage2();
			}
			else if (_currentBuildStage == 2 && _buildingState == BuildingState.Construction)
			{
				_modelHandler[_modelHandlerIndex].OnStage3();
			}
			else
			{
				_modelHandler[_modelHandlerIndex].OnFinishedConstruction();
			}
		}

		/// <summary>
		/// Initializes all required components
		/// </summary>
		protected virtual void Init()
		{
			_initialized = true;
			if (_modelHandler.Length == 0)
				_modelHandler = GetComponentsInChildren<BuildingModelHandler>();
			_graphUpdater = GetComponent<UpdateGraphBounds>();
			_displayText = GetComponentInChildren<UnitTextDisplay>();
			TryGetComponent(out _levelHandler);

			_healthHandler = GetComponent<HealthHandler>();
			_targetable = GetComponent<TargetableBuilding>();
			_hasStation = TryGetComponent(out _station);
		}

		private void OnBuildingLevelIncreased(BuildingType type)
		{
			if (type != _buildingType)
				return;

			_levelHandler.MaxLevel = BuildingManager.BuildingsMaxLevel[_buildingType];
		}

		private void OnBuildingAged(BuildingType type)
		{
			if (type != _buildingType)
				return;

			_age = BuildingManager.BuildingAges[_buildingType];

			if (_buildingType == BuildingType.Wall)
				return;

			SetModelHandlerIndex((int)_age);
			//Age Up Building Models.
		}

		private void Awake()
		{
			Init();
		}

		private void OnEnable()
		{
			if (!_initialized)
				Init();
			OnSpawn();
			_healthHandler.OnDeath += RestoreFoliage;
		}

		private void OnDisable()
		{
			_healthHandler.OnDeath -= RestoreFoliage;
		}
	}
}