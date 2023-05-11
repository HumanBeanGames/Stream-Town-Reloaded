namespace SavingAndLoading.Structs
{
	[System.Serializable]
	public struct ObjectiveSaveData
	{
		public int RequiredAmount;
		public int Amount;

		public ObjectiveSaveData(int amount, int requiredAmount)
		{
			Amount = amount;
			RequiredAmount = requiredAmount;
		}
	}
}