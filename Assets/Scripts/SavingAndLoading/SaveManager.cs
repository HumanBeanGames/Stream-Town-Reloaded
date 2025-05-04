using UnityEngine;
using World.Generation;
using Managers;
using System.Collections.Generic;
using SavingAndLoading.Structs;
using Utils;
using GameResources;
using Units;
using Buildings;
using System.Collections;
using Character;
using UnityEngine.SceneManagement;
using Twitch.Utils;
using GUIDSystem;
using Sensors;
using PlayerControls;
using System;
using Pets.Enumerations;
using TechTree.ScriptableObjects;
using TownGoal.Data;
using Enemies;
using Utils.Pooling;
using SavingAndLoading.SavableObjects;
using Target;

namespace SavingAndLoading
{
	public class SaveManager : MonoBehaviour
	{
		[SerializeField]
		private ProceduralWorldGenerator _generationObject = null;
		private List<Player> _players = null;
		private List<Enemy> _enemies = null;

		private bool _autosave = false;
		private float _autosaveTime = 0.0f;
		private float _timeElapsed = 0.0f;

		private int _loadPercent = 0;
		public int LoadPercent => _loadPercent;

		public event Action<float> UpdateProgress;
		private void EscapePressed()
		{
			SceneManager.LoadScene(0);
		}

		public void SetAutosaveTime(float time)
		{
			_autosaveTime = time;
			_autosave = _autosaveTime <= 0.0f ? false : true;
		}

		private void Update()
		{
			if(_autosave)
				if(_timeElapsed >= _autosaveTime)
				{
					_timeElapsed = 0.0f;
					SaveGame();
				}
				else
				_timeElapsed += Time.deltaTime;
		}

		/// <summary>
		/// Saves the entire game
		/// </summary>
		public void SaveGame()
		{
			Debug.Log("Saving Game");
			WorldGenSaveData worldGenSave = GetWorldGenerationData();
			List<BuildingSaveData> buildings = GetBuildingsData();
			List<EnemySaveData> enemySaveData = GetEnemySaveData();
			WorldSaveData worldSaveData = GetWorldData();

			List<PlayerSaveData> playerSaveData = GetPlayerSaveData();

			playerSaveData = SetPlayerTargetGUIDs(playerSaveData); // Sets target GUIDS
			enemySaveData = SetEnemyTargetGUIDs(enemySaveData); // Sets enemys GUIDS

			SaveGameData gameSave = new SaveGameData(worldGenSave, buildings, enemySaveData, worldSaveData);
			SavePlayersData playersSave = new SavePlayersData(playerSaveData);
			GameIO.SaveGameData(gameSave);
			GameIO.SavePlayersData(playersSave);
		}

		/// <summary>
		/// Sets the players GUIDs for targets and stations
		/// </summary>
		/// <param name="data">A list of PlayerSaveData to set GUIDs for</param>
		/// <returns>An updated list of PlayerSaveData with GUIDs set</returns>
		private List<PlayerSaveData> SetPlayerTargetGUIDs(List<PlayerSaveData> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				// Sets players target
				Target.Targetable target = _players[i].TargetSensor.CurrentTarget;
				if (target != null)
					data[i].SetTargetGUID(target.GUIDComponent.GUID);
				else
					data[i].SetTargetGUID(0);

				// Sets players station
				Station station = _players[i].StationSensor.CurrentStation;
				if (target != null)
					data[i].SetStationGUID(station.GUIDComponent.GUID);
				else
					data[i].SetStationGUID(0);
			}

			return data;
		}

		/// <summary>
		/// Sets the enemies GUIDs for targets and stations
		/// </summary>
		/// <param name="data">A list of EnemySaveData to set GUIDs for</param>
		/// <returns>An updated list of EnemySaveData with GUIDs set</returns>
		private List<EnemySaveData> SetEnemyTargetGUIDs(List<EnemySaveData> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				// Sets enemies target
				TargetSensor targetSensor = _enemies[i].TargetSensor;
				if (targetSensor.CurrentTarget != null)
					data[i].SetTargetGUID(targetSensor.CurrentTarget.GUIDComponent.GUID);

				// Sets enemies station
				StationSensor stationSensor = _enemies[i].StationSensor;
				if (stationSensor != null)
					if (stationSensor.CurrentStation != null)
						data[i].SetTargetGUID(stationSensor.CurrentStation.GUIDComponent.GUID);
			}

