using Utils;

namespace Character
{
	/// <summary>
	/// Used for when multiple resource types need to be stored.
	/// </summary>
	[System.Serializable]
	public class ResourceInventory
	{
		private int _maxAmount;
		private int _amount;
		private bool _unlimited = false;

		public bool Full => (Amount >= MaxAmount && !_unlimited);
		public bool HalfFull => (Amount >= MaxAmount * 0.5f && !_unlimited);
		public bool Empty => (Amount == 0 && !_unlimited);
		public string ResourceDataToString => _unlimited ? $"{StringUtils.GetShortenedNumberAsString(_amount)}" : $"{StringUtils.GetShortenedNumberAsString(_amount)}/{StringUtils.GetShortenedNumberAsString(_maxAmount)}";

		public int MaxAmount
		{
			get { return _maxAmount; }
			set
			{
				_maxAmount = value;
				OnMaxAmountChanged();
			}
		}

		public int Amount
		{
			get { return _amount; }
			set
			{
				_amount = value;
				OnAmountChanged();
			}
		}

		// Constructor
		public ResourceInventory(int startingAmount, int maxAmount, bool unlimited = false)
		{
			_amount = startingAmount;
			_maxAmount = maxAmount;
			_unlimited = unlimited;
		}

		/// <summary>
		/// Called when the resource amount has changed and ensures it is kept within the bounds of the storage amount.
		/// </summary>
		private void OnAmountChanged()
		{
			if ( _amount > _maxAmount && !_unlimited)
				_amount = _maxAmount;

			if (_amount < 0)
				_amount = 0;
		}

		/// <summary>
		/// Called when the max amount of a resource has changed to ensure it doesn't go into the negatives.
		/// </summary>
		private void OnMaxAmountChanged()
		{
			if (_maxAmount < 0)
				_maxAmount = 0;
		}
	}
}