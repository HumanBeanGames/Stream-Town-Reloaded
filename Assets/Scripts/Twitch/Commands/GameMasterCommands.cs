using Character;
using GameEventSystem;
using GameEventSystem.Events;
using Managers;
using Pets.Enumerations;
using System;
using System.Linq;
using Twitch.Utils;
using Utils;

namespace Twitch.Commands
{
	/// <summary>
	/// Handles all Twitch Chat messages relating to Game Management.
	/// </summary>
	public static class GameMasterCommands
	{
		public static void ToggleBuildCosts(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.BuildingsCostResources = !GameManager.Instance.BuildingsCostResources;
			MessageSender.SendMessage($"Buildings Cost Resources: {GameManager.Instance.BuildingsCostResources}");
		}

		public static void TogglePlayerRoleLimits(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.PlayerRoleLimits = !GameManager.Instance.PlayerRoleLimits;
			MessageSender.SendMessage($"Player Role Limits: {GameManager.Instance.PlayerRoleLimits}");
		}

		public static void AddResources(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (args.Length < 2)
				return;

			string resourceArg = args[0].ToLower();
			Resource resource = Resource.None;
			//TODO:: Make static helper function
			for (int i = 1; i < (int)Resource.Count - 1; i++)
			{
				if (resourceArg == ((Resource)i).ToString().ToLower())
					resource = (Resource)i;
			}

			if (resource == Resource.None)
				return;

			if (int.TryParse(args[1], out int amount))
			{
				GameManager.Instance.TownResourceManager.AddResource(resource, amount);
			}
		}

		public static void KillPlayer(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (Utils.TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
				targetPlayer.HealthHandler.SetHealth(0);
		}

		public static void RevivePlayer(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (Utils.TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
				targetPlayer.HealthHandler.Revive();

		}

		public static void GivePlayerExp(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (args.Length < 2)
				return;

			if (Utils.TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
			{
				if (int.TryParse(args[1], out int amount))
				{
					if (amount <= 0)
						return;

					targetPlayer.RoleHandler.PlayerRoleData.IncreaseExperience(amount);
				}
			}
		}

		public static void LevelUpPlayer(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			int amount = 1;
			if (args.Length >= 2)
				int.TryParse(args[1], out amount);

			if (amount <= 0)
				return;

			if (Utils.TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
			{
				for (int i = 0; i < amount; i++)
				{
					targetPlayer.RoleHandler.PlayerRoleData.LevelUp();
				}
			}
		}

		public static void GiveAllExp(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (int.TryParse(args[0], out int amount))
			{
				if (amount <= 0)
					return;

				for (int i = 0; i < GameManager.Instance.PlayerManager.PlayerCount(); i++)
				{
					GameManager.Instance.PlayerManager.GetPlayer(i).RoleHandler.PlayerRoleData.IncreaseExperience(amount);
				}
			}
		}

		public static void StopCurrentEvent(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameEvent currentEvent = GameManager.Instance.GameEventManager.CurrentEvent;

			if (currentEvent == null)
				return;

			currentEvent.Stop();
		}

		public static void GivePlayerPet(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (args.Length < 2)
				return;

			PetType type = TwitchUtils.GetPetTypeFromString(args[1]);

			if (type == PetType.Count)
				return;

			if (TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
			{
				targetPlayer.PetsUnlocked[type] = true;
			}

		}

		public static void QueueEvent(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;
			GameEvent.EventType type = TwitchUtils.StringToEventEnum(args[0]);
			switch (type)
			{
				case GameEvent.EventType.None:
					break;
				case GameEvent.EventType.FishGod:
					GameManager.Instance.GameEventManager.AddEvent(new FishGodEvent(0));
					break;
				case GameEvent.EventType.NightRaid:
					break;
				case GameEvent.EventType.BloodMoonRaid:
					break;
				case GameEvent.EventType.AdventureLandNecro:
					break;
				case GameEvent.EventType.AdventureLandFishGod:
					break;
				case GameEvent.EventType.DragonFire:
					break;
				case GameEvent.EventType.DragonForest:
					break;
				case GameEvent.EventType.DragonIce:
					break;
				case GameEvent.EventType.DragonTwoHeaded:
					break;
				case GameEvent.EventType.DragonUndead:
					break;
				case GameEvent.EventType.Subscription:
					break;
				case GameEvent.EventType.BitsDonated:
					break;
				case GameEvent.EventType.Vote:
					break;
				case GameEvent.EventType.MonsterRaid:
					string[] enemies = new string[] { "Minotaur" };
					GameManager.Instance.GameEventManager.AddEvent(new RaidEvent(0, 1200, enemies, boss: "MinotaurBoss"));
					break;
				default:
					break;
			}
		}

		public static void CompleteCurrentGoal(Player player)
		{
			if (!player.IsGameMaster())
				return;

			if (GameManager.Instance.TownGoalManager.CurrentGoals.Count > 0)
				GameManager.Instance.TownGoalManager.CurrentGoals[0].ForceComplete();
		}

		public static void StartRandomTech(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.TechTreeManager.StartNewRandomTech();
		}

		public static void StartVoteTech(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.TechTreeManager.StartNewTechVote();
		}

		public static void ActionEvent(Player player)
		{
			if (!player.IsGameMaster())
				return;

			var ev = GameManager.Instance.GameEventManager.CurrentEvent;

			if (ev == null)
				return;

			ev.Action();
		}

		public static void UnlockAllTech(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.TechTreeManager.UnlockAllTech();
		}

		public static void UnlockToAge2(Player player)
		{
			if (!player.IsGameMaster())
				return;

			GameManager.Instance.TechTreeManager.UnlockToAge2Tech();
		}

		public static bool IsGameMaster(this Player player)
		{
			return GameManager.GM_IDS.Contains(player.TwitchUser.UserID);
		}

		public static void ResetID(Player player, string command, params string[] args)
		{
			if (!player.IsGameMaster())
				return;

			if (args.Length == 2)
			{
				if (args[0] == "building" && args[0] != (BuildingType.Townhall).ToString().ToLower())
				{
					if (Enum.TryParse(args[1], true, out BuildingType building))
						GameManager.Instance.BuildingManager.ResetBuilding(building);
				}

				MessageSender.SendMessage($"Reset ID: {args[0]}, {args[1]}");
			}
		}
	}
}