			return data;
		}

		/// <summary>
		/// Gathers all the enemies data needed for saving
		/// </summary>
		/// <returns>A list of enemy data structs to be saved</returns>
		private List<EnemySaveData> GetEnemySaveData()
		{
			List<EnemySaveData> enemySaveData = new List<EnemySaveData>();
			_enemies = new List<Enemy>();
			for (int i = 0; i < (int)EnemyType.Count; i++)
			{
				List<PoolableObject> objs = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(((EnemyType)i).ToString());
				for (int o = 0; o < objs.Count; o++)
				{
					enemySaveData.Add((EnemySaveData)((SaveableEnemy)objs[o].SaveableObject).SaveData());
					_enemies.Add(((SaveableEnemy)objs[o].SaveableObject).Enemy);
				}
			}
			return enemySaveData;
		}

		/// <summary>
		/// Gathers all the players data needed for saving
		/// </summary>
		/// <returns>A list of player data structs to be saved</returns>
		private List<PlayerSaveData> GetPlayerSaveData()
		{
			List<PlayerSaveData> playerSaveDatas = new List<PlayerSaveData>();

			List<PoolableObject> players = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType("Player");
			_players = new List<Player>();

			for (int i = 0; i < players.Count; i++)
			{
				playerSaveDatas.Add((PlayerSaveData)((SaveablePlayer)players[i].SaveableObject).SaveData());
				_players.Add(((SaveablePlayer)players[i].SaveableObject).RoleHandler.Player);
			}

			return playerSaveDatas;
		}

		/// <summary>
		/// Gathers all world generation data needed for saving
		/// </summary>
		/// <returns>The struct of world generation data</returns>
		private WorldGenSaveData GetWorldGenerationData()
		{
			WorldGenSaveData worldGenData = new WorldGenSaveData();

			// The generated mesh 
			worldGenData.MapMesh = new MeshSaveData(GameManager.Instance.ProceduralWorldGenerator.GeneratedMesh);

			// The generated resources
			List<ResourceSaveData> resources = new List<ResourceSaveData>();

			for (int i = 0; i < (int)ResourceType.Count; i++)
			{
				if ((ResourceType)i != ResourceType.Fish)
				{
					List<PoolableObject> objs = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(((ResourceType)i).ToString());
					for (int o = 0; o < objs.Count; o++)
					{
						resources.Add((ResourceSaveData)((SaveableResource)objs[o].SaveableObject).SaveData());
					}
				}
			}
			worldGenData.Resources = resources;

			// The generated foliage
			List<FoliageSaveData> foliage = new List<FoliageSaveData>();

			for (int i = 0; i < (int)FoliageSaveType.Count; i++)
			{
				List<PoolableObject> objs = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(((FoliageSaveType)i).ToString());

				for (int o = 0; o < objs.Count; o++)
				{
					foliage.Add((FoliageSaveData)((SaveablFoliage)objs[o].SaveableObject).SaveData());
				}
			}

			worldGenData.Foliage = foliage;

			// The generated enemy camps
			List<EnemyCampSaveData> camps = new List<EnemyCampSaveData>();

			List<PoolableObject> campObjects = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(SaveItem.EnemyCamp_Goblin);
			for (int i = 0; i < campObjects.Count; i++)
			{
				EnemyCampSaveData enemyCampSaveData = new EnemyCampSaveData(campObjects[i].transform, ((SaveableEnemyCamp)campObjects[i].SaveableObject).HealthHandler.Health, GameManager.Instance.GUIDManager.CreateGUIDandAddToDictionary(campObjects[i]));
				camps.Add(enemyCampSaveData);
			}
			worldGenData.EnemyCamps = camps;

			return worldGenData;
		}

		/// <summary>
		/// Gathers all the buildings data needed for saving
		/// </summary>
		/// <returns>A list of building data structs to be saved to file</returns>
		private List<BuildingSaveData> GetBuildingsData()
		{
			List<BuildingSaveData> buildings = new List<BuildingSaveData>();
			for (int i = 0; i < (int)BuildingType.Count; i++)
			{
				List<PoolableObject> objs = GameManager.Instance.PoolingManager.GetAllActivePooledObjectsOfType(((BuildingType)i).ToString());
				if (objs != null)
					for (int o = 0; o < objs.Count; o++)
						buildings.Add((BuildingSaveData)((SaveableBuilding)objs[o].SaveableObject).SaveData());

			}
			return buildings;
		}

		/// <summary>
		/// Gathers the world data needed for saving
		/// </summary>
		/// <returns>The struct of world data to be saved to file</returns>
		private WorldSaveData GetWorldData()
		{
			WorldSaveData worldSaveData = new WorldSaveData();
			worldSaveData.WoodResourceAmount = GameManager.Instance.TownResourceManager.GetResourceAmount(Resource.Wood);
			worldSaveData.OreResourceAmount = GameManager.Instance.TownResourceManager.GetResourceAmount(Resource.Ore);
			worldSaveData.GoldResourceAmount = GameManager.Instance.TownResourceManager.GetResourceAmount(Resource.Gold);
			worldSaveData.FoodResourceAmount = GameManager.Instance.TownResourceManager.GetResourceAmount(Resource.Food);
			worldSaveData.WorldAgeInSeconds = GameManager.Instance.TimeManager.WorldTimePassed;


			// Tech Tree
			TechTreeSaveData techTree = new TechTreeSaveData();
			techTree.UnlockedTechs = GameManager.Instance.TechTreeManager.TechTree.GetUnlockedNodes();

			if (GameManager.Instance.TechTreeManager.CurrentTech != null)
			{
				techTree.CurrentTechName = GameManager.Instance.TechTreeManager.CurrentTech.name;
				List<ObjectiveSaveData> objectives = new List<ObjectiveSaveData>();
				Node_SO currentNode = GameManager.Instance.TechTreeManager.CurrentTech;
				Goal goal = GameManager.Instance.TownGoalManager.CurrentGoals[0];
				List<Objective> objs = new List<Objective>();

				foreach (KeyValuePair<Objective, bool> obj in goal.ObjectivesStatuses)
				{
					objectives.Add(new ObjectiveSaveData(obj.Key.Amount, obj.Key.RequiredAmount));
				}

				techTree.CurrentTechData = objectives;
				techTree.TechAvailable = true;
			}
			else
				techTree.TechAvailable = false;

			worldSaveData.IsCurrentRuler = GameManager.Instance.PlayerManager.Ruler == null ? false : true;

			worldSaveData.TimeUntillNextRulerVote = GameManager.Instance.GameEventManager.TimeTillRulerVote;
			if (worldSaveData.IsCurrentRuler)
				worldSaveData.RulerName = GameManager.Instance.PlayerManager.Ruler.TwitchUser.Username;

			worldSaveData.TechTree = techTree;
			return worldSaveData;
		}


		/// <summary>
		/// Starts the loading coroutine,
		/// </summary>
		public void LoadGame()
		{
			StartCoroutine(DelayedLoadGame());
		}

		// TODO: Seperate this into multiple functions
		/// <summary>
		/// Loads the game from file.
		/// </summary>
		/// <returns></returns>
		private IEnumerator DelayedLoadGame()
		{
			yield return null;
			SaveGameData gameData = GameIO.LoadGameData();
			SavePlayersData playersData = GameIO.LoadPlayersData();

			WorldGenSaveData genData = gameData.WorldGenData;
			List<BuildingSaveData> buildings = gameData.BuildingSaveData;
			WorldSaveData worldData = gameData.WorldSaveData;
			List<EnemySaveData> enemies = gameData.EnemySaveData;

			List<PlayerSaveData> playerSaveDatas = playersData.PlayerSaveDatas;

			// World generation mesh
			Mesh meshData = genData.MapMesh.GetMeshFromData();

			GameManager.Instance.ProceduralWorldGenerator.SetMesh(meshData);

			// Resources
			for (int i = 0; i < genData.Resources.Count; i++)
			{
				((SaveableResource)((GameManager.Instance.PoolingManager.GetPooledObject(genData.Resources[i].ResourceType, false)).SaveableObject)).LoadData((object)genData.Resources[i]);
			}

			// Foliage
			for (int i = 0; i < genData.Foliage.Count; i++)
			{
				((SaveablFoliage)((GameManager.Instance.PoolingManager.GetPooledObject(genData.Foliage[i].FoliageType, false)).SaveableObject)).LoadData((object)genData.Foliage[i]);
			}

			// Enemy camps
			for (int i = 0; i < genData.EnemyCamps.Count; i++)
			{
				((SaveableEnemyCamp)((GameManager.Instance.PoolingManager.GetPooledObject("EnemyCamp_Goblin", false)).SaveableObject)).LoadData((object)genData.EnemyCamps[i]);
			}

			//  Buildings
			List<UpdateGraphBounds> buildingsToUpdate = new List<UpdateGraphBounds>();
			for (int i = 0; i < buildings.Count; i++)
			{
				var building = GameManager.Instance.PoolingManager.GetPooledObject(buildings[i].BuildingType, false);
				 ((SaveableBuilding)((building).SaveableObject)).LoadData((object)buildings[i]);

				UpdateGraphBounds ugb = building.GetComponent<UpdateGraphBounds>();
				if (ugb)
					buildingsToUpdate.Add(ugb);
			}

			// Creates enemies
			List<PoolableObject> enemyObjs = new List<PoolableObject>();// fill list with enemy objs
			for (int i = 0; i < enemies.Count; i++)
			{
				PoolableObject temp = GameManager.Instance.PoolingManager.GetPooledObject((enemies[i].EnemyType.ToString()), false);
				((SaveableEnemy)temp.SaveableObject).LoadData((object)enemies[i]);
				enemyObjs.Add(temp);
			}

			// Creates players
			List<PoolableObject> playerObjs = new List<PoolableObject>(); // fill list with player objs
																		  // Create players
			for (int i = 0; i < playerSaveDatas.Count; i++)
			{
				playerObjs.Add(GameManager.Instance.PlayerManager.AddExistingPlayer(playerSaveDatas[i].ToPlayer(playerSaveDatas[i].GUID, playerSaveDatas[i].TargetGUID, playerSaveDatas[i].StationGUID), playerSaveDatas[i].CurrentRole).PoolableObject);
				//GameManager.Instance.GUIDManager.AddToDictionary(playerObjs[playerObjs.Count - 1]);
			}

			// TODO: Test this, problem where players/enemies automaticly go to the closest target/station

			// Setting player connections
			//for (int i = 0; i < playerObjs.Count; i++)
			//{
			//	if (playerSaveDatas[i].TargetGUID != 0)
			//		((SaveablePlayer)playerObjs[i].SaveableObject).RoleHandler.Player.TargetSensor.TrySetTarget(((SaveableObject)GameManager.Instance.GUIDManager.GetComponentFromID(playerSaveDatas[i].TargetGUID).SaveableObject).Target);
			//	if (playerSaveDatas[i].StationGUID != 0)
			//		((SaveablePlayer)playerObjs[i].SaveableObject).RoleHandler.Player.StationSensor.TrySetStation(((SaveablePlayer)GameManager.Instance.GUIDManager.GetComponentFromID(playerSaveDatas[i].StationGUID).SaveableObject).RoleHandler.Player.StationSensor.CurrentStation);
			//}

			//// Setting enemy connections
			//for (int i = 0; i < enemyObjs.Count; i++)
			//{
			//	if (enemies[i].TargetGUID != 0)
			//		((SaveableEnemy)enemyObjs[i].SaveableObject).Enemy.TargetSensor.TrySetTarget(((SaveableObject)GameManager.Instance.GUIDManager.GetComponentFromID(enemies[i].TargetGUID).SaveableObject).Target);
			//	if (enemies[i].CampGUID != 0)
			//		((SaveableEnemy)enemyObjs[i].SaveableObject).Enemy.StationSensor.TrySetStation(((SaveableEnemy)GameManager.Instance.GUIDManager.GetComponentFromID(enemies[i].CampGUID).SaveableObject).Enemy.StationSensor.CurrentStation);
			//}

			//Load worldSaveData
			GameManager.Instance.TownResourceManager.SetResourceAmount(Resource.Wood, worldData.WoodResourceAmount);
			GameManager.Instance.TownResourceManager.SetResourceAmount(Resource.Ore, worldData.OreResourceAmount);
			GameManager.Instance.TownResourceManager.SetResourceAmount(Resource.Food, worldData.FoodResourceAmount);
			GameManager.Instance.TownResourceManager.SetResourceAmount(Resource.Gold, worldData.GoldResourceAmount);
			GameManager.Instance.TimeManager.WorldTimePassed = worldData.WorldAgeInSeconds;
			GameManager.Instance.TimeManager.CalculateDayCount();
			GameManager.Instance.SeasonManager.SetSeasonByTimePassed();
			GameManager.Instance.TechTreeManager.TechTree.SetUnlockedNodes(worldData.TechTree.UnlockedTechs);
			if (worldData.TechTree.TechAvailable)
			{
				Goal goal = GameManager.Instance.TechTreeManager.StartGoalFromNode(GameManager.Instance.TechTreeManager.TechTree.GetNodeFromName(worldData.TechTree.CurrentTechName));
				goal.SetobjectivesFromSave(worldData.TechTree.CurrentTechData);
			}
			else
				GameManager.Instance.TechTreeManager.StartNewTechVote(20);

			GameManager.Instance.GameEventManager.TimeTillRulerVote = worldData.TimeUntillNextRulerVote;

			// Sets ruler
			if (worldData.IsCurrentRuler && GameManager.Instance.PlayerManager.PlayerExistsByNameToLower(worldData.RulerName, out int index))
				GameManager.Instance.PlayerManager.SetRuler(GameManager.Instance.PlayerManager.GetPlayer(index));
			else
				GameManager.Instance.GameEventManager.StartNewRulerVote();

			// Force all buildings to update their graph bounds.
			_generationObject.ScanWorld();

			yield return new WaitForSeconds(25);
			for(int i = 0; i < buildingsToUpdate.Count;i++)
			{
				buildingsToUpdate[i].SetGraphBounds();
			}
		}

		private void OnEnable()
		{
			//PlayerInputManager.OnEscape += EscapePressed;
			//PlayerInputManager.OnSaveGame += SaveGame;
		}

		private void OnDisable()
		{
			//PlayerInputManager.OnEscape -= EscapePressed;
			//PlayerInputManager.OnSaveGame -= SaveGame;
		}
	}
}