using UnityEngine;

namespace Utils
{
	/// <summary>
	/// Holds a given data type and chance, used for randomly choosing an object based on chance.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class ChanceObject<T>
	{
		[SerializeField]
		public T Object;
		[SerializeField]
		public float Chance;
	}
}