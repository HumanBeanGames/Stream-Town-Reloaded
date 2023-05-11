using DataStructures;
using System.Collections.Generic;

namespace TechTree.Utilities
{
	public static class CollectionUtility
	{
		public static void AddItem<K, V>(this SerializableDictionary<K, List<V>> serializableDictionary, K key, V Value)
		{
			if (serializableDictionary.ContainsKey(key))
			{
				serializableDictionary[key].Add(Value);

				return;
			}

			serializableDictionary.Add(key, new List<V>() { Value });
		}
	}
}