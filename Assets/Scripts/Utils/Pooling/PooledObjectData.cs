using UnityEngine;

namespace Utils.Pooling
{
	/// <summary>
	/// Stores a prefab to be used for pooling.
	/// </summary>
	[System.Serializable]
	public struct PooledObjectData
	{
		public string Name;
		public int PoolAmount;
		public GameObject Prefab;
	}
}