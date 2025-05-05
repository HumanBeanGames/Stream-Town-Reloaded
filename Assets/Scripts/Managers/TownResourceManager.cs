using Character;
using GameResources;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using UnityEngine.Events;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor;
using Events;

namespace Managers
{
	/// <summary>
	/// Managers all the resources for the town.
	/// </summary>
	[GameManager]
	public static class TownResourceManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static TownResourceConfig Config = TownResourceConfig.Instance;

        /// <summary>
        /// How long to store the value of a resource at a given time, used in rate of change.
        /// </summary>
        public static float RESOURCE_RATE_TIME_PERIOD => Config.RESOURCE_RATE_TIME_PERIOD;

        /// <summary>
        /// How frequenly should the rate of change be updated.
        /// </summary>
        public static float RESOURCE_UPDATE_RATE=> Config.RESOURCE_UPDATE_RATE;

        /// <summary>
        /// Current resources in the town, including max amounts.
        /// </summary>
        [SerializeField]
		private static Dictionary<Resource, ResourceInventory> _resources => Config.resources;

        [SerializeField]
        /// <summary>
        /// Holds all rate of change data for all town resources.
        /// </summary>
        private static Dictionary<Resource, ResourceRateOfChange> _resourceRatesOfChange => Config.resourcesRateOfChange;

		/// <summary>
		/// A lookup table for each resource that get invoked when that resource amount has changed.
		/// </summary>
		private static Dictionary<Resource, StorageStatusEventSO> _onResourceChangeEventDict => Config.onResourceChangeEventDict;

		/// <summary>
		/// Called when any resource amount has changed.
		/// </summary>
		[HideInInspector]
		private static UnityEvent<Resource, int, bool> _onAnyResourceChangeEvent = new UnityEvent<Resource, int, bool>();

		public static UnityEvent<Resource, int, bool> OnAnyResourceChangeEvent => _onAnyResourceChangeEvent;

		public static Dictionary<Resource, int> ResourceBoostValues => Config.resourceBoostValues;

		/// <summary>
		/// Returns a reference to the event called when a specific type of resource count changes.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static UnityEvent<StorageStatus> GetResourceChangeEvent(Resource type)
		{
			return _onResourceChangeEventDict[type];
		}

		/// <summary>
		/// Returns the amount of resources for a specified type
		/// </summary>
		/// <param name="resourceType"></param>
		/// <returns></returns>
		public static int GetResourceAmount(Resource resourceType)
		{
			return _resources[resourceType].Amount;
		}

		/// <summary>
		/// Sets the amoiunt of resources for a specified type
		/// </summary>
		/// <param name="resourceType"></param>
		/// <param name="resourceAmount"></param>
		public static void SetResourceAmount(Resource resourceType, int resourceAmount)
		{
			_resources[resourceType].Amount = resourceAmount;
		}

		/// <summary>
		/// Adds the specific type of resource to the storage.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		public static void AddResource(Resource type, int amount, bool purchase = false)
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
		public static void RemoveResource(Resource type, int amount, bool purchase = false)
		{
			_resources[type].Amount -= amount;
			ResourceChanged(type, -amount, purchase);
		}

		/// <summary>
		/// Returns true if a specific type of resource is full.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool ResourceFull(Resource type)
		{
			return _resources[type].Full;
		}

		/// <summary>
		/// Returns true if the current storage amount of a resource is equal to or greater than the value passed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static bool MoreThanEqualComparison(Resource type, int amount)
		{
			return _resources[type].Amount >= amount;
		}

		/// <summary>
		/// Increases the storage amount of a specific type of resource.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public static void IncreaseStorage(Resource type, int amount)
		{
			_resources[type].MaxAmount += amount;
			ResourceChanged(type, amount,true);
		}

		/// <summary>
		/// Reduces the storage amount of a specific type of resource.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public static void ReduceStorage(Resource type, int amount)
		{
			_resources[type].MaxAmount -= amount;
			ResourceChanged(type, -amount,true);
		}

		/// <summary>
		/// Returns the status of a specific type of resource, determining if it is full, half full or empty.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static StorageStatus GetResourceStatus(Resource type)
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
		public static string ResourcePrint(Resource type)
		{
			return _resources[type].ResourceDataToString;
		}

		public static int CurrentResourceAmount(Resource type)
		{
			return _resources[type].Amount;
		}

		public static int MaxResourceAmount(Resource type)
		{
			return _resources[type].MaxAmount;
		}

		/// <summary>
		/// Returns the current rate of change of a specific resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static int RateOfChangeForResource(Resource resource)
		{
			return _resourceRatesOfChange[resource].AverageOverTime;
		}

		/// <summary>
		/// Called when a specific type of resource has changed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		private static void ResourceChanged(Resource type, int amount, bool purchase = false)
		{
			_onAnyResourceChangeEvent?.Invoke(type, amount, purchase);
			((UnityEvent<StorageStatus>)_onResourceChangeEventDict[type]).Invoke(GetResourceStatus(type));
		}


        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Initialize()
		{
		    GameObject runnerObject = new GameObject("TownResourcesManagerRunner");
            runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            runner.StartCoroutine(ProcessResources());
        }

		public static bool TryTakeReviveCost(Utils.ReviveType type)
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

		private static IEnumerator ProcessResources()
		{
			while (true)
			{
				foreach (var r in _resourceRatesOfChange)
				{
					r.Value.ProcessQueue();
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}
}