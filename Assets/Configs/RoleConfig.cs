using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;
using Target;
using GridSystem.Partitioning;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Role Manager Config")]
    public class RoleConfig : Config<RoleConfig>
    {
        public const int maxRoleLevel = 99;

        //debugging ReadOnly
        [ReadOnly]
        public int maxLevelXp;

        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public AllRoleDataScriptable allRoleData;

        public float ExponentialBase = 1.2f;
        public float ExponentTempering = 0.5f;
        public float Multiplier = 100f;

        //debugging ReadOnly
        [ReadOnly]
        public int[] expTableLookup = new int[maxRoleLevel];

        [Button("Regenerate XP Table")]
        public void CalculateEXPTable()
        {
            for (int i = 0; i < maxRoleLevel; i++)
            {
                expTableLookup[i] = Mathf.RoundToInt(Multiplier * (Mathf.Pow(Mathf.Pow(ExponentialBase, i - 1), ExponentTempering) - Mathf.Pow(Mathf.Pow(ExponentialBase, -1), ExponentTempering)));
            }
            maxLevelXp = expTableLookup[maxRoleLevel - 1];
        }
    }
}
