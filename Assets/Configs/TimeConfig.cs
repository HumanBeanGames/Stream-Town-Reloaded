using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Time Manager Config")]
    public class TimeConfig : Config<TimeConfig>
    {
        public int secondsPerDay = 3600;
        [HideInInspector]
        public int dayCount = 0;
        [HideInInspector]
        public float worldTimePassed = 0;
    }
}
