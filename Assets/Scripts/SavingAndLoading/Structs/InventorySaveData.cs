using Character;
using System.Collections.Generic;
using Utils;

namespace SavingAndLoading.Structs
{
	/// <summary>
	/// Struct for holding inventory data
	/// </summary>
	[System.Serializable]
	public struct InventorySaveData
	{
		List<Resource> Resources;
		List<ResourceInventory> Inventory;

		/// <summary>
		/// Overloaded constructor,
		/// Converts inventory dictionary to INventorySaveData
		/// </summary>
		/// <param name="dictionary">The current dictionary</param>
		public InventorySaveData(Dictionary<Resource, ResourceInventory> dictionary)
		{
			Resources = new List<Resource>();
			Inventory = new List<ResourceInventory>();

			for (int i = 0; i < (int)Resource.Count; i++)
			{
				ResourceInventory temp;
				if (dictionary.TryGetValue((Resource)i, out temp))
				{
					Resources.Add((Resource)i);
					Inventory.Add(temp);
				}
			}
		}

		/// <summary>
		/// Converts InventorySaveData to dictionary
		/// </summary>
		/// <returns>A dictionary</returns>
		public Dictionary<Resource, ResourceInventory> ToDictionary()
		{
			Dictionary<Resource,ResourceInventory> dic = new Dictionary<Resource,ResourceInventory>();
			for(int i = 0; i < Resources.Count; i++)
			{
				dic.Add(Resources[i], Inventory[i]);
			}

			return dic;
		}
	}
}