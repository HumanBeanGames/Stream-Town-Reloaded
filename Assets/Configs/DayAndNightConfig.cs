using UnityEngine;
using UnityEngine.Rendering;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/DayAndNight Manager Config")]
    public class DayAndNightConfig : Config<DayAndNightConfig>
    {
        public float transitionLength;
        public float dayPercentage;
        public float nightLightIntensity;
        public float dayLightIntensity;
        public Material buildingMaterial;
        public float maxEmissionStrength;
        public GameObject dayPP;
        public GameObject nightPP;
    }
}
