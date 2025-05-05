using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Season Manager Config")]
    public class SeasonConfig : Config<SeasonConfig>
    {
        [Title("Season Settings")]
        public Season startingSeason = Season.Summer;
        public int daysPerSeason = 3;
        public float seasonTransitionTime = 10;

        [HideInInspector]
        public bool seasonChanging = false;

        public Season currentSeason = Season.Summer;

        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public AllSeasonsScriptable allSeasonsData;

        [Title("Material References")]
        public Material grassMaterial;
        public Material terrainMaterial;
        public Material treeMaterial;
        public Material buildingMaterial;
        public Material waterMaterial;

        [Title("Tint Values")]
        public float winterTint = 0.42f;
        public float restTint = -0.08f;

#if UNITY_EDITOR
        [Title("Editor Settings")]
        public bool driveSeasonsByTime = false;
#endif
    }
}
