using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Handles the number of slots available for a role.
	/// </summary>
	public class RoleSlot
	{
		[SerializeField]
		private PlayerRole _playerRole;

		[SerializeField]
		private bool _infinite;

		[SerializeField]
		private int _maxSlots;

		[SerializeField]
		private int _slotsTaken;

		public bool Available => _slotsTaken < _maxSlots || _infinite;
		public bool Full => _slotsTaken >= _maxSlots && !_infinite;

		public int SlotsTaken => _slotsTaken;
		public int MaxSlots => _maxSlots;

		public bool Infinite => _infinite;

		public string SlotDataAsString => _infinite ? $"{_slotsTaken}" : $"{_slotsTaken}   /   {_maxSlots}";

		// Constructor.
		public RoleSlot(PlayerRole role, int maxSlots, bool infinite)
		{
			_playerRole = role;
			_maxSlots = maxSlots;
			_slotsTaken = 0;
			_infinite = infinite;
		}

		/// <summary>
		/// Increments the number of slots taken for the role.
		/// </summary>
		public void OnSlotTaken()
		{
			_slotsTaken++;
		}

		/// <summary>
		/// Decrements the number of slots taken for the role.
		/// </summary>
		public void OnSlotRemoved()
		{
			_slotsTaken--;
		}

		/// <summary>
		/// Increases the max number of slots available for the role.
		/// </summary>
		/// <param name="amount"></param>
		public void IncreaseMaxSlots(int amount)
		{
			_maxSlots += amount;
		}

		/// <summary>
		/// Reduces the max number of slots available for the role.
		/// </summary>
		/// <param name="amount"></param>
		public void DecreaseMaxSlots(int amount)
		{
			_maxSlots -= amount;
			if (_maxSlots < 0)
				Debug.LogError($"Max slots for {_playerRole} went below 0, this should not happen!");
		}

		/// <summary>
		/// Sets the maximum number of slots for the role.
		/// </summary>
		/// <param name="maxAmount"></param>
		public void SetMaxSlots(int maxAmount)
		{
			_maxSlots = maxAmount;
		}
	}
}