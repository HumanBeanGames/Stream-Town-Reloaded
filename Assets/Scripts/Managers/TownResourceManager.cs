using Character;
using GameResources;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using UnityEngine.Events;
using System;

namespace Managers
{
	/// <summary>
	/// Managers all the resources for the town.
	/// </summary>
	public class TownResourceManager : MonoBehaviour
	{
		/// <summary>
		/// How long to store the value of a resource at a given time, used in rate of change.
		/// </summary>
		public const float RESOURCE_RATE_TIME_PERIOD = 25;

		/// <summary>
		/// How frequenly should the rate of change be updated.
		/// </summary>
		public const float RESOURCE_UPDATE_RATE = 1;

		/// <summary>
		/// Current resources in the town, including max amounts.
		/// </summary>
		[SerializeField]
		private Dictionary<Resource, ResourceInventory> _resources = new Dictionary<Resource, ResourceInventory>();

		/// <summary>
		/// A lookup table for each resource that get invoked when that resource amount has changed.
		/// </summary>
		private Dictionary<Resource, UnityEvent<StorageStatus>> _onResourceChangeEventDict = new Dictionary<Resource, UnityEvent<StorageStatus>>();

		/// <summary>
		/// Called when any resource amount has changed.
		/// </summary>
		private UnityEvent<Resource, int, bool> _onAnyResourceChangeEvent = new UnityEvent<Resource, int, bool>();

		public UnityEvent<Resource, int, bool> OnAnyResourceChangeEvent => _onAnyResourceChangeEvent;
		/// <summary>
		/// Holds all rate of change data for all town resources.
		/// </summary>
		private Dictionary<Resource, ResourceRateOfChange> _resourceRatesOfChange = new Dictionary<Resource, ResourceRateOfChange>();



		public Dictionary<Resource, int> ResourceBoostValues = new Dictionary<Resource, int>();

		/// <summary>
		/// Returns a reference to the event called when a specific type of resource count changes.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public UnityEvent<StorageStatus> GetResourceChangeEvent(Resource type)
		{
			return _onResourceChangeEventDict[type];
		}

		/// <summary>
		/// Returns the amount of resources for a specified type
		/// </summary>
		/// <param name="resourceType"></param>
		/// <returns></returns>
		public int GetResourceAmount(Resource resourceType)
		{
			return _resources[resourceType].Amount;
		}

		/// <summary>
		/// Sets the amoiunt of resources for a specified type
		/// </summary>
		/// <param name="resourceType"></param>
		/// <param name="resourceAmount"></param>
		public void SetResourceAmount(Resource resourceType, int resourceAmount)
		{
			_resources[resourceType].Amount = resourceAmount;
		}

		/// <summary>
		/// Adds the specific type of resource to the storage.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		public void AddResource(Resource type, int amount, bool purchase = false)
		{
			_resources[type].Amount += amount;
			ResourceChanged(type, amount, purchase);

			if (!purchase && amount > 0)
				EventManager.ResourceGained?.Invoke(type, amount);
		}

		/// <summary>
		/// Removes the specific type of resource from the storage.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		public void RemoveResource(Resource type, int amount, bool purchase = false)
		{
			_resources[type].Amount -= amount;
			ResourceChanged(type, -amount, purchase);
		}

		/// <summary>
		/// Returns true if a specific type of resource is full.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool ResourceFull(Resource type)
		{
			return _resources[type].Full;
		}

		/// <summary>
		/// Returns true if the current storage amount of a resource is equal to or greater than the value passed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool MoreThanEqualComparison(Resource type, int amount)
		{
			return _resources[type].Amount >= amount;
		}

		/// <summary>
		/// Increases the storage amount of a specific type of resource.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void IncreaseStorage(Resource type, int amount)
		{
			_resources[type].MaxAmount += amount;
			ResourceChanged(type, amount,true);
		}

		/// <summary>
		/// Reduces the storage amount of a specific type of resource.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void ReduceStorage(Resource type, int amount)
		{
			_resources[type].MaxAmount -= amount;
			ResourceChanged(type, -amount,true);
		}

		/// <summary>
		/// Returns the status of a specific type of resource, determining if it is full, half full or empty.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public StorageStatus GetResourceStatus(Resource type)
		{
			StorageStatus storageStatus = StorageStatus.Empty;

			if (_resources[type].Full)
				storageStatus = StorageStatus.Full;
			else if (_resources[type].HalfFull)
				storageStatus = StorageStatus.HalfFull;

			return storageStatus;
		}

		/// <summary>
		/// Returns a printable string of the resource.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string ResourcePrint(Resource type)
		{
			return _resources[type].ResourceDataToString;
		}

		public int CurrentResourceAmount(Resource type)
		{
			return _resources[type].Amount;
		}

		public int MaxResourceAmount(Resource type)
		{
			return _resources[type].MaxAmount;
		}

		/// <summary>
		/// Returns the current rate of change of a specific resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public int RateOfChangeForResource(Resource resource)
		{
			return _resourceRatesOfChange[resource].AverageOverTime;
		}

		/// <summary>
		/// Called when a specific type of resource has changed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		private void ResourceChanged(Resource type, int amount, bool purchase = false)
		{
			_onAnyResourceChangeEvent?.Invoke(type, amount, purchase);
			_onResourceChangeEventDict[type].Invoke(GetResourceStatus(type));
		}

		// Unity Events.
		private void Awake()
		{
			// TODO: Set starting amounts to config
			_resources.Add(Resource.Food, new ResourceInventory(5000, 15000));
			_resources.Add(Resource.Ore, new ResourceInventory(5000, 15000));
			_resources.Add(Resource.Wood, new ResourceInventory(5000, 15000));
			_resources.Add(Resource.Gold, new ResourceInventory(5000, 0, true));
			_resources.Add(Resource.Recruit, new ResourceInventory(0, 5));

			_resourceRatesOfChange.Add(Resource.Food, new ResourceRateOfChange(Resource.Food, RESOURCE_RATE_TIME_PERIOD, RESOURCE_UPDATE_RATE, this));
			_resourceRatesOfChange.Add(Resource.Ore, new ResourceRateOfChange(Resource.Ore, RESOURCE_RATE_TIME_PERIOD, RESOURCE_UPDATE_RATE, this));
			_resourceRatesOfChange.Add(Resource.Wood, new ResourceRateOfChange(Resource.Wood, RESOURCE_RATE_TIME_PERIOD, RESOURCE_UPDATE_RATE, this));
			_resourceRatesOfChange.Add(Resource.Gold, new ResourceRateOfChange(Resource.Gold, RESOURCE_RATE_TIME_PERIOD, RESOURCE_UPDATE_RATE, this));
			_resourceRatesOfChange.Add(Resource.Recruit, new ResourceRateOfChange(Resource.Recruit, RESOURCE_RATE_TIME_PERIOD, RESOURCE_UPDATE_RATE, this));

			_onResourceChangeEventDict.Add(Resource.Food, new UnityEvent<StorageStatus>());
			_onResourceChangeEventDict.Add(Resource.Ore, new UnityEvent<StorageStatus>());
			_onResourceChangeEventDict.Add(Resource.Wood, new UnityEvent<StorageStatus>());
			_onResourceChangeEventDict.Add(Resource.Gold, new UnityEvent<StorageStatus>());
			_onResourceChangeEventDict.Add(Resource.Recruit, new UnityEvent<StorageStatus>());

			ResourceBoostValues.Add(Resource.Food, 0);
			ResourceBoostValues.Add(Resource.Ore, 0);
			ResourceBoostValues.Add(Resource.Wood, 0);
			ResourceBoostValues.Add(Resource.Recruit, 0);
		}

		public bool TryTakeReviveCost(Utils.ReviveType type)
		{
			if(type == ReviveType.Others && CurrentResourceAmount(Resource.Food) >= 200)
			{
				RemoveResource(Resource.Food, 200);
				return true;
			}
			else if(type == ReviveType.Self && CurrentResourceAmount(Resource.Food) >= 400)
			{
				RemoveResource(Resource.Food, 400);
				return true;
			}
			else return false;
		}

		private void Update()
		{
			foreach (var r in _resourceRatesOfChange)
			{
				r.Value.ProcessQueue();
			}
		}
	}
}