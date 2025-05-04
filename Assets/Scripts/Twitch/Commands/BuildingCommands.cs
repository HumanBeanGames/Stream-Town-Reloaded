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
        public static string GetBuildingTypeFromArg(string arg)
        {
            return char.ToUpper(arg[0]) + arg.Substring(1);
        }

        public static string StartBuild(Player player, string command, params string[] args)
        {
            string buildType = GetBuildingTypeFromArg(args[0]);
            if (Enum.TryParse(buildType, out BuildingType type))
            {
                if (!GameManager.Instance.BuildingManager.TryStartNewBuildingPlacer(player, type, out string errorMessage))
                    return $"Failed - {errorMessage}";
                return "Building placer initiated.";
            }
            return "Invalid building type.";
        }

        public static string LevelBuilding(Player player, BuildingType type, int id, int iterations)
        {
            string errorMessage = "";
            int successfulLevels = 0;

            for (int i = 0; i < iterations; i++)
            {
                if (!GameManager.Instance.BuildingManager.TryLevelBuilding(type, id - 1, out errorMessage))
                    break;
                successfulLevels++;
            }

            if (successfulLevels > 0)
                return $"{player.TwitchUser.Username} successfully leveled building {successfulLevels} {(successfulLevels > 1 ? "times" : "time")}.";
            else
                return $"{player.TwitchUser.Username} failed to level building - {errorMessage}";
        }


        public static string AdjustBuildingPlacer(Player player, string command, params string[] args)
        {
            Vector3 moveVector = Vector3.zero;
            int rotationAmount = 0;

            string[] argsAppended = new string[args.Length + 1];
            argsAppended[0] = command;
            args.CopyTo(argsAppended, 1);

            // Movement direction mapping for artificial cursor (UI space is Vector2)
            Vector2 uiMove = Vector2.zero;

            for (int i = 0; i < argsAppended.Length; i += 2)
            {
                int value = ((i + 1) < argsAppended.Length && int.TryParse(argsAppended[i + 1], out int v)) ? v : 1;

                switch (argsAppended[i])
                {
                    case "up":
                        moveVector += Vector3.right * value;
                        uiMove += Vector2.up * value;
                        break;
                    case "down":
                        moveVector += Vector3.left * value;
                        uiMove += Vector2.down * value;
                        break;
                    case "left":
                        moveVector += Vector3.forward * value;
                        uiMove += Vector2.left * value;
                        break;
                    case "right":
                        moveVector += Vector3.back * value;
                        uiMove += Vector2.right * value;
                        break;
                    case "rotate":
                        rotationAmount += value;
                        break;
                }
            }

            GameManager.Instance.BuildingManager.TryMoveBuilding(player, moveVector);
            GameManager.Instance.BuildingManager.TryRotateBuilding(player, rotationAmount);

            return "Adjusted building placer.";
        }

        public static string ConfirmBuildingPlacement(Player player)
        {
            if (!GameManager.Instance.BuildingManager.TryPlaceBuilding(player, out string errorMessage))
                return $"Failed - {errorMessage}";
            return "Building successfully placed.";
        }

        public static string CancelBuildingPlacement(Player player)
        {
            GameManager.Instance.BuildingManager.TryCancelBuilding(player);
            return "Building placement cancelled.";
        }

        public static string LevelAllOfType(Player player, string command, params string[] args)
        {
            if (args.Length < 2) return "Invalid arguments.";
            string buildType = GetBuildingTypeFromArg(args[0]);
            int successfulAttempts = 0;
            if (Enum.TryParse(buildType, out BuildingType type) && int.TryParse(args[1], out int levelTo))
            {
                var buildingsOfType = GameManager.Instance.BuildingManager.GetBuildingsByType(type);
                if (buildingsOfType.Count == 0) return "No buildings of this type.";
                buildingsOfType = buildingsOfType.OrderByDescending(x => x.LevelHandler.Level).ToList();
                for (int i = 0; i < levelTo; i++)
                {
                    bool successfulLevel = false;
                    foreach (var building in buildingsOfType)
                    {
                        if (building.LevelHandler.Level >= levelTo) continue;
                        if (GameManager.Instance.BuildingManager.TryLevelBuilding(building, out string _))
                        {
                            successfulAttempts++;
                            successfulLevel = true;
                        }
                    }
                    if (!successfulLevel) break;
                }
            }
            return successfulAttempts > 0 ? $"Successfully leveled {successfulAttempts} times!" : "Failed to level buildings.";
        }

        public static string RemoveBuilding(Player player, string command, params string[] args)
        {
            if (args.Length < 2) return "Invalid arguments.";
            string buildType = GetBuildingTypeFromArg(args[0]);
            if (Enum.TryParse(buildType, out BuildingType type) && int.TryParse(args[1], out int index))
            {
                if (GameManager.Instance.BuildingManager.TryRemoveBuilding(type, index - 1, out string errorMessage))
                    return "Successfully removed building.";
                else return $"Failed to remove building - {errorMessage}";
            }
            return "Invalid building type or index.";
        }

        public static string PrintUnlockedBuildings(Player player)
        {
            string buildingList = "Unlocked Buildings: ";
            bool hasBuildings = false;
            for (int i = 0; i < (int)BuildingType.Count; i++)
            {
                BuildingType type = (BuildingType)i;
                if (GameManager.Instance.BuildingManager.BuildingsUnlocked.TryGetValue(type, out bool unlocked) && unlocked && type != BuildingType.Townhall)
                {
                    hasBuildings = true;
                    buildingList += $"{type}, ";
                }
            }
            if (!hasBuildings) return "Unlocked Buildings: None";
            return buildingList.TrimEnd(',', ' ');
        }

        public static string ShowBuildingIDsByType(Player player, string command, params string[] args)
        {
            if (args.Length < 1) return "Usage: !showbuildings <type>";
            string buildType = GetBuildingTypeFromArg(args[0]);
            if (Enum.TryParse(buildType, out BuildingType type))
            {
                GameManager.Instance.BuildingManager.DisplayBuildingIdsOfType(type);
                return $"Displayed building IDs for {type}.";
            }
            return "Invalid building type.";
        }
    }
}
