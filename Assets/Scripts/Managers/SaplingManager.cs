using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Target;
using Utils;
using Managers;
using World.Generation;
using Utils.Pooling;
using World;

namespace Environment
{
    /// <summary>
    /// Manages all trees in the environment and places SaplingLocation targets
    /// at valid positions near trees or randomly. Foresters detect these with TargetSensor.
    /// </summary>
    public class SaplingManager : MonoBehaviour
    {
        [Header("Spawn Parameters")]
        [SerializeField] private GameObject saplingLocationPrefab;
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private float treeSpawnRadius = 15f;
        [SerializeField] private float saplingLifespan = 20f;
        [SerializeField] private float nearTreeInterval = 5f;
        [SerializeField] private float randomInterval = 15f;

        private float nearTreeTimer;
        private float randomTimer;

        private List<Targetable> _treeTargets;
        private ResourceGenerationSettings _treeGenerationSettings;
        private ObjectPoolingManager _poolingManager;

        private void Start()
        {
            //CacheTreeList();
            CacheTreeGenerationSettings();
            _poolingManager = GameManager.Instance.PoolingManager;
        }

        private void Update()
        {
            nearTreeTimer += Time.deltaTime;
            randomTimer += Time.deltaTime;

            if (nearTreeTimer >= nearTreeInterval)
            {
                if (TryPlaceNearTree())
                {
                    nearTreeTimer = 0f;
                    randomTimer = 0f;
                }
            }

            if (randomTimer >= randomInterval)
            {
                if (TryPlaceRandom())
                {
                    randomTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Caches a direct reference to the list of trees tracked by the TargetManager.
        /// </summary>
        public void CacheTreeList()
        {
            if (GameManager.Instance != null && GameManager.Instance.TargetManager != null)
            {
                _treeTargets = GameManager.Instance.TargetManager.GetSingleTargetList(TargetMask.Tree);
                Debug.Log($"[SaplingManager] Cached {_treeTargets.Count} tracked trees.");
            }
        }

        /// <summary>
        /// Caches the resource generation settings used for tree placement.
        /// </summary>
        private void CacheTreeGenerationSettings()
        {
            var worldGen = GameManager.Instance.ProceduralWorldGenerator;
            if (worldGen == null)
            {
                Debug.LogWarning("[SaplingManager] ProceduralWorldGenerator not found for bounds.");
                return;
            }

            var settingsList = typeof(ProceduralWorldGenerator)
                .GetField("_resourceGenerationSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(worldGen) as List<ResourceGenerationSettings>;

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
        private bool TryPlaceNearTree()
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
        private bool TryPlaceRandom()
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
        private bool TrySpawnAt(Vector3 position)
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

            GameObject sapling = _poolingManager.GetPooledObject(saplingLocationPrefab.name, false).gameObject;
            sapling.transform.position = spawnPoint;
            sapling.transform.rotation = Quaternion.identity;
            sapling.SetActive(true);
            return true;
        }
    }
}
