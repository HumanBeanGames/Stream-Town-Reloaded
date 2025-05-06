using UnityEngine;
using static UnityEngine.Rendering.STP;
using UnityEngine.VFX;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Weather Manager Config")]
    public class WeatherConfig : Config<WeatherConfig>
    {
        public GameObject autumnVFXPrefab;
        public GameObject winterVFXPrefab;
        public GameObject summerVFXPrefab;
        public GameObject springVFXPrefab;
    }
}
