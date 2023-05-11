using UnityEngine;
using Utils;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct Holding information needed to load foliage from a save file
    /// </summary>
    
    [System.Serializable]
    public struct FoliageSaveData 
	{
        public TransformSaveData FoliageTransform;
        public string FoliageType;

        /// <summary>
        /// Sets the values of FoliageSaveData
        /// </summary>
        /// <param name="transform">The foliage transform</param>
        /// <param name="type">The foliage type</param>
        public FoliageSaveData(Transform transform, string type)
        {
            FoliageTransform = new TransformSaveData(transform);
            FoliageType = type;
        }
    }
}