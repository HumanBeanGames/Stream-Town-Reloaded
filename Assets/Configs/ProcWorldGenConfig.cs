using UnityEngine;
using Sirenix.OdinInspector;
using World.Generation;
using System.Collections.Generic;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Proc World Gen Config")]
    public class ProcWorldGenConfig : Config<ProcWorldGenConfig>
    {
        public float xScale = 4;
        public float yScale = 4;
        public GenerationSettings generationSettings;
        public List<ResourceGenerationSettings> resourceGenerationSettings;
        public List<ResourceGenerationSettings> waterResourceGenerationSettings;
        public List<FoliageGenerationSettings> foliageGenerationSettings;
        public List<FoliageGenerationSettings> waterFoliageGenerationSettings;
        public List<CampGenerationSettings> campGenerationSettings;
        public bool generateOnStart = true;
        public bool randomizeSeed = true;
        public LayerMask collisionMask;
        public LayerMask terrainMask;

        [Button("Generate Terrain")]
        public void GenerateTerrainButton()
        {
            ProcWorldGenManager.GenerateTerrain();
        }

        [Button("Generate Fake Resources")]
        public void GenerateFakeResourcesButton()
        {
            ProcWorldGenManager.MainMenuGenerateWorld();
        }
    }
}
