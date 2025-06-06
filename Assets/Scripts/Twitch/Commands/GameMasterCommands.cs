using Character;
using GameEventSystem;
using GameEventSystem.Events;
using Managers;
using Pets.Enumerations;
using System;
using System.Linq;
using TechTree;
using TownGoal;
using Twitch.Utils;
using Utils;

namespace Twitch.Commands
{
    /// <summary>
    /// Handles all Twitch Chat messages relating to Game Management.
    /// </summary>
    public static class GameMasterCommands
    {
        public static string ToggleBuildCosts(Player player)
        {
            if (!player.IsGameMaster()) return null;
            GameManager.Instance.BuildingsCostResources = !GameManager.Instance.BuildingsCostResources;
            return $"Buildings Cost Resources: {GameManager.Instance.BuildingsCostResources}";
        }

        public static string TogglePlayerRoleLimits(Player player)
        {
            if (!player.IsGameMaster()) return null;
            GameManager.Instance.PlayerRoleLimits = !GameManager.Instance.PlayerRoleLimits;
            return $"Player Role Limits: {GameManager.Instance.PlayerRoleLimits}";
        }

        public static string AddResources(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (args.Length < 2) return "Usage: !addresource <type> <amount>";

            string resourceArg = args[0].ToLower();
            Resource resource = Resource.None;
            for (int i = 1; i < (int)Resource.Count - 1; i++)
                if (resourceArg == ((Resource)i).ToString().ToLower()) resource = (Resource)i;

            if (resource == Resource.None) return "Unknown resource type.";
            if (!int.TryParse(args[1], out int amount)) return "Invalid amount.";

            TownResourceManager.AddResource(resource, amount);
            return $"Added {amount} {resource}.";
        }

        public static string KillPlayer(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
            {
                targetPlayer.HealthHandler.SetHealth(0);
                return $"{targetPlayer.TwitchUser.Username} has been killed.";
            }
            return "Player not found.";
        }

        public static string RevivePlayer(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
            {
                targetPlayer.HealthHandler.Revive();
                return $"{targetPlayer.TwitchUser.Username} has been revived.";
            }
            return "Player not found.";
        }

        public static string GivePlayerExp(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (args.Length < 2) return "Usage: !givexp <user> <amount>";
            if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer) && int.TryParse(args[1], out int amount) && amount > 0)
            {
                targetPlayer.RoleHandler.PlayerRoleData.IncreaseExperience(amount);
                return $"Gave {amount} exp to {targetPlayer.TwitchUser.Username}.";
            }
            return "Invalid input.";
        }

        public static string LevelUpPlayer(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            int amount = 1;
            if (args.Length >= 2) int.TryParse(args[1], out amount);
            if (amount <= 0) return "Invalid level amount.";
            if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
            {
                for (int i = 0; i < amount; i++) targetPlayer.RoleHandler.PlayerRoleData.LevelUp();
                return $"Leveled up {targetPlayer.TwitchUser.Username} {amount} times.";
            }
            return "Player not found.";
        }

        public static string GiveAllExp(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (!int.TryParse(args[0], out int amount) || amount <= 0) return "Invalid amount.";
            for (int i = 0; i < PlayerManager.PlayerCount(); i++)
                PlayerManager.GetPlayer(i).RoleHandler.PlayerRoleData.IncreaseExperience(amount);
            return $"Gave {amount} exp to all players.";
        }

        public static string StopCurrentEvent(Player player)
        {
            if (!player.IsGameMaster()) return null;
            var currentEvent = GameEventManager.CurrentEvent;
            if (currentEvent == null) return "No current event to stop.";
            currentEvent.Stop();
            return "Current event stopped.";
        }

        public static string GivePlayerPet(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (args.Length < 2) return "Usage: !givepet <user> <pet>";
            PetType type = TwitchUtils.GetPetTypeFromString(args[1]);
            if (type == PetType.Count) return "Invalid pet type.";
            if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
            {
                targetPlayer.PetsUnlocked[type] = true;
                return $"Gave pet {type} to {targetPlayer.TwitchUser.Username}.";
            }
            return "Player not found.";
        }

        public static string QueueEvent(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            GameEvent.EventType type = TwitchUtils.StringToEventEnum(args[0]);
            switch (type)
            {
                case GameEvent.EventType.FishGod:
                    GameEventManager.AddEvent(new FishGodEvent(0));
                    return "Queued FishGod Event.";
                case GameEvent.EventType.MonsterRaid:
                    GameEventManager.AddEvent(new RaidEvent(0, 1200, new string[] { "Minotaur" }, boss: "MinotaurBoss"));
                    return "Queued Monster Raid.";
                default:
                    return "Invalid or unsupported event.";
            }
        }

        public static string CompleteCurrentGoal(Player player)
        {
            if (!player.IsGameMaster()) return null;
            if (TownGoalManager.CurrentGoals.Count > 0)
            {
                TownGoalManager.CurrentGoals[0].ForceComplete();
                return "Completed current goal.";
            }
            return "No goal to complete.";
        }

        public static string StartRandomTech(Player player)
        {
            if (!player.IsGameMaster()) return null;
            TechTreeManager.StartNewRandomTech();
            return "Started new random tech.";
        }

        public static string StartVoteTech(Player player)
        {
            if (!player.IsGameMaster()) return null;
            TechTreeManager.StartNewTechVote();
            return "Started tech vote.";
        }

        public static string ActionEvent(Player player)
        {
            if (!player.IsGameMaster()) return null;
            var ev = GameEventManager.CurrentEvent;
            if (ev == null) return "No event to act on.";
            ev.Action();
            return "Event action triggered.";
        }

        public static string UnlockAllTech(Player player)
        {
            if (!player.IsGameMaster()) return null;
            TechTreeManager.UnlockAllTech();
            return "All tech unlocked.";
        }

        public static string UnlockToAge2(Player player)
        {
            if (!player.IsGameMaster()) return null;
            TechTreeManager.UnlockToAge2Tech();
            return "Unlocked to Age 2.";
        }

        public static bool IsGameMaster(this Player player)
        {
            return GameManager.GM_IDS.Contains(player.TwitchUser.UserID);
        }

        public static string ResetID(Player player, string command, params string[] args)
        {
            if (!player.IsGameMaster()) return null;
            if (args.Length != 2) return "Usage: !resetid <type> <name>";
            if (args[0] == "building" && Enum.TryParse(args[1], true, out BuildingType building))
            {
                BuildingManager.ResetBuilding(building);
                return $"Reset ID for building {args[1]}.";
            }
            return "Invalid reset command.";
        }
    }
}
