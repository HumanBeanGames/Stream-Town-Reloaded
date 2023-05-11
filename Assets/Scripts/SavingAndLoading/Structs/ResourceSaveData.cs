using UnityEngine;
using Utils;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct for holding information for each resource
    /// </summary>
    [System.Serializable]
    public struct ResourceSaveData
    {
        public TransformSaveData ResourceTransform;
        public string ResourceType;
        public int ResourceAmountLeft;
        public uint GUID;

        /// <summary>
        /// Sets the values of ResourceSaveData
        /// </summary>
        /// <param name="transform">The resources transform</param>
        /// <param name="type">Thye resource type</param>
        /// <param name="resourceAmount">The amount of resources left</param>
        /// <param name="poolName">The name of the resource in the object pooler</param>
        public ResourceSaveData (Transform transform, string type, int resourceAmount, uint gUID)
        {
            ResourceTransform = new TransformSaveData(transform);
            ResourceType = type;
            ResourceAmountLeft = resourceAmount;
            GUID = gUID;
        }
    }
}