using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Target;
using UnityEngine;
using Utils;
using World;
using World.Generation;

namespace Environment
{
    /// <summary>
    /// Manages all trees in the environment and places SaplingLocation targets
    /// at valid positions near trees or randomly. Foresters detect these with TargetSensor.
    /// </summary>
    [GameManager]
    public static class SaplingManager
    {
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static SaplingConfig Config = SaplingConfig.Instance;

        [Header("Spawn Parameters")]
        [HideInInspector] private static GameObject SaplingLocationPrefab => Config.saplingLocationPrefab;
        [HideInInspector] private static GameObject TreePrefab => Config.treePrefab;
        [HideInInspector] private static float TreeSpawnRadius => Config.treeSpawnRadius;
        [HideInInspector] private static float SaplingLifespan => Config.saplingLifespan;
        [HideInInspector] private static float NearTreeInterval => Config.nearTreeInterval;
        [HideInInspector] private static float RandomInterval => Config.randomInterval;

        [HideInInspector] private static float _nearTreeTimer;
        [HideInInspector] private static float _randomTimer;

        [HideInInspector] private static List<Targetable> _treeTargets;
        [HideInInspector] private static ResourceGenerationSettings _treeGenerationSettings;

        private class Runner : MonoBehaviour
        {
            private void OnEnable()
            {
                DontDestroyOnLoad(this);
            }
        }
        [HideInInspector]
        private static Runner runner;

        private static Runner RunnerInstance
        {
            get
            {
                if (runner == null)
                {
                    GameObject runnerObject = new GameObject("SaplingManagerRunner");
                    runnerObject.hideFlags = HideFlags.HideAndDontSave;
                    runner = runnerObject.AddComponent<Runner>();
                }
                return runner;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return RunnerInstance.StartCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            RunnerInstance.StopCoroutine(coroutine);
        }

        private static IEnumerator UpdateSaplingCoroutine()
        {
            while (true)
            {
                _nearTreeTimer += Time.deltaTime;
                _randomTimer += Time.deltaTime;

                if (_nearTreeTimer >= NearTreeInterval)
                {
                    if (TryPlaceNearTree())
                    {
                        _nearTreeTimer = 0f;
                        _randomTimer = 0f;
                    }
                }

                if (_randomTimer >= RandomInterval)
                {
                    if (TryPlaceRandom())
                    {
                        _randomTimer = 0f;
                    }
                }
                yield return null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            CacheTreeGenerationSettings();
            StartCoroutine(UpdateSaplingCoroutine());
        }

        /// <summary>
        /// Caches a direct reference to the list of trees tracked by the TargetManager.
        /// </summary>
        public static void CacheTreeList()
        {
            _treeTargets = TargetManager.GetSingleTargetList(TargetMask.Tree);
            Debug.Log($"[SaplingManager] Cached {_treeTargets.Count} tracked trees.");
        }

        /// <summary>
        /// Caches the resource generation settings used for tree placement.
        /// </summary>
        private static void CacheTreeGenerationSettings()
        {
            var settingsList = ProcWorldGenManager.GetResourceGenerationSettings();

            if (settingsList != null)
            {
                foreach (var s in settingsList)
                {
                    if (s.TargetType == TargetMask.Tree)
                    {
                        _treeGenerationSettings = s;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to place a sapling near a tracked tree.
        /// </summary>
        private static bool TryPlaceNearTree()
        {
            if (_treeTargets == null || _treeTargets.Count == 0)
            {
                Debug.Log("[SaplingManager] No tracked trees found.");
                return false;
            }

            Transform randomTree = _treeTargets[Random.Range(0, _treeTargets.Count)].transform;
            Vector3 offset = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            Vector3 spawnPos = randomTree.position + offset;
            return TrySpawnAt(spawnPos);
        }

        /// <summary>
        /// Attempts to place a sapling randomly within the procedural world bounds.
        /// </summary>
        private static bool TryPlaceRandom()
        {
            if (_treeGenerationSettings == null)
            {
                Debug.LogWarning("[SaplingManager] Tree generation settings not available.");
                return false;
            }

            float size = _treeGenerationSettings.Size;
            float half = size / 2f;

            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-half + 2f, half - 2f), 0, Random.Range(-half + 2f, half - 2f));
                if (TrySpawnAt(pos))
                    return true;
            }

            Debug.Log("[SaplingManager] Failed to place random sapling within world bounds.");
            return false;
        }

        /// <summary>
        /// Tries to spawn a SaplingLocation at a valid world position using terrain validation.
        /// </summary>
        private static bool TrySpawnAt(Vector3 position)
        {
            (bool valid, float height) = WorldUtils.OnGroundCheckHeight(position);

            if (!valid)
            {
                Debug.Log("[SaplingManager] Invalid ground check for position: " + position);
                return false;
            }

            Vector3 spawnPoint = new Vector3(position.x, height, position.z);

            Collider[] overlaps = Physics.OverlapSphere(spawnPoint, 0.5f);
            foreach (var c in overlaps)
            {
                if (!c.CompareTag("Untagged"))
                {
                    Debug.Log("[SaplingManager] Overlap detected at: " + spawnPoint);
                    return false;
                }
            }

            GameObject sapling = ObjectPoolingManager.GetPooledObject(SaplingLocationPrefab.name, false).gameObject;
            sapling.transform.position = spawnPoint;
            sapling.transform.rotation = Quaternion.identity;
            sapling.SetActive(true);
            return true;
        }
    }
}
