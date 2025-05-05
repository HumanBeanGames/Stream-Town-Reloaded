using UnityEngine;
using Utils;
using System.Collections.Generic;
using Buildings;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Station Config")]
    public class StationConfig : Config<StationConfig>
    {
        [HideInInspector]
        public Dictionary<StationMask, List<Station>> stationsDictionary = new Dictionary<StationMask, List<Station>>();
        [HideInInspector]
        public Queue<Station> stationUpdateQueue = new Queue<Station>();
        [HideInInspector]
        public Queue<Station> clearDisabledQueue = new Queue<Station>();
    }
}
