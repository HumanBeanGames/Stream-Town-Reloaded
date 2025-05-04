using Managers;
using UnityEngine;
using Utils;

namespace Buildings
{
	/// <summary>
	/// A component that modifies the amount of role slots available for a given role.
	/// </summary>
	public class RoleSlotModifier : MonoBehaviour
	{
		/// <summary>
		/// Which role type this component modifies.
		/// </summary>
		[SerializeField, Tooltip("Which role type this component modifies.")]
		private PlayerRole _role;

		/// <summary>
		/// The base amount this component increases role slots by.
		/// </summary>
		[SerializeField, Tooltip("The base amount this component increases role slots by.")]
		private int _baseAmount;

		/// <summary>
		/// How much this component increases the amount of role slots per increment.
		/// </summary>
		[SerializeField, Tooltip("How much this component increases the amount of role slots per increment.")]
		private int _incrementAmount;

		/// <summary>
		/// Keeps track of the total number of role slots this component has added.
		/// </summary>
		private int _totalAmount = 0;

		// Required Components.
		private RoleManager _roleManager;

		public RoleManager RoleManager
		{
			get
			{
				if (_roleManager == null)
					_roleManager = GameManager.Instance.RoleManager;
				return _roleManager;
			}
		}

		// Properties.
		public PlayerRole Role => _role;

		/// <summary>
		/// Increments the amount of role slots available.
		/// </summary>
		public void Increment()
		{
			RoleManager.AddSlots(_role, _incrementAmount);
			_totalAmount += _incrementAmount;
		}

		/// <summary>
		/// Adds base number of role slots.
		/// </summary>
		public void AddBaseSlots()
		{
			RoleManager.AddSlots(_role, _baseAmount);
			_totalAmount += _baseAmount;
		}

		/// <summary>
		/// Removes the total number of role slots contributed.
		/// </summary>
		public void RemoveTotalSlots()
		{
			_roleManager.RemoveSlots(_role, _totalAmount);
			_totalAmount = 0;
		}

		/// <summary>
		/// Called on object being disabled.
		/// </summary>
		private void OnDisable()
		{
			if (RoleManager)
				RemoveTotalSlots();
		}
	}
}