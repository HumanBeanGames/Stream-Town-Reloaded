using System.Collections.Generic;
using Utils;

namespace Managers
{
	public static class TradeHandler
	{
		/// <summary>
		/// How much gold each resource sells for.
		/// </summary>
		public static Dictionary<Resource, float> ResourceSellRates = new Dictionary<Resource, float>()
		{
			{ Resource.Wood, 0.25f },
			{Resource.Ore, 0.25f },
			{Resource.Food, 0.25f }
		};

		public static float SellTaxRate = 0.5f;
		public static float BuyTaxRate = 0.6f;


		private static TownResourceManager _resourceManager;
		public static TownResourceManager ResourceManager
		{
			get
			{
				if (_resourceManager == null)
					_resourceManager = GameManager.Instance.TownResourceManager;

				return _resourceManager;
			}
		}

		/// <summary>
		/// Sells an amount of a resource for gold.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="amount"></param>
		/// <param name="message"></param>
		public static void SellResource(Resource resource, int amount, out string message)
		{
			message = "";

			int availableAmount = ResourceManager.CurrentResourceAmount(resource);

			if (amount <= 0)
			{
				message = $"No {resource} available to sell!";
				return;
			}

			if (amount > availableAmount)
				amount = availableAmount;

			ResourceManager.RemoveResource(resource, amount, true);

			int goldValue = (int)(amount * ResourceSellRates[resource]);
			goldValue -= (int)(goldValue * SellTaxRate);

			ResourceManager.AddResource(Resource.Gold, goldValue, true);
			EventManager.ResourceSold?.Invoke(resource, amount);
			message = $"Sold {amount} {resource} for {goldValue} gold.";
		}

		/// <summary>
		/// Buys an amount of a resource for gold.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="amount"></param>
		/// <param name="message"></param>
		public static void BuyResource(Resource resource, int amount, out string message)
		{
			message = "";

			int availableGold = ResourceManager.CurrentResourceAmount(Resource.Gold);
			int remainingStorageAmount = ResourceManager.MaxResourceAmount(resource) - ResourceManager.CurrentResourceAmount(resource);

			if (remainingStorageAmount <= 0)
			{
				message = "Storages are full, can't buy!";
				return;
			}

			if (remainingStorageAmount < amount)
				amount = remainingStorageAmount;

			float costPerResource = ResourceSellRates[resource] / BuyTaxRate;
			int costForAll = (int)(costPerResource * amount);

			if (costForAll > availableGold)
			{
				amount = (int)((float)availableGold / costPerResource);
				costForAll = (int)(costPerResource * amount);
			}

			ResourceManager.RemoveResource(Resource.Gold, costForAll, true);
			ResourceManager.AddResource(resource, amount, true);
			EventManager.ResourceBought?.Invoke(resource, amount);
			message = $"Bought {amount} {resource} for {costForAll} gold.";
		}
	}
}