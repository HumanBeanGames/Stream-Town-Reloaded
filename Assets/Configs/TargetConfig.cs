using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;
using Target;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Target Manager Config")]
    public class TargetConfig : Config<TargetConfig>
    {
        [SerializeField]
        public TargetableData[] targetableData;
    }
}
