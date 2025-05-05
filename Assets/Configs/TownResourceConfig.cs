using UnityEngine;
using Utils;
using Character;
using System.Collections.Generic;
using GameResources;
using Events;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Town Resource Config")]
    public class TownResourceConfig : Config<TownResourceConfig>
    {
        public float RESOURCE_RATE_TIME_PERIOD = 25f;
        public float RESOURCE_UPDATE_RATE = 1f;

        public Dictionary<Resource, ResourceInventory> resources = new Dictionary<Resource, ResourceInventory>();
        public Dictionary<Resource, ResourceRateOfChange> resourcesRateOfChange = new Dictionary<Resource, ResourceRateOfChange>();
        public Dictionary<Resource, StorageStatusEventSO> onResourceChangeEventDict = new Dictionary<Resource, StorageStatusEventSO>();
        public Dictionary<Resource, int> resourceBoostValues = new Dictionary<Resource, int>();
    }
}
