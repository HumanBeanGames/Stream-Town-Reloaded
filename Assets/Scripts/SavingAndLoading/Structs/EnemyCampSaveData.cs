using UnityEngine;

namespace SavingAndLoading.Structs 
{
    /// <summary>
    /// A struct holding information on the Enemy Camps
    /// </summary>
    [System.Serializable]
    public struct EnemyCampSaveData 
	{
        public TransformSaveData Transform;
        public int Health;
        public uint GUID;

        /// <summary>
        /// Sets the values of EnemyCampSaveData
        /// </summary>
        /// <param name="transform">The camps transform</param>
        /// <param name="health">The camps health</param>
        public EnemyCampSaveData(Transform transform, int health, uint gUID)
        {
            Transform = new TransformSaveData(transform);
            Health = health;
            GUID = gUID;
        }
    }
}