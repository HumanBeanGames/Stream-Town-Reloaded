using Buildings;
using Character;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Twitch.Commands
{
	/// <summary>
	/// Handles all Twitch Chat Commands related to Building.
	/// </summary>
	public static class BuildingCommands
	{
		/// <summary>
		/// Returns the type of Building from the given argument.
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static string GetBuildingTypeFromArg(string arg)
		{
			return char.ToUpper(arg[0]) + arg.Substring(1);
		}

		/// <summary>
		/// Attempts to start a building with type given in the argument.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void StartBuild(Player player, string command, params string[] args)
		{
			string buildType = GetBuildingTypeFromArg(args[0]);

			if (Enum.TryParse(buildType, out BuildingType type))
			{
				if (!GameManager.Instance.BuildingManager.TryStartNewBuildingPlacer(player, type, out string errorMessage))
					MessageSender.SendMessage($"{player.TwitchUser.Username} : Failed - {errorMessage}");
			}
		}

		/// <summary>
		/// Moves and Rotates the Building Placer by the specified amount.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void AdjustBuildingPlacer(Player player, string command, params string[] args)
		{
			Vector3 moveVector = Vector3.zero;
			int rotationAmount = 0;

			// Append Command as an argument
			string[] argsAppended = new string[args.Length + 1];
			argsAppended[0] = command;
			args.CopyTo(argsAppended, 1);


			for (int i = 0; i < argsAppended.Length; i += 2)
			{
				int value = 0;

				if (!((i + 1) < argsAppended.Length && int.TryParse(argsAppended[i + 1], out value)))
					value = 1;

				switch (argsAppended[i])
				{
					case "up":
						moveVector += Vector3.right * value;
						break;
					case "down":
						moveVector += Vector3.left * value;
						break;
					case "left":
						moveVector += Vector3.forward * value;
						break;
					case "right":
						moveVector += Vector3.back * value;
						break;
					case "rotate":
						rotationAmount += value;
						break;
				}

			}

			GameManager.Instance.BuildingManager.TryMoveBuilding(player, moveVector);
			GameManager.Instance.BuildingManager.TryRotateBuilding(player, rotationAmount);
		}

		/// <summary>
		/// Confirms the placement of the building and returns a message if it failed.
		/// </summary>
		/// <param name="player"></param>
		public static void ConfirmBuildingPlacement(Player player)
		{
			if (!GameManager.Instance.BuildingManager.TryPlaceBuilding(player, out string errorMessage))
			{
				MessageSender.SendMessage($"{player.TwitchUser.Username} Failed - {errorMessage}");
			}
		}

		/// <summary>
		/// Cancels the placement of a building.
		/// </summary>
		/// <param name="player"></param>
		public static void CancelBuildingPlacement(Player player)
		{
			GameManager.Instance.BuildingManager.TryCancelBuilding(player);
		}

		/// <summary>
		/// Attempts to level up all buildings of a given type by the given amount;
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void LevelBuilding(Player player, BuildingType type, int id, int iterations)
		{

			string errorMessage = "";
			int successfulLevels = 0;
			for (int i = 0; i < iterations; i++)
				if (!GameManager.Instance.BuildingManager.TryLevelBuilding(type, id - 1, out errorMessage))
					break;
				else
					successfulLevels++;

			if (successfulLevels > 0)
				MessageSender.SendMessage($"{player.TwitchUser.Username} Successfully Leveled Building {successfulLevels} {(successfulLevels > 1 ? "Times" : "Time")}");
			else
				MessageSender.SendMessage($"{player.TwitchUser.Username} Failed - {errorMessage}");
		}


		public static void LevelAllOfType(Player player, string command, params string[] args)
		{
			if (args.Length < 2)
				return;

			string buildType = GetBuildingTypeFromArg(args[0]);
			int successfulAttempts = 0;
			if (Enum.TryParse(buildType, out BuildingType type) && int.TryParse(args[1], out int levelTo))
			{
				var buildingsOfType = GameManager.Instance.BuildingManager.GetBuildingsByType(type);

				if (buildingsOfType.Count <= 0)
					return;

				buildingsOfType = buildingsOfType.OrderByDescending(x => x.LevelHandler.Level).ToList();

				for (int i = 0; i < levelTo; i++)
				{
					bool successfulLevel = false;
					for (int j = buildingsOfType.Count - 1; j >= 0; j--)
					{
						if (buildingsOfType[j].LevelHandler.Level >= levelTo)
							continue;
						if (!GameManager.Instance.BuildingManager.TryLevelBuilding(buildingsOfType[j], out string errorMessage))
							continue;
						successfulAttempts++;
						successfulLevel = true;
					}

					if (!successfulLevel)
						break;

				}
			}

			if (successfulAttempts > 0)
				MessageSender.SendPlayerMessage(player, $"Successfully leveled {successfulAttempts} times!");
			else
				MessageSender.SendPlayerMessage(player, $"Failed to level buildings");
		}

		public static void GetBuildingInformation(Player player, string command, params string[] args)
		{
			string buildingTypeString = GetBuildingTypeFromArg(args[0]);

			if (!Enum.TryParse(buildingTypeString, out BuildingType type))
				return;

			BuildingManager manager = GameManager.Instance.BuildingManager;

			var buildData = BuildingManager.GetBuildingData(type);

			var resourceCost = buildData.BuildResourceCost;

			int woodCost = resourceCost.WoodCost + (int)((float)(resourceCost.WoodCost * manager.BuildingCounts[type]) * buildData.CostIncreasePerBuildingMultiplier);
			int oreCost = resourceCost.OreCost + (int)((float)(resourceCost.OreCost * manager.BuildingCounts[type]) * buildData.CostIncreasePerBuildingMultiplier);
			int foodCost = resourceCost.FoodCost + (int)((float)(resourceCost.FoodCost * manager.BuildingCounts[type]) * buildData.CostIncreasePerBuildingMultiplier);
			int goldCost = resourceCost.GoldCost + (int)((float)(resourceCost.GoldCost * manager.BuildingCounts[type]) * buildData.CostIncreasePerBuildingMultiplier);
			woodCost -= manager.CalculateCostReduction(type, woodCost);
			oreCost -= manager.CalculateCostReduction(type, oreCost);
			foodCost -= manager.CalculateCostReduction(type, foodCost);
			goldCost -= manager.CalculateCostReduction(type, goldCost);
			int maxLevel = manager.BuildingsMaxLevel[type];

			string message = $"Building Cost for '{type}': Wood: {woodCost} | Ore: {oreCost} | Food: {foodCost} | Gold: {goldCost} | Max Level: {maxLevel}";
			MessageSender.SendMessage($"{player.TwitchUser.Username}: {message}");
		}

		/// <summary>
		/// Attempts to remove a building based on the given arguments.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void RemoveBuilding(Player player, string command, params string[] args)
		{
			if (args.Length < 2)
				return;

			string buildType = GetBuildingTypeFromArg(args[0]);
			if (Enum.TryParse(buildType, out BuildingType type) && int.TryParse(args[1], out int index))
			{

				if (GameManager.Instance.BuildingManager.TryRemoveBuilding(type, index - 1, out string errorMessage))
				{
					MessageSender.SendMessage($"{player.TwitchUser.Username} Successfully Removed Building");
				}
			}
		}

		/// <summary>
		/// Displays the ID of all buildings of the given type.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ShowBuildingIDsByType(Player player, string command, params string[] args)
		{
			string buildType = GetBuildingTypeFromArg(args[0]);

			if (Enum.TryParse(buildType, out BuildingType type))
			{
				GameManager.Instance.BuildingManager.DisplayBuildingIdsOfType(type);
			}
		}

		/// <summary>
		/// Displays the currently unlocked buildings.
		/// </summary>
		/// <param name="player"></param>
		public static void PrintUnlockedBuildings(Player player)
		{
			string buildingList = "Unlocked Buildings: ";
			bool hasBuildings = false;

			for (int i = 0; i < (int)BuildingType.Count; i++)
			{
				BuildingType type = (BuildingType)i;

				if (GameManager.Instance.BuildingManager.BuildingsUnlocked.ContainsKey(type) && GameManager.Instance.BuildingManager.BuildingsUnlocked[type] && type != BuildingType.Townhall)
				{
					hasBuildings = true;
					buildingList += $"{type}, ";
				}
			}

			if (hasBuildings)
			{
				buildingList = buildingList.Remove(buildingList.Length - 2, 2);
			}
			else
				buildingList += "None";

			MessageSender.SendMessage($"{player.TwitchUser.Username} {buildingList}");
		}
	}
}