using UnityEngine;
using System.Collections.Generic;

namespace SavingAndLoading.Structs 
{
    /// <summary>
    /// A struct to hold transform information
    /// </summary>
    [System.Serializable]
    public struct TransformSaveData 
	{
        public Vector3SaveData Position;
        public Vector3SaveData Rotation;
        public Vector3SaveData LossyScale;

        /// <summary>
        /// Overloaded constructor,
        /// Converts Unitys transform to TransformSaveData
        /// </summary>
        /// <param name="transform">Unitys transform</param>
        public TransformSaveData(Transform transform)
        {
            Position = new Vector3SaveData(transform.position);
            Rotation = new Vector3SaveData(transform.eulerAngles);
            LossyScale = new Vector3SaveData(transform.lossyScale);
        }
    }
}