using System;
using UnityEngine;
using Utils;

namespace GameResources
{
	/// <summary>
	/// Holds a specified resource and amount, for use on resource objects such as trees and ore.
	/// </summary>
	public class ResourceHolder : MonoBehaviour
	{
		[SerializeField]
		private Utils.Resource _resourceType;

		[SerializeField]
		private int _amount;

		[SerializeField]
		private bool _unlimited = false;

		[SerializeField]
		private bool _setByDistance = false;
		[SerializeField]
		private AnimationCurve _curve;
		[SerializeField]
		private int _minAmount;
		[SerializeField]
		private int _maxAmount;
		[SerializeField]
		private float _multiplierByDistanceSqr = 5;

		private object _ownerObject;

		private int _maxDistance = 150;

		public object OwnerObject => _ownerObject;
		public Resource ResourceType => _resourceType;
		public int Amount => _amount;
		public event Action<ResourceHolder> OnAmountChange;



		/// <summary>
		/// Removes resources from this source.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int TakeResource(int value)
		{
			if (_unlimited)
				return value;

			int taken = 0;

			if (_amount - value < 0)
				taken = _amount;
			else
				taken = value;

			_amount -= taken;

			if (_amount < 0)
				_amount = 0;

			OnAmountChanged();
			OnAmountChange?.Invoke(this);
			return taken;
		}

		/// <summary>
		/// Sets the remaining amount of resources.
		/// </summary>
		/// <param name="value"></param>
		public void SetResources(int value)
		{
			_amount = value;
		}

		/// <summary>
		/// Called when the resource amount has changed.
		/// </summary>
		private void OnAmountChanged()
		{
			if (_amount <= 0)
			{
				gameObject.SetActive(false);
			}
		}

		// Unity Functions.
		private void OnEnable()
		{
			if (_setByDistance)
			{
				var eval = _curve.Evaluate(transform.position.magnitude / (float)_maxDistance);
				var remap = MathExtended.RemapValue(eval, 0, 1, _minAmount, _maxAmount);
				_amount = (int)remap;
			}
		}
	}
}