using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct Holding information needed to load buildings from a save file
    /// </summary>
    [System.Serializable]
    public struct BuildingSaveData
    {
        public TransformSaveData BuildingTranform;
        public string BuildingType;
        public int BuildingHealth;
        public uint GUID;
        public BuildingState BuildingState;
        public List<FoliageSaveData> DestroyedFoliage;
        /// <summary>
        /// Sets the values of BuildingSaveData
        /// </summary>
        /// <param name="transform">The buildings transform</param>
        /// <param name="type">The building type</param>
        /// <param name="health">The buildings health</param>
        public BuildingSaveData(Transform transform, string type, int health, uint gUID,BuildingState state, List<FoliageSaveData> desstroyedFoliage)
        {
            BuildingTranform =  new TransformSaveData(transform);
            BuildingType = type;
            BuildingHealth = health;
            
            GUID = gUID;    
            BuildingState = state;
            DestroyedFoliage = desstroyedFoliage;
        }
    }
}