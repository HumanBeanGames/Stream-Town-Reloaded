using UnityEngine;
using static UnityEngine.Rendering.STP;
using UnityEngine.VFX;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Weather Manager Config")]
    public class WeatherConfig : Config<WeatherConfig>
    {
        public VisualEffect autumnVFX;
        public VisualEffect winterVFX;
        public VisualEffect summerVFX;
        public VisualEffect springVFX;
    }
}
