using Level;
using Managers;
using UnityEngine;
using Utils;

namespace Buildings
{
	/// <summary>
	/// Component that modifies the town's resource storage
	/// </summary>
	public class ResourceStorageModifier : MonoBehaviour
	{
		/// <summary>
		/// Which resource type this component modifies.
		/// </summary>
		[SerializeField, Tooltip("Which resource type this component modifies.")]
		private Resource _resource;

		/// <summary>
		/// How much this component expands the resource when spawned or constructed.
		/// </summary>
		[SerializeField, Tooltip("How much it expands the storage on spawn/construction.")]
		private int _baseAmount;

		/// <summary>
		/// How much the storage expands when incremented.
		/// </summary>
		[SerializeField, Tooltip("How much it expands the storage per increment.")]
		private int _incrementAmount;

		/// <summary>
		/// How much the increment gets multiplied per increment.
		/// </summary>
		[SerializeField, Tooltip("How much the increment amount gets increased per increment")]
		private float _incrementMultiPerLevel = 4;

		/// <summary>
		/// Total amount of storage this component has added.
		/// </summary>
		private int _totalAmount = 0;

		private LevelHandler _levelHandler;
		// Properties.
		public Resource ResourceType => _resource;

		/// <summary>
		/// Increments the amount of storage based on set parameters.
		/// </summary>
		public void Increment()
		{
			RecalculateStorageAmount();
		}

		/// <summary>
		/// Adds the base amount of storage to the resource manager.
		/// </summary>
		public void AddBaseStorage()
		{
			//int amount = _baseAmount;
			//amount += (int)(amount * (TownResourceManager.ResourceBoostValues[_resource] / 100.0f));
			//TownResourceManager.IncreaseStorage(_resource, amount);
			//_totalAmount += amount;
			RecalculateStorageAmount();
		}

		/// <summary>
		/// Removes the total amount of storage that this component contributed.
		/// </summary>
		public void RemoveTotalStorage()
		{
			TownResourceManager.ReduceStorage(_resource, _totalAmount);
			_totalAmount = 0;
		}

		private void RecalculateStorageAmount()
		{
			if (_levelHandler == null)
				_levelHandler = GetComponent<LevelHandler>();

			int amount = _levelHandler.Level <= 1 ? _baseAmount : _incrementAmount * (int)(_levelHandler.Level * _incrementMultiPerLevel);
			amount += (int)(amount * (TownResourceManager.ResourceBoostValues[_resource] / 100.0f));
			RemoveTotalStorage();
			TownResourceManager.IncreaseStorage(_resource, amount);
			_totalAmount = amount;
		}

		private void OnResourceStorageIncreased(Resource type)
		{
			if (type != _resource)
				return;

			RecalculateStorageAmount();
		}

		private void Start()
		{
			_levelHandler = GetComponent<LevelHandler>();
		}

		/// <summary>
		/// Called when object is disabled.
		/// </summary>
		private void OnDisable()
		{
			RemoveTotalStorage();

			GameManager.Instance.TechTreeManager.OnStorageBoostUnlocked -= OnResourceStorageIncreased;
		}

		private void OnEnable()
		{
			GameManager.Instance.TechTreeManager.OnStorageBoostUnlocked += OnResourceStorageIncreased;
		}
	}
}