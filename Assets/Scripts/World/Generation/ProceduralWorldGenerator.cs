using Enemies;
using Environment;
using Managers;
using Pathfinding;
using SavingAndLoading.SavableObjects;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Pooling;

namespace World.Generation
{
	[GameManager]
	public static class ProceduralWorldGenerator
	{
        private static ProcWorldGenConfig Config => ProcWorldGenConfig.Instance;

        public static float XScale => Config.xScale;
        public static float YScale => Config.yScale;
        public static GenerationSettings GenerationSettings => Config.generationSettings;
        public static List<ResourceGenerationSettings> ResourceGenerationSettings => Config.resourceGenerationSettings;
        public static List<ResourceGenerationSettings> WaterResourceGenerationSettings => Config.waterResourceGenerationSettings;
        public static List<FoliageGenerationSettings> FoliageGenerationSettings => Config.foliageGenerationSettings;
        public static List<FoliageGenerationSettings> WaterFoliageGenerationSettings => Config.waterFoliageGenerationSettings;
        public static List<CampGenerationSettings> CampGenerationSettings => Config.campGenerationSettings;
        public static bool GenerateOnStart => Config.generateOnStart;
        public static bool RandomizeSeed => Config.randomizeSeed;
        public static LayerMask CollisionMask => Config.collisionMask;
        public static LayerMask TerrainMask => Config.terrainMask;

        /// <summary>
        /// The max number of attempts the generation will try to place an enemy camp. Prevents an infinite loop.
        /// </summary>
        private const int MAX_CAMP_GENERATION_ATTEMPTS = 500;
		private static Mesh _generatedMesh;

		public static Mesh GeneratedMesh => _generatedMesh;

        public static List<ResourceGenerationSettings> GetResourceGenerationSettings()
        {
            return ResourceGenerationSettings;
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
		private static bool _regen = false;
		[SerializeField, HideInInspector]
		private static bool _previewTreePlacements = false;
		[SerializeField, HideInInspector]
		private static bool _previewFoliagePlacements = false;
		[SerializeField, HideInInspector]
		private static Mesh _treeMesh;
		private static List<Vector3> _previewTreePositions = new List<Vector3>();
#endif
		/// <summary>
		/// Generates a terrain based on the stored settings.
		/// </summary>
        /// 
		
		public static bool IsPointWithinBounds(Vector3 point)
        {
			float xSize = XScale * GenerationSettings.Size;
			float ySize = YScale * GenerationSettings.Size;

			float minX = -(0.5f * xSize);
			float maxX = +(0.5f * xSize);
			float minZ = -(0.5f * ySize);
			float maxZ = +(0.5f * ySize);

			if (point.x > minX && point.x < maxX || point.x > maxX && point.x < minX)
				if (point.z > minZ && point.z < maxZ || point.z > maxZ && point.z < minZ)
					return true;

			return false;
		}
		public static void GenerateTerrain()
		{
			_generatedMesh = ProceduralMeshGenerator.CreateMesh(ProceduralMeshGenerator.GenerateTerrainMeshData(GenerationSettings), null);
		}

		public static void SetMesh(Mesh mesh)
		{
			_generatedMesh = ProceduralMeshGenerator.CreateMesh(mesh, null);
		}

		/// <summary>
		/// Attempts to generate enemy camps.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="camps"></param>
		/// <param name="poolManager"></param>
		//private static void GenerateEnemyCamps(CampGenerationSettings settings, ref List<GameObject> camps, ObjectPoolingManager poolManager)
		//{
		//	GameManager.Instance.EnemyCampSpawners = new List<Enemies.EnemySpawner>();
		//	//Attempt to place Enemy camps
		//	for (int i = 0; i < settings.MaxAmount; i++)
		//	{
		//		int attempts = 0;
		//		bool valid = false;
		//		while (!valid && attempts < MAX_CAMP_GENERATION_ATTEMPTS)
		//		{
		//			attempts++;
		//			bool failed = false;
		//			//Get Random Position in valid Range
		//			Vector3 randPos = new Vector3(UnityEngine.Random.Range(settings.MinBounds.x, settings.MaxBounds.x), 0, UnityEngine.Random.Range(settings.MinBounds.y, settings.MaxBounds.y));
		//			int r = UnityEngine.Random.Range(0, 2);
		//			if (r == 0)
		//				randPos.x *= -1;
		//			r = UnityEngine.Random.Range(0, 2);
		//			if (r == 0)
		//				randPos.z *= -1;
		//			int halfSize = settings.CampSize / 2;

		//			//Check that it is entirely over land
		//			for (int x = -halfSize; x <= halfSize; x++)
		//			{
		//				for (int y = -halfSize; y <= halfSize; y++)
		//				{
		//					if (WorldUtils.GetTerrainHeightAtPosition(randPos + new Vector3(x, 0, y)) != 0)
		//					{
		//						failed = true;
		//					}
		//				}
		//			}

		//			if (failed)
		//				continue;

		//			//If it is on land, check that its not too close to other camps
		//			for (int j = 0; j < camps.Count; j++)
		//			{
		//				if (Vector3.Distance(camps[j].transform.position, randPos) <= settings.MinDistanceFromOther)
		//					failed = true;
		//			}

		//			if (failed)
		//				continue;

		//			//Seems everything is fine... Spawn camp at the spot
		//			GameObject obj = poolManager.GetPooledObject(settings.GetPoolName(), false);
		//			obj.transform.position = randPos;
		//			obj.SetActive(true);
		//			GameManager.Instance.EnemyCampSpawners.Add(obj.GetComponent<EnemySpawner>());
		//			camps.Add(obj);
		//			break;
		//		}
		//	}
		//}

		/// <summary>
		/// Generates all pooled objects required for World Generation.
		/// </summary>
		private static IEnumerator GeneratePooledObjects()
		{
			int seed = GenerationSettings.Seed;
			DateTime before = DateTime.Now;
			DateTime after;
			TimeSpan duration;

			// Create townhall
			PoolableObject th = ObjectPoolingManager.GetPooledObject("Townhall");
			GameObject thObj = ((SaveableBuilding)th.SaveableObject).BuildingBase.gameObject;
			thObj.transform.position = Vector3.zero;
			thObj.SetActive(true);
			BuildingManager.AddLoadedBuilding(((SaveableBuilding)th.SaveableObject).BuildingBase);

			// Generate all normal resources (trees, ore, etc).
			if (ResourceGenerationSettings != null)
			{
				foreach (ResourceGenerationSettings settings in ResourceGenerationSettings)
				{
					before = DateTime.Now;
					GenerateFromSettings(settings, ref seed, WorldUtils.OnGroundCheckHeight);
					after = DateTime.Now;
					duration = after.Subtract(before);
					Debug.Log($"Generating {settings.PoolName} took {duration.TotalMilliseconds}ms");
					yield return new WaitForEndOfFrame();
				}

                // Tell the SaplingManager the trees exist now
                SaplingManager.CacheTreeList();
            }

			// Generate all resources for water on the shore line (fish).
			if (WaterResourceGenerationSettings != null)
				foreach (ResourceGenerationSettings settings in WaterResourceGenerationSettings)
				{
					before = DateTime.Now;
					GenerateFromSettings(settings, ref seed, WorldUtils.OnShoreLineCheckHeight);
					after = DateTime.Now;
					duration = after.Subtract(before);
					Debug.Log($"Generating {settings.PoolName} took {duration.TotalMilliseconds}ms");
					yield return new WaitForEndOfFrame();
				}

			// Generate the ground foliage (flowers, grass, etc).
			if (FoliageGenerationSettings != null)
				foreach (FoliageGenerationSettings settings in FoliageGenerationSettings)
				{
					before = DateTime.Now;
					GenerateFromSettings(settings, ref seed, WorldUtils.OnGroundCheckHeight);
					after = DateTime.Now;
					duration = after.Subtract(before);
					Debug.Log($"Generating {settings.PoolNames[0]} took {duration.TotalMilliseconds}ms");
					yield return new WaitForEndOfFrame();
				}

			// Generate the underwater foliage (seaweed, corals, etc.).
			if (WaterFoliageGenerationSettings != null)
				foreach (FoliageGenerationSettings settings in WaterFoliageGenerationSettings)
				{
					before = DateTime.Now;
					GenerateFromSettings(settings, ref seed, WorldUtils.UnderWaterCheckHeight, false);
					after = DateTime.Now;
					duration = after.Subtract(before);
					Debug.Log($"Generating {settings.PoolNames[0]} took {duration.TotalMilliseconds}ms");
					yield return new WaitForEndOfFrame();
				}
		}

		public static void MainMenuGenerateWorld()
		{
			WorldUtils.GroundLayerMask = LayerMask.GetMask("Ground");
			ObjectPoolingManager.SimplePoolObjects();
			int seed = GenerationSettings.Seed;

			// Generate all normal resources (trees, ore, etc).
			if (ResourceGenerationSettings != null)
			{
				foreach (ResourceGenerationSettings settings in ResourceGenerationSettings)
				{
					GenerateFromSettings(settings, ref seed, WorldUtils.OnGroundCheckHeight);
				}
			}

			// Generate the ground foliage (flowers, grass, etc).
			if (FoliageGenerationSettings != null)
				foreach (FoliageGenerationSettings settings in FoliageGenerationSettings)
				{
					GenerateFromSettings(settings, ref seed, WorldUtils.OnGroundCheckHeight);
				}

			// Generate the underwater foliage (seaweed, corals, etc.).
			if (WaterFoliageGenerationSettings != null)
				foreach (FoliageGenerationSettings settings in WaterFoliageGenerationSettings)
				{
					GenerateFromSettings(settings, ref seed, WorldUtils.UnderWaterCheckHeight, false);
				}
		}

		/// <summary>
		/// Uses settings to generate resources or objects in the world, accounting for collision to avoid overlap.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="seed"></param>
		/// <param name="poolManager"></param>
		/// <param name="comparisonLambda"></param>
		private static void GenerateFromSettings(GenerationSettings settings, ref int seed, Func<Vector3, (bool, float)> comparisonLambda, bool useCollision = true)
		{
			settings.Size = (GenerationSettings.Size * (int)XScale);
			settings.Seed = ++seed;
			//Generate resource map (stored in Height Map)
			Vector3 colSize = Vector3.one * settings.Spacing * 0.45f;
			settings.HeightMap = Noise.GenerateNoiseMap(settings);

			//Set pooled objects to the position
			int halfSize = (settings.Size) / 2;

			if (settings.Spacing == 0)
				settings.Spacing = 1;

			Vector3 position;
			for (int y = -halfSize + 2; y < halfSize - 2; y += settings.Spacing)
			{
				for (int x = -halfSize + 2; x < halfSize - 2; x += settings.Spacing)
				{
					position = new Vector3(y + settings.Offset.y, 0, x + settings.Offset.x);
					if (Mathf.FloorToInt(settings.HeightMap[x + halfSize, y + halfSize]) == 1)
					{
						(bool, float) lambaResult = comparisonLambda(position);

						if (lambaResult.Item1)
						{
							//TODO:: Put this as an out in the lambda
							position.y = lambaResult.Item2;

							//Check for collision
							if (useCollision)
								if (Physics.BoxCast(position + Vector3.up * 5, colSize, Vector3.down, Quaternion.identity, 10, CollisionMask))
									continue;

							PoolableObject obj = ObjectPoolingManager.GetPooledObject(settings.GetPoolName(), false);
							obj.transform.position = position;
							float randomRotation = UnityEngine.Random.Range(0, 4) * 90;
							obj.transform.Rotate(Vector3.up, randomRotation);
							obj.gameObject.SetActive(true);
						}
					}
				}
			}
		}

		/// <summary>
		/// Attempts to generate a new world with the given settings.
		/// </summary>
		public static IEnumerator TryGenerateWorld()
		{
			int tries = 0;
			if (GenerateOnStart)
			{
				WorldUtils.GroundLayerMask = LayerMask.GetMask("Ground");
				DateTime before = DateTime.Now;
				yield return new WaitForEndOfFrame();

				do
				{
					yield return new WaitForEndOfFrame();
					if (RandomizeSeed)
						GenerationSettings.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

					GenerateTerrain();
					yield return new WaitForEndOfFrame();

				} while (!AcceptableTerrainCheck() || ++tries == 10);

				if (tries != 10)
				{
					yield return GeneratePooledObjects();
					//Check that townhall is on ground, not above water.

					Debug.Log($"World generated in {tries} attempts!");

                    GameStateManager.NotifyWorldLoaded();
				}
				else
				{
					Debug.Log("World failed generation!");
				}
            }
		}

		private static bool AcceptableTerrainCheck()
		{
			// Check Town Hall is not above water
			if (!TownHallAboveGround())
			{
				Debug.Log("Town Hall Above Water, Regenerating Terrain");
				return false;
			}

			// Check all enemy spawns have a valid path to the town
			AstarPath.active.Scan();

			Transform[] enemySpawners = GameManager.Instance.EnemySpawner.SpawnLocations;
			GraphNode a = AstarPath.active.GetNearest(Vector3.zero, NNConstraint.Default).node;

			for (int i = 0; i < enemySpawners.Length; i++)
			{
				GraphNode b = AstarPath.active.GetNearest(enemySpawners[i].position, NNConstraint.Default).node;

				// Check for valid path
				if (!PathUtilities.IsPathPossible(a, b))
				{
					Debug.Log($"Path wasn't possible from Enemy Spawner {i}");
					return false;
				}
			}

			return true;
		}

		private static bool TownHallAboveGround()
		{
			int townHallCheckSize = 5;

			for (int i = -(townHallCheckSize) / 2; i < townHallCheckSize / 2; i++)
			{
				for (int j = -(townHallCheckSize / 2); j < townHallCheckSize / 2; j++)
				{
					if (Physics.Raycast(new Vector3(i, 5, j), Vector3.down, out RaycastHit info, 10, TerrainMask))
					{
						if (!WorldUtils.OnGroundCheck(info.point))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public static IEnumerator ScanWorld()
		{
			WorldUtils.GroundLayerMask = LayerMask.GetMask("Ground");
			yield return new WaitForEndOfFrame();
			GenerateTerrain();
			yield return GeneratePooledObjects();
			//Check that townhall is on ground, not above water.
			GameStateManager.NotifyWorldLoaded();
			AstarPath.active.Scan();
		}

		// Unity Functions.
		private static void OnValidate()
		{
			if (GenerationSettings == null)
				return;

			if (GenerationSettings.Lacunarity < 1)
				GenerationSettings.Lacunarity = 1;

			if (GenerationSettings.Octaves < 0)
				GenerationSettings.Octaves = 0;

		}

#if UNITY_EDITOR

		private static List<Vector3> GenerateDebugPositions(GenerationSettings settings, ref int seed, Func<Vector3, (bool, float)> comparisonLambda)
		{
			List<Vector3> listOfPositions = new List<Vector3>();

			settings.Size = (GenerationSettings.Size * (int)XScale);
			settings.Seed = ++seed;
			//Generate resource map (stored in Height Map)
			Vector3 colSize = Vector3.one * settings.Spacing * 0.45f;
			settings.HeightMap = Noise.GenerateNoiseMap(settings);

			//Set pooled objects to the position
			int halfSize = (settings.Size) / 2;

			if (settings.Spacing == 0)
				settings.Spacing = 1;

			for (int y = -halfSize; y < halfSize; y += settings.Spacing)
			{
				for (int x = -halfSize; x < halfSize; x += settings.Spacing)
				{
					if (Mathf.FloorToInt(settings.HeightMap[x + halfSize, y + halfSize]) == 1)
					{
						Vector3 position = new Vector3(y + settings.Offset.y, 0, x + settings.Offset.x);
						(bool, float) lambaResult = comparisonLambda(position);

						if (lambaResult.Item1)
						{
							position.y = lambaResult.Item2;

							listOfPositions.Add(position);
						}
					}
				}
			}

			return listOfPositions;
		}

		private static void OnDrawGizmosSelected()
		{
			// Preview Tree Placements
			if (_regen)
			{
				_regen = false;
				if (_previewTreePlacements)
				{
					ResourceGenerationSettings settings = default;

					for (int i = 0; i < ResourceGenerationSettings.Count; i++)
					{
						if (ResourceGenerationSettings[i].TargetType == TargetMask.Tree)
						{
							settings = ResourceGenerationSettings[i];
							break;
						}
					}

					_previewTreePositions = GenerateDebugPositions(settings, ref settings.Seed, WorldUtils.OnGroundCheckHeight);

				}
			}

			for (int i = 0; i < _previewTreePositions.Count; i++)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawMesh(_treeMesh, _previewTreePositions[i]);
				Gizmos.color = Color.white;
			}
		}
#endif
	}
}