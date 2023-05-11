using UnityEngine;
namespace SavingAndLoading.Structs 
{
    /// <summary>
    /// Struct for converting Unitys Vector2 class into a binary formatter friendly Vector2
    /// </summary>
    [System.Serializable]
    public struct Vector2SaveData 
	{
        public float X;
        public float Y;

        /// <summary>
        /// Overloaded constructor,
        /// Creates a vector2SaveData from Unity's Vector2,
        /// </summary>
        /// <param name="vector">Unity Vector2 to be converted</param>
        public Vector2SaveData(Vector2 vector)
        {
            X = vector.x;
            Y = vector.y;
        }

        /// <summary>
        /// Overloaded constructor,
        /// Creates a Vector2SaveData from 2 floats,
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        public Vector2SaveData(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Converts Vector2SaveData to a Unity Vector2
        /// </summary>
        /// <returns>The Unity Vector2</returns>
        public static Vector2 ToUnityVec2(Vector2SaveData data)
        {
            return new Vector2(data.X, data.Y);
        }

        /// <summary>
        /// Converts Unity's Vector2[] to a Vector2SaveData[]
        /// </summary>
        /// <param name="vector2Array">An array of Unity's Vector2s</param>
        /// <returns>An array of Vector2SaveData</returns>
        public static Vector2SaveData[] ToVector2SaveDataArray(Vector2[] vector2Array)
        {
            Vector2SaveData[] array = new Vector2SaveData[vector2Array.Length];

            for(int i = 0; i < vector2Array.Length; i++)
            {
                array[i] = new Vector2SaveData(vector2Array[i]);
            }

            return array;
        }
        
        /// <summary>
        /// Converts a Vector2SaveData[] to Unity's Vector2[]
        /// </summary>
        /// <param name="vector2SaveDataArray">An array of Unity's Vector2s</param>
        /// <returns>An array of Unity Vector2s</returns>
        public static Vector2[] ToUnityVector2Array(Vector2SaveData[] vector2SaveDataArray)
        {
            Vector2[] array = new Vector2[vector2SaveDataArray.Length];

            for(int i = 0; i < vector2SaveDataArray.Length; i++)
            {
                array[i] = ToUnityVec2(vector2SaveDataArray[i]);
            }

            return array;
        }
    }
}