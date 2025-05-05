using Buildings;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Character;
using Scriptables;
using SavingAndLoading;
using Utils.Pooling;
using SavingAndLoading.SavableObjects;
using SavingAndLoading.Structs;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
	/// <summary>
	/// Manages all of the buildings in the game.
	/// </summary>
	[System.Serializable]
	public class BuildingManager : MonoBehaviour
	{
		// Make these 3 dictionaries into a class?
		public Dictionary<BuildingType, int> BuildingCostModifiers = new Dictionary<BuildingType, int>();
		public int GlobalBuildCostModifier;
		public Dictionary<BuildingType, int> BuildingsMaxLevel = new Dictionary<BuildingType, int>();
		public Dictionary<BuildingType, Age> BuildingAges = new Dictionary<BuildingType, Age>();

		private static Dictionary<BuildingType, BuildingDataScriptable> _buildingDataDictionary;

		[SerializeField]
		private AllBuildingDataScriptable _allBuildingData;

		private Dictionary<Player, BuildingPlacer> _placers = new Dictionary<Player, BuildingPlacer>();

		private Dictionary<BuildingType, List<BuildingBase>> _buildings = new Dictionary<BuildingType, List<BuildingBase>>();

		private int _numOfBuildings = 1;

		private Dictionary<BuildingType, int> _buildingCounts = new Dictionary<BuildingType, int>();

		private Dictionary<BuildingType, bool> _buildingsUnlocked = new Dictionary<BuildingType, bool>();

		public int NumberOfBuildings => _numOfBuildings;
		public Dictionary<BuildingType, int> BuildingCounts => _buildingCounts;
		public Dictionary<BuildingType, bool> BuildingsUnlocked => _buildingsUnlocked;

		public void Initialize()
		{
			InitializeBuildingData();
		}

		/// <summary>
		/// Returns the building data for a given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static BuildingDataScriptable GetBuildingData(BuildingType type)
		{
			if (!_buildingDataDictionary.ContainsKey(type))
				Debug.LogError($"Dictionary Doesnt Contain Data for {type}");

			return _buildingDataDictionary[type];
		}

		public bool GetPlacerBuildingType(Player player, out BuildingType type)
		{
			type = BuildingType.Count;

			if (!_placers.ContainsKey(player))
				return false;

			type = _placers[player].GetBuildingType();
			return true;
		}

		/// <summary>
		/// Returns true if the building can be afforded.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool CanAffordToBuild(BuildingType type)
		{
			if (!GameManager.Instance.BuildingsCostResources)
				return true;

			BuildingDataScriptable data = _buildingDataDictionary[type];
			int woodCost = data.BuildResourceCost.WoodCost + (int)((float)(data.BuildResourceCost.WoodCost * BuildingCounts[type]) * data.CostIncreasePerBuildingMultiplier);
			int oreCost = data.BuildResourceCost.OreCost + (int)((float)(data.BuildResourceCost.OreCost * BuildingCounts[type]) * data.CostIncreasePerBuildingMultiplier);
			int foodCost = data.BuildResourceCost.FoodCost + (int)((float)(data.BuildResourceCost.FoodCost * BuildingCounts[type]) * data.CostIncreasePerBuildingMultiplier);
			int goldCost = data.BuildResourceCost.GoldCost + (int)((float)(data.BuildResourceCost.GoldCost * BuildingCounts[type]) * data.CostIncreasePerBuildingMultiplier);
			if (TownResourceManager.MoreThanEqualComparison(Resource.Wood, woodCost - CalculateCostReduction(type, woodCost))
				&& TownResourceManager.MoreThanEqualComparison(Resource.Ore, oreCost - CalculateCostReduction(type, oreCost))
				&& TownResourceManager.MoreThanEqualComparison(Resource.Food, foodCost - CalculateCostReduction(type, foodCost))
				&& TownResourceManager.MoreThanEqualComparison(Resource.Gold, goldCost - CalculateCostReduction(type, goldCost)))
				return true;
			else
				return false;
		}

		public int CalculateCostReduction(BuildingType type, int baseValue)
		{
			return (int)(BuildingCostModifiers[type] * (baseValue / 100.0f));
		}

		/// <summary>
		/// Called when a new building has been built.
		/// </summary>
		/// <param name="building"></param>
		public void OnBuiltNewBuilding(BuildingBase building)
		{
			BuildingDataScriptable data = _buildingDataDictionary[building.BuildingType];

			if (GameManager.Instance.BuildingsCostResources)
			{
				int woodCost = data.BuildResourceCost.WoodCost + (int)((float)(data.BuildResourceCost.WoodCost * BuildingCounts[building.BuildingType]) * data.CostIncreasePerBuildingMultiplier);
				int oreCost = data.BuildResourceCost.OreCost + (int)((float)(data.BuildResourceCost.OreCost * BuildingCounts[building.BuildingType]) * data.CostIncreasePerBuildingMultiplier);
				int foodCost = data.BuildResourceCost.FoodCost + (int)((float)(data.BuildResourceCost.FoodCost * BuildingCounts[building.BuildingType]) * data.CostIncreasePerBuildingMultiplier);
				int goldCost = data.BuildResourceCost.GoldCost + (int)((float)(data.BuildResourceCost.GoldCost * BuildingCounts[building.BuildingType]) * data.CostIncreasePerBuildingMultiplier);
				TownResourceManager.RemoveResource(Resource.Wood, woodCost - CalculateCostReduction(building.BuildingType, woodCost), true);
				TownResourceManager.RemoveResource(Resource.Ore, oreCost - CalculateCostReduction(building.BuildingType, oreCost), true);
				TownResourceManager.RemoveResource(Resource.Food, foodCost - CalculateCostReduction(building.BuildingType, foodCost), true);
				TownResourceManager.RemoveResource(Resource.Gold, goldCost - CalculateCostReduction(building.BuildingType, goldCost), true);
			}

			if (!_buildings.ContainsKey(building.BuildingType))
			{
				_buildings.Add(building.BuildingType, new List<BuildingBase>());
				_buildings[building.BuildingType].Add(building);
			}
			else if (!_buildings[building.BuildingType].Contains(building))
				_buildings[building.BuildingType].Add(building);

			_numOfBuildings++;
			_buildingCounts[building.BuildingType]++;
		}

		public void AddLoadedBuilding(BuildingBase building)
		{
			if (!_buildings.ContainsKey(building.BuildingType))
			{
				_buildings.Add(building.BuildingType, new List<BuildingBase>());
				_buildings[building.BuildingType].Add(building);
			}
			else if (!_buildings[building.BuildingType].Contains(building))
				_buildings[building.BuildingType].Add(building);

			_numOfBuildings++;
			_buildingCounts[building.BuildingType]++;
		}

		/// <summary>
		/// Called when a building has been removed.
		/// </summary>
		/// <param name="building"></param>
		public void OnBuildingRemoved(BuildingBase building)
		{
			if (_buildings[building.BuildingType].Contains(building))
				_buildings[building.BuildingType].Remove(building);

			_numOfBuildings--;
			_buildingCounts[building.BuildingType]--;
		}

		/// <summary>
		/// Returns true if the building can be leveled up, using the type of building and it's current level
		/// </summary>
		/// <param name="type"></param>
		/// <param name="currentLevel"></param>
		/// <returns></returns>
		public bool CanAffordToLevel(BuildingType type, int currentLevel)
		{
			if (!GameManager.Instance.BuildingsCostResources)
				return true;

			BuildingDataScriptable data = _buildingDataDictionary[type];

			int woodCost = (int)(data.LevelResourceCost.WoodCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			woodCost -= CalculateCostReduction(type, woodCost);
			int oreCost = (int)(data.LevelResourceCost.OreCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			oreCost -= CalculateCostReduction(type, woodCost);
			int goldCost = (int)(data.LevelResourceCost.GoldCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			goldCost -= CalculateCostReduction(type, woodCost);
			int foodCost = (int)(data.LevelResourceCost.FoodCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			foodCost -= CalculateCostReduction(type, woodCost);


			// Store this calculation in a better way rather.
			if (TownResourceManager.MoreThanEqualComparison(Resource.Wood, woodCost)
				&& TownResourceManager.MoreThanEqualComparison(Resource.Ore, oreCost)
				&& TownResourceManager.MoreThanEqualComparison(Resource.Gold, goldCost)
				&& TownResourceManager.MoreThanEqualComparison(Resource.Food, foodCost))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Called when a building is leveled.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="currentLevel"></param>
		public void OnLevelBuilding(BuildingType type, int currentLevel)
		{
			if (!GameManager.Instance.BuildingsCostResources)
				return;

			BuildingDataScriptable data = _buildingDataDictionary[type];
			int woodCost = (int)(data.LevelResourceCost.WoodCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			woodCost -= CalculateCostReduction(type, woodCost);
			int oreCost = (int)(data.LevelResourceCost.OreCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			oreCost -= CalculateCostReduction(type, woodCost);
			int goldCost = (int)(data.LevelResourceCost.GoldCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			goldCost -= CalculateCostReduction(type, woodCost);
			int foodCost = (int)(data.LevelResourceCost.FoodCost * currentLevel * currentLevel * data.CostIncreasePerLevelMultiplier);
			foodCost -= CalculateCostReduction(type, woodCost);

			TownResourceManager.RemoveResource(Resource.Wood, woodCost);
			TownResourceManager.RemoveResource(Resource.Ore, oreCost);
			TownResourceManager.RemoveResource(Resource.Gold, goldCost);
			TownResourceManager.RemoveResource(Resource.Food, foodCost);
		}

		/// <summary>
		/// returns true if the type of building exists in the data dictionary.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool CheckBuildingTypeExists(BuildingType type)
		{
			return _buildingDataDictionary.ContainsKey(type);
		}

		/// <summary>
		/// Checks if the building type is placeable by the player.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool CheckBuildingIsPlaceable(BuildingType type)
		{
			//TODO:: Add check for build permissions as well as building requirements
			if (!CheckBuildingTypeExists(type))
				return false;
			else
				return _buildingDataDictionary[type].Placeable;
		}

		/// <summary>
		/// Attempts to start a new building placer. <br/>
		/// Returns <b>false</b> if player is already building, or building type doesn't exist / isn't placeable
		/// </summary>
		/// <param name="player"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool TryStartNewBuildingPlacer(Player player, BuildingType type, out string errorMessage)
		{
			// Check if player is already placing a building.
			if (_placers.ContainsKey(player))
			{
				errorMessage = "Already Placing Building";
				return false;
			}

			if (!_buildingsUnlocked[type] && !GameManager.Instance.IgnoreTechUnlocks)
			{
				errorMessage = "Building Not Unlocked Yet!";
				return false;
			}

			// Check if the building type is valid.
			if (!CheckBuildingIsPlaceable(type))
			{
				errorMessage = "Invalid Building Type";
				return false;
			}

			// Check if the building can be afforded.
			if (!CanAffordToBuild(type) && GameManager.Instance.BuildingsCostResources)
			{
				errorMessage = "Can't Afford to Build";
				return false;
			}

			// Get Building Placer from pool and set it's position to the player's last succesful building position;
			BuildingPlacer obj = GameManager.Instance.PoolingManager.GetPooledObject("BuildingPlacer").GetComponent<BuildingPlacer>();
			obj.OnPooled(player);
			obj.transform.position = player.LastBuildingPlacement;
			obj.RotatePlacer(amount: player.TotalBuildingRotation);
			// Add players placer to the list.
			_placers.Add(player, obj);

			// Set up building placer to use proper building model
			obj.gameObject.SetActive(true);
			obj.SetBuildingByType(type);
			obj.UpdateCollision();

			errorMessage = "";
			return true;
		}

		/// <summary>
		/// Attempts to move a player's active building placer and returns whether it succeeded.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="moveInput"></param>
		/// <returns></returns>
		public bool TryMoveBuilding(Player player, Vector3 moveInput)
		{
			if (!_placers.ContainsKey(player))
				return false;

			_placers[player].MovePlacer(moveInput);
			return true;
		}

		/// <summary>
		/// Attempts to rotate a player's active building placer and returns whether it succeeded.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="rotationAmount"></param>
		/// <returns></returns>
		public bool TryRotateBuilding(Player player, int rotationAmount)
		{
			if (!_placers.ContainsKey(player))
				return false;

			_placers[player].RotatePlacer(amount: rotationAmount);
			player.TotalBuildingRotation += rotationAmount;
			return true;
		}

		public void UpdatePlacerCollision(Player player)
		{
			if (!_placers.ContainsKey(player))
				return;

			_placers[player].UpdateCollision();
		}

		/// <summary>
		/// Attempts to place a building and returns whether it succeeded, passing out an error message to inform the user why it failed.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool TryPlaceBuilding(Player player, out string errorMessage)
		{
			// Check that the player is in build mode and has a building placer down.
			if (!_placers.ContainsKey(player))
			{
				errorMessage = " Not In Build Mode!";
				return false;
			}

			// If the player can place the building, remove their placer and clear the error message.
			if (_placers[player].TrySpawnBuilding(out Vector3 placementPos, out errorMessage))
			{
				_placers.Remove(player);
				player.LastBuildingPlacement = placementPos;
				errorMessage = "";
				return true;
			}

			return false;

		}

		/// <summary>
		/// Attempts to cancel a building placement and returns whether it succeeded.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool TryCancelBuilding(Player player)
		{
			// If the player does not have a building placer active, then there is nothing to cancel.
			if (player != null)
			{
				if (!_placers.ContainsKey(player))
					return false;

				_placers[player].gameObject.SetActive(false);
				_placers.Remove(player);
				return true;
			}

			return false;
		}

		public void TryCancelBuilding(object obj)
		{
			TryCancelBuilding((Player)obj);
		}

		public bool IsBuildingUnlocked(BuildingType buildingType)
		{
			return _buildingsUnlocked[buildingType];
		}

		public bool TryLevelBuilding(BuildingBase building, out string errorMessage)
		{

			// Check if the building can be leveled up.
			if (!building.BuildingData.CanLevel || building.LevelHandler == null || building.BuildingState == BuildingState.Construction)
			{
				errorMessage = "Building Can't Be Leveld Up";
				return false;
			}

			// Check the building is not at max level.
			if (!building.LevelHandler.CanLevel(true))
			{
				errorMessage = "Building Already at Max Level";
				return false;
			}

			// Check that the town can afford to level the building.
			if (!CanAffordToLevel(building.BuildingType, building.LevelHandler.Level))
			{
				errorMessage = "Can't Afford To Level";
				return false;
			}

			// Finally, attempt to level the building.
			if (building.LevelHandler.TryLevel())
			{
				errorMessage = "";
				OnLevelBuilding(building.BuildingType, building.LevelHandler.Level - 1);
				return true;
			}

			errorMessage = "Unknown Error";
			return false;
		}

		public bool CanLevelBuilding(BuildingBase building)
		{
			// Check if the building can be leveled up.
			if (!building.BuildingData.CanLevel || building.LevelHandler == null || building.BuildingState == BuildingState.Construction)
				return false;


			// Check the building is not at max level.
			if (!building.LevelHandler.CanLevel(true))
				return false;

			// Check that the town can afford to level the building.
			if (!CanAffordToLevel(building.BuildingType, building.LevelHandler.Level))
				return false;


			// Finally, attempt to level the building.
			if (building.LevelHandler.TryLevel())
				return true;

			return false;
		}

		/// <summary>
		/// Attempts to level the building and returns whether it succeeded, passing out an error message to inform the user why it failed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool TryLevelBuilding(BuildingType type, int index, out string errorMessage)
		{
			// Check that this type of building exists.
			if (!_buildings.ContainsKey(type))
			{
				errorMessage = "Building Not Found";
				return false;
			}

			// Check that they are within the bounds of the building array.
			if (_buildings[type].Count <= index || index < 0)
			{
				errorMessage = "Building Not Found";
				return false;
			}

			BuildingBase building = _buildings[type][index];

			return TryLevelBuilding(building, out errorMessage);

		}

		/// <summary>
		/// Attempts to remove a building from the game.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool TryRemoveBuilding(BuildingType type, int index, out string errorMessage)
		{
			// Check that the type of building exists already.
			if (!_buildings.ContainsKey(type))
			{
				errorMessage = "Building Does Not Exist";
				return false;
			}

			// Check that the index is within the bounds of the array.
			if (_buildings[type].Count <= index || index < 0)
			{
				errorMessage = "Building Does Not Exist";
				return false;
			}

			// Remove the building.
			errorMessage = "";
			BuildingBase building = _buildings[type][index];
			building.RemoveBuilding();
			_buildings[type].Remove(building);
			building.RestoreFoliage(false);
			return true;
		}

		public bool TryGetBuilding(BuildingType type, int index, out BuildingBase buildingBase, out string errorMessage)
		{
			// Check that the type of building exists already.
			if (!_buildings.ContainsKey(type))
			{
				errorMessage = "Building Does Not Exist";
				buildingBase = null;
				return false;
			}

			// Check that the index is within the bounds of the array.
			if (_buildings[type].Count <= index || index < 0)
			{
				errorMessage = "Building Does Not Exist";
				buildingBase = null;
				return false;
			}

			// Remove the building.
			errorMessage = "";
			buildingBase = _buildings[type][index]; ;
			return true;
		}

		public bool TryRemoveBuilding(BuildingBase building)
		{
			building.RemoveBuilding();
			_buildings[building.BuildingType].Remove(building);
			building.RestoreFoliage(false);
			return true;
		}

		/// <summary>
		/// Displays the building ID for a given amount of time.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool DisplayBuildingIdsOfType(BuildingType type)
		{
			if (!_buildings.ContainsKey(type))
			{
				return false;
			}

			for (int i = 0; i < _buildings[type].Count; i++)
			{
				UtilDisplayManager.AddTextDisplay(_buildings[type][i].TargetableBuilding, $"{i + 1}");
			}

			return true;
		}

		/// <summary>
		/// Returns an array of all the buildings of a particular type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public BuildingBase[] GetAllBuildingsOfType(BuildingType type)
		{
			return _buildings[type].ToArray();
		}

		/// <summary>
		/// Returns the dictionary of all the buildings.
		/// </summary>
		/// <returns></returns>
		public Dictionary<BuildingType, List<BuildingBase>> GetAllBuildingsDictionary()
		{
			return _buildings;
		}

		public List<BuildingBase> GetBuildingsByType(BuildingType type)
		{
			return _buildings[type];
		}

		public void UnlockBuilding(BuildingType type)
		{
			_buildingsUnlocked[type] = true;
		}

		public int GetBuildCostModifier(BuildingType type) => BuildingCostModifiers[type];


		private void InitializeBuildingData()
		{
			_buildingDataDictionary = new Dictionary<BuildingType, BuildingDataScriptable>();
			_buildingsUnlocked = new Dictionary<BuildingType, bool>();
			BuildingsMaxLevel = new Dictionary<BuildingType, int>();
			BuildingCostModifiers = new Dictionary<BuildingType, int>();
			_buildingCounts = new Dictionary<BuildingType, int>();

			for (int i = 0; i < _allBuildingData.BuildingData.Length; i++)
			{
				BuildingType buildingType = (BuildingType)i;
				_buildingDataDictionary.Add(buildingType, _allBuildingData.GetDataByBuildingType(buildingType));
				_buildingsUnlocked.Add(buildingType, _buildingDataDictionary[buildingType].Unlocked);
				BuildingsMaxLevel.Add(buildingType, 1);
				BuildingCostModifiers.Add(buildingType, 0);
				BuildingAges.Add(buildingType, _buildingDataDictionary[buildingType].StartingAge);
				_buildingCounts.Add(buildingType, 0);
			}

			GlobalBuildCostModifier = 0;
		}

		public void ResetBuilding(BuildingType type)
		{
			List<BuildingSaveData> buildings = new List<BuildingSaveData>();

			List<PoolableObject> objs = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(type.ToString());
			if (objs != null)
				for (int o = 0; o < objs.Count; o++)
					buildings.Add((BuildingSaveData)((SavingAndLoading.SavableObjects.SaveableBuilding)objs[o].SaveableObject).SaveData());

			_buildings[type].Clear();
			GameManager.Instance.PoolingManager.DisableObjectsInPool(type.ToString());

			for(int i = 0; i < buildings.Count; i++)
			{
				var building = GameManager.Instance.PoolingManager.GetPooledObject(buildings[i].BuildingType, false);
				((SaveableBuilding)((building).SaveableObject)).LoadData((object)buildings[i]);
			}
		}
	}
}