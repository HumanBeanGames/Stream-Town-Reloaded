using Buildings;
using Character;
using GameResources;
using Managers;
using Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Twitch.Commands
{
	/// <summary>
	/// Handles all miscellaneous Twitch chat commands.
	/// </summary>
	public static class MiscCommands
	{
		public static readonly Dictionary<BuildingType, string> BuildingDescriptions = new Dictionary<BuildingType, string>
		{
			{ BuildingType.Barracks, "Barracks: Unlocks Soldier slots. "},
			{ BuildingType.Bowyard, "Bowyard: Unlocks Ranger slots. "},
			{ BuildingType.Castle, "Castle: Unlocks Paladin slots. "},
			{ BuildingType.Farm, "Farm: Unlocks Farmer slots. Can be farmed for Food resources. "},
			{ BuildingType.Fishinghut, "FishingHut: Unlocks Fisher slots. Must be placed on river's edge. "},
			{ BuildingType.Foodstorage, "FoodStorage: Increases Town's food storage. "},
			{ BuildingType.Forge, "Forge: Not yet available. " },
			{ BuildingType.Fountain, "Fountain: Town decoration. "},
			{ BuildingType.Gate, "Gate: Players can walk through them but enemies can't. "},
			{ BuildingType.House, "House: Unlocks Recruit slots (NPC characters). "},
			{ BuildingType.Lumbermill, "Lumbermill: Unlocks Logger slots. "},
			{ BuildingType.Marketplace, "Marketplace: Generates Gold income over time. "},
			{ BuildingType.Monastery, "Monastery: Unlocks Priest slots. "},
			{ BuildingType.Necrotower, "NecroTower: Unlocks Necromancer slots. "},
			{ BuildingType.Orestorage, "OreStorage: Increases Town's ore storage. "},
			{ BuildingType.Statue1, "Statue1: Town decoration. "},
			{ BuildingType.Statue2, "Statue2: Town decoration. "},
			{ BuildingType.Statue3, "Statue3: Town decoration. "},
			{ BuildingType.Stonemason, "Stonemason: Unlocks Miner slots. "},
			{ BuildingType.Torch, "Torch: Illuminates Town at night. "},
			{ BuildingType.Tower, "Tower: Unlocks Defender slots. Fires projectiles at enemies. "},
			{ BuildingType.Townhall, "TownHall: The spawn point. Type !stuck to return here. "},
			{ BuildingType.Wall, "Wall: Provides defense from enemies. "},
			{ BuildingType.Windmill, "Windmill: Unlocks Gatherer slots. "},
			{ BuildingType.Wizardtower, "WizardTower: Unlocks Wizard slots. "},
			{ BuildingType.Woodstorage, "WoodStorage: Increases Town's wood storage. "}
		};

		public static readonly Dictionary<PlayerRole, string> PlayerRoleDescriptions = new Dictionary<PlayerRole, string>
		{
			//{ PlayerRole.Blacksmith, "Insert Description Here"},
			{ PlayerRole.Builder, "Builder: Makes buildings. "},
			{ PlayerRole.Defender, "Defender: Basic melee combat unit. "},
			{ PlayerRole.Farmer, "Farmer: Collects Food from Farms. "},
			{ PlayerRole.Fisher, "Fisher: Collects Food from rivers. "},
			{ PlayerRole.Gatherer, "Gatherer: Collects Food from bushes. "},
			{ PlayerRole.Logger, "Logger: Collects Wood from trees. "},
			{ PlayerRole.Miner, "Miner: Collects Ore from rocks. "},
			{ PlayerRole.Necromancer, "Necromancer: Ranged combat unit. "},
			{ PlayerRole.Paladin, "Paladin: Melee combat and healing unit. "},
			{ PlayerRole.Priest, "Priest: Healing unit. "},
			{ PlayerRole.Ranger, "Ranger: Ranged combat unit. "},
			{ PlayerRole.Ruler, "Ruler: Controls the camera, recruits NPCs, buys and sells resources and fights enemies. "},
			{ PlayerRole.Soldier, "Soldier: Strong melee combat unit. "},
			{ PlayerRole.Wizard, "Wizard: Ranged combat unit. "}
		};

		public static readonly Dictionary<Resource, string> ResourceDescriptions = new Dictionary<Resource, string>
		{
			{ Resource.Gold, "Gold: Required to make and upgrade buildings. Enemies will drop Gold when they die. The Ruler can buy and sell other resources with Gold. "},
			{ Resource.Wood, "Wood: Required to make and upgrade buildings. Loggers collect Wood from trees. "},
			{ Resource.Food, "Food: Required to make and upgrade buildings. Gatherers, Farmers and Fishers collect Food from bushes, Farms and rivers, respectively. "},
			{ Resource.Ore, "Ore: Required to make and upgrade buildings. Miners collect Ore from rocks. "}

		};

		public static readonly Dictionary<EnemyType, string> EnemyDescriptions = new Dictionary<EnemyType, string>
		{
			{ EnemyType.Blargul, "Blargul: Ranged combat unit. "},
			{ EnemyType.Goblin, "Goblin: Basic melee combat unit. "},
			{ EnemyType.GoblinBoss, "Goblin Boss: Strong melee combat unit. "},
			{ EnemyType.NecroSlasher, "Necro Slasher: Strong melee combat unit. "},
			{ EnemyType.NecroStalker, "Necro Stalker: Strong melee combat unit. "},
			{ EnemyType.Skeleton, "Skeleton: Strong melee combat unit. "},
			{ EnemyType.MinotaurBoss, "Minotaur Boss: Strong melee combat unit. "},
			{ EnemyType.BatteringRam, "Battering Ram: Only attacks buildings. "}
		};

		/// <summary>
		/// Sends the Stream Town Discord link in chat.
		/// </summary>
		public static void Discord()
		{
			MessageSender.SendPreBuiltMessage("discord");
		}

		/// <summary>
		/// Sends a basic Help message to chat.
		/// </summary>
		public static void Help()
		{
			MessageSender.SendPreBuiltMessage("help");
		}

		/// <summary>
		/// Sends a brief list of important Town Statistics to chat.
		/// </summary>
		public static void TownStats()
		{
			//TODO:: Implement
		}

		private static string GetResourceInfo(Resource type, string[] args)
		{
			return ResourceDescriptions[type];
		}

		private static string GetPlayerRoleInfo(PlayerRole role, string[] args)
		{
			return PlayerRoleDescriptions[role];
		}

		public static void Level(Player player, string command, params string[] args)
		{
			if (Enum.TryParse(args[0], true, out PlayerRole role))
				RoleCommands.ExperienceForRole(player, role);

			else if (Enum.TryParse(args[0], true, out BuildingType type))
			{
				if (args.Length < 2)
				{
					MessageSender.SendMessage($"{player.TwitchUser.Username} no enough arguments to level up building (!level <BuildingName> <Id> +- <amount>)");
					return;
				}
				int iterations = 1;
				if (int.TryParse(args[1], out int id))
				{
					if (args.Length >= 3)
						int.TryParse(args[2], out iterations);

					BuildingCommands.LevelBuilding(player, type, id, iterations);
				}
				else
					MessageSender.SendMessage($"{player.TwitchUser.Username} the id {id} is not a valid id");
			}
		}


		public static string GetBuildingCost(BuildingManager manager, BuildingType building)
		{
			BuildingDataScriptable buildData = BuildingManager.GetBuildingData(building);

			ResourceCostData resourceCost = buildData.BuildResourceCost;
			int woodCost = resourceCost.WoodCost + (int)((float)(resourceCost.WoodCost * manager.BuildingCounts[building]) * buildData.CostIncreasePerBuildingMultiplier);
			int oreCost = resourceCost.OreCost + (int)((float)(resourceCost.OreCost * manager.BuildingCounts[building]) * buildData.CostIncreasePerBuildingMultiplier);
			int foodCost = resourceCost.FoodCost + (int)((float)(resourceCost.FoodCost * manager.BuildingCounts[building]) * buildData.CostIncreasePerBuildingMultiplier);
			int goldCost = resourceCost.GoldCost + (int)((float)(resourceCost.GoldCost * manager.BuildingCounts[building]) * buildData.CostIncreasePerBuildingMultiplier);
			woodCost -= manager.CalculateCostReduction(building, woodCost);
			oreCost -= manager.CalculateCostReduction(building, oreCost);
			foodCost -= manager.CalculateCostReduction(building, foodCost);
			goldCost -= manager.CalculateCostReduction(building, goldCost);
			int maxLevel = manager.BuildingsMaxLevel[building];

			string description = BuildingDescriptions[building];
			return $" Cost: Wood: {woodCost} | Ore: {oreCost} | Food: {foodCost} | Gold: {goldCost} | Max Level: {maxLevel}";
		}



		public static string GetBuildingInfo(BuildingType building, string[] args)
		{
			BuildingManager manager = GameManager.Instance.BuildingManager;

			// Gets information for the building type
			if (args.Count() == 1 || building == BuildingType.Townhall)
				return $" {BuildingDescriptions[building]}";

			// Gets information for the specified building eith index of type
			else if (int.TryParse(args[1], out int id))
			{
				int index = id - 1;
				if (manager.TryGetBuilding(building, index, out BuildingBase buildingBase, out string errorMessage))
				{
					string extraInfo = "";

					if (building == BuildingType.Marketplace)
						extraInfo = buildingBase.GetComponent<PassiveResourceIncrementer>().GetInformation();

					return $" {building} {id}   |" +
							$"{extraInfo}" +
							$"Health: {buildingBase.HealthHandler.Health} / {buildingBase.HealthHandler.MaxHealth} | " +
							$" Level: {buildingBase.LevelHandler.Level} / {buildingBase.LevelHandler.MaxLevel} | " +
							$"Can level up: {buildingBase.LevelHandler.CanLevel()}";
				}
				else
					return $"Error: Can't find building {building} with id '{id}'";
			}
			else
				return $"Error: cant find the building of type {building}";
		}

		public static string GetEnemyInfo(EnemyType type, string[] args)
		{
			return EnemyDescriptions[type];
		}

		public static void ItemInfo(Player player, string command, params string[] args)
		{
			if (args.Length >= 1)
			{
				string itemName = char.ToUpper(args[0][0]) + args[0].Substring(1);

				if (Enum.TryParse(itemName, out Resource resourceType))
					MessageSender.SendMessage(GetResourceInfo(resourceType, args));

				else if (Enum.TryParse(itemName, out PlayerRole role))
					MessageSender.SendMessage(GetPlayerRoleInfo(role, args));

				else if (Enum.TryParse(itemName, out BuildingType building))
					MessageSender.SendMessage(GetBuildingInfo(building, args));

				else if (Enum.TryParse(itemName, out EnemyType enemy))
					MessageSender.SendMessage(GetEnemyInfo(enemy, args));

				else
					MessageSender.SendMessage(player.TwitchUser.Username + " " + itemName + " Is not a valid argument for !info");
			}
		}
	}
}
