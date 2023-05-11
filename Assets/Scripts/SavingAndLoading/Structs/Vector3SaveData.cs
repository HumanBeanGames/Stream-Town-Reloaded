using UnityEngine;

namespace SavingAndLoading.Structs 
{
    /// <summary>
    /// Struct for converting Unitys Vector3 class into a binary formatter friendly Vector3
    /// </summary>
    [System.Serializable]
    public struct Vector3SaveData
    {
        public float X;
        public float Y;
        public float Z;

        /// <summary>
        /// Overloaded constructor,
        /// Creates a Vector3SaveData from Unity's Vector3,
        /// </summary>
        /// <param name="vector">Unity Vector3 to be converted</param>
        public Vector3SaveData(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        /// <summary>
        /// Overloaded constructor,
        /// Creates a Vector3SaveData from 3 floats,
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3SaveData(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Converts Vector2SaveData to Unity Vector3
        /// </summary>
        /// <returns></returns>
        public static Vector3 ToUnityVec3(Vector3SaveData data)
        {
            return new Vector3(data.X, data.Y, data.Z);
        }

        /// <summary>
        /// Converts Unity's Vector3[] to a Vector3SaveData[]
        /// </summary>
        /// <param name="vector3Array">An array of Unity's Vector3s</param>
        /// <returns>An array of Vector3SaveData</returns>
        public static Vector3SaveData[] ToVector3SaveDataArray(Vector3[] vector3Array)
        {
            Vector3SaveData[] array = new Vector3SaveData[vector3Array.Length];

            for (int i = 0; i < vector3Array.Length; i++)
            {
                array[i] = new Vector3SaveData(vector3Array[i]);
            }

            return array;
        }

        /// <summary>
        /// Converts a Vector3SaveData[] to Unity's Vector3[]
        /// </summary>
        /// <param name="vector3SaveDataArray">An array of Unity's Vector3s</param>
        /// <returns>An array of Unity Vector2s</returns>
        public static Vector3[] ToUnityVector3Array(Vector3SaveData[] vector3SaveDataArray)
        {
            Vector3[] array = new Vector3[vector3SaveDataArray.Length];

            for (int i = 0; i < vector3SaveDataArray.Length; i++)
            {
                array[i] = ToUnityVec3(vector3SaveDataArray[i]);
            }

            return array;
        }   }
}