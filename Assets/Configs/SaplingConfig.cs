using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Sapling Manager Config")]
    public class SaplingConfig : Config<SaplingConfig>
    {
        public GameObject saplingLocationPrefab;
        public GameObject treePrefab;
        public float treeSpawnRadius = 15f;
        public float saplingLifespan = 20f;
        public float nearTreeInterval = 5f;
        public float randomInterval = 15f;
    }
}
