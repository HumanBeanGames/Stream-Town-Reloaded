using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;
using Target;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Building Manager Config")]
    public class BuildingConfig : Config<BuildingConfig>
    {
        public int globalBuildCostModifier = 0;
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public AllBuildingDataScriptable allBuildingData;
    }
}
