
using System;
using UnityEngine;
using Utils;

namespace GameResources
{
	/// <summary>
	/// Passively increments a town resource over time.
	/// </summary>
	public class PassiveResourceIncrementer : MonoBehaviour
	{
		[SerializeField]
		protected Utils.Resource _resource;
		[SerializeField]
		protected float _amountPerSecond;
		[SerializeField]
		protected float _amountPerLevel;

		protected float _totalAmount;
		protected bool _enabled = false;
		protected float _accumulated = 0;

		protected TownResourceManager _resourceManager;
		public event Action<PassiveResourceIncrementer> OnRateChange;

		/// <summary>
		/// Called when a containing level handler has leveled up.
		/// </summary>
		public void OnLevelUp()
		{
			_totalAmount += _amountPerLevel;
			OnRateChange?.Invoke(this);
		}

		/// <summary>
		/// Creates a string used for displaying information
		/// </summary>
		/// <returns></returns>
		public string GetInformation()
		{
			float amountPerHour = _totalAmount * 60 * 60;
			return $"Rate +{StringUtils.GetShortenedNumberAsString((int)amountPerHour)} {_resource}/HR ";
		}

		/// <summary>
		/// Enables the passive resource income.
		/// </summary>
		public void Enable()
		{
			_enabled = true;
			_totalAmount = _amountPerSecond;
		}

		/// <summary>
		/// Disables the passive resource income.
		/// </summary>
		public void Disable()
		{
			_enabled = false;
			_totalAmount = _amountPerSecond;
		}

		// Unity Functions.
		private void Awake()
		{
			_resourceManager = GameManager.Instance.TownResourceManager;
		}

		private void OnDisable()
		{
			Disable();
		}

		private void Update()
		{
			if (!_enabled)
				return;

			_accumulated += _totalAmount * Time.deltaTime;

			if (_accumulated > 1)
			{
				int rounded = Mathf.FloorToInt(_accumulated);
				_accumulated -= rounded;

				_resourceManager.AddResource(_resource, rounded);
			}
		}
	}
}