using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;
using Target;
using GridSystem.Partitioning;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/CSP Config")]
    public class CSPConfig : Config<CSPConfig>
    {
        public Vector2 originOffset;
        public float width = 100;
        public float length = 100;
        public float cellWidth = 10;
        public float cellLength = 10;

        [Button("Generate Partitions")]
        public void GeneratePartitionsButton()
        {
            CSPManager.GeneratePartitions();
        }
    }
}
