using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Sapling Manager Config")]
    public class SaplingConfig : Config<SaplingConfig>
    {

        [Header("Spawn Parameters")]
        public GameObject saplingLocationPrefab;
        public GameObject treePrefab;
        public float treeSpawnRadius;
        public float saplingLifespan;
        public float nearTreeInterval;
        public float randomInterval;

    }
}
