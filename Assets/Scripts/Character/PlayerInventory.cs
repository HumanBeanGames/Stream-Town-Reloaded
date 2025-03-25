using GameResources;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Handles the inventory of a player character.
	/// </summary>
	[System.Serializable]
	public class PlayerInventory : MonoBehaviour, IResourceHolder
	{
		/// <summary>
		/// Resource Table that holds the current resources the player has.
		/// </summary>
		private Dictionary<Resource, ResourceInventory> _resources = new Dictionary<Resource, ResourceInventory>();
		public Dictionary<Resource, ResourceInventory> Resources => _resources;

		/// <summary>
		/// Sets the entire resource dictionary,
		/// </summary>
		/// <param name="resources">The dictionary to be set to</param>
		public void SetResources(Dictionary<Resource, ResourceInventory> resources)
		{
			_resources = resources;
		}


		/// <summary>
		/// Adds a given resource and amount to the player's inventory.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void AddResource(Utils.Resource type, int amount)
		{
			_resources[type].Amount += amount;
		}

		/// <summary>
		/// Removes a given resource and amount from the player's inventory.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void RemoveResource(Utils.Resource type, int amount)
		{
			_resources[type].Amount -= amount;
		}

		/// <summary>
		/// Returns true if the player is full on a specific type of resource.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool ResourceFull(Utils.Resource type)
		{
			if (!_resources.ContainsKey(type))
				return false;

			return _resources[type].Full;
		}

		/// <summary>
		/// Returns the amount of a specific resource the player has.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int ResourceCount(Utils.Resource type)
		{
			return _resources[type].Amount;
		}

		/// <summary>
		/// Sets the max amount of a specific resource that the player can hold.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void SetMaxStorage(Utils.Resource type, int amount)
		{
			InitData();
			_resources[type].MaxAmount = amount;
		}

		/// <summary>
		/// Initalizes all inventory data
		/// </summary>
		private void InitData()
		{
			if (_resources.Count == 0)
			{
				_resources.Add(Utils.Resource.Food, new ResourceInventory(0, 10));
				_resources.Add(Utils.Resource.Ore, new ResourceInventory(0, 10));
				_resources.Add(Utils.Resource.Wood, new ResourceInventory(0, 10));
                _resources.Add(Utils.Resource.SaplingCounter, new ResourceInventory(0, 10));
            }
		}

		// Unity Functions.
		private void Awake()
		{
			// TODO: Set starting amounts to config
			InitData();
		}
	}
}