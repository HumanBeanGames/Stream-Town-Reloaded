using Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Twitch.Commands
{
	/// <summary>
	/// Contains all commands used in Twitch chat.
	/// </summary>
	public static class CommandDictionary
	{

		private static Dictionary<string, Action<Player, string, string[]>> _commandsWithArgs;

		/// <summary>
		/// Contains all Game Commands that require further arguments.
		/// </summary>
		public static Dictionary<string, Action<Player, string, string[]>> CommandsWithArgs
		{
			get
			{
				if (_commandsWithArgs == null)
				{
					_commandsWithArgs = new Dictionary<string, Action<Player, string, string[]>>();
					_commandsWithArgs.Add("role", RoleCommands.TryChangeRole);
					_commandsWithArgs.Add("build", BuildingCommands.StartBuild);
					_commandsWithArgs.Add("move", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("up", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("down", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("left", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("right", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("rotate", BuildingCommands.AdjustBuildingPlacer);
					_commandsWithArgs.Add("level", MiscCommands.Level);
					_commandsWithArgs.Add("remove", BuildingCommands.RemoveBuilding);
					_commandsWithArgs.Add("bid", BuildingCommands.ShowBuildingIDsByType);
					_commandsWithArgs.Add("station", RoleCommands.SwitchStation);
					_commandsWithArgs.Add("target", RoleCommands.SwitchTarget);
					_commandsWithArgs.Add("hair", PlayerCommands.ChangeHairStyle);
					_commandsWithArgs.Add("facialhair", PlayerCommands.ChangeFacialHair);
					_commandsWithArgs.Add("eyes", PlayerCommands.ChangeEyes);
					_commandsWithArgs.Add("body", PlayerCommands.ChangeBodyType);
					_commandsWithArgs.Add("haircolor", PlayerCommands.ChangeHairColor);
					_commandsWithArgs.Add("eyecolor", PlayerCommands.ChangeEyeColor);
					_commandsWithArgs.Add("addresource", GameMasterCommands.AddResources);
					_commandsWithArgs.Add("vote", PlayerCommands.Vote);
					_commandsWithArgs.Add("modrole", ModeratorCommands.ChangePlayerRole);
					_commandsWithArgs.Add("kill", GameMasterCommands.KillPlayer);
					_commandsWithArgs.Add("grevive", GameMasterCommands.RevivePlayer);
					_commandsWithArgs.Add("revive", PlayerCommands.RevivePlayerWithCost);
					_commandsWithArgs.Add("givexp", GameMasterCommands.GivePlayerExp);
					_commandsWithArgs.Add("givexpall", GameMasterCommands.GiveAllExp);
					_commandsWithArgs.Add("levelup", GameMasterCommands.LevelUpPlayer);
					_commandsWithArgs.Add("qevent", GameMasterCommands.QueueEvent);
					_commandsWithArgs.Add("buy", RulerCommands.BuyResource);
					_commandsWithArgs.Add("sell", RulerCommands.SellResource);
					_commandsWithArgs.Add("levelall", BuildingCommands.LevelAllOfType);
					//_commandsWithArgs.Add("bi", BuildingCommands.GetBuildingInformation);
					_commandsWithArgs.Add("recruit", RulerCommands.RecruitNPC);
					_commandsWithArgs.Add("givepet", GameMasterCommands.GivePlayerPet);
					_commandsWithArgs.Add("pet", PlayerCommands.SwitchPet);
					_commandsWithArgs.Add("cam", RulerCommands.MoveCamera);
					_commandsWithArgs.Add("info", MiscCommands.ItemInfo);
					_commandsWithArgs.Add("rrole", RulerCommands.SwapRecruitRole);
					_commandsWithArgs.Add("rinfo", RulerCommands.DisplayRecruitInfo);
					_commandsWithArgs.Add("rdismiss", RulerCommands.DismissRecruit);
					_commandsWithArgs.Add("resetid", GameMasterCommands.ResetID);
				}
				return _commandsWithArgs;
			}
		}

		private static Dictionary<string, Action<Player>> _commandsNoArgs;

		/// <summary>
		/// Contains all Game Commands that do NOT require arguments.
		/// </summary>
		public static Dictionary<string, Action<Player>> CommandsNoArgs
		{
			get
			{
				if (_commandsNoArgs == null)
				{
					_commandsNoArgs = new Dictionary<string, Action<Player>>();
					_commandsNoArgs.Add("role", RoleCommands.Experience);
					_commandsNoArgs.Add("level", RoleCommands.Experience);
					_commandsNoArgs.Add("health", RoleCommands.Health);
					_commandsNoArgs.Add("revive", PlayerCommands.ReviveWithCost);
					_commandsNoArgs.Add("confirm", BuildingCommands.ConfirmBuildingPlacement);
					_commandsNoArgs.Add("accept", BuildingCommands.ConfirmBuildingPlacement);
					_commandsNoArgs.Add("cancel", BuildingCommands.CancelBuildingPlacement);
					_commandsNoArgs.Add("station", RoleCommands.DisplayStationIDs);
					_commandsNoArgs.Add("target", RoleCommands.DisplayTargetIDs);
					_commandsNoArgs.Add("tbuildcosts", GameMasterCommands.ToggleBuildCosts);
					_commandsNoArgs.Add("trolelimits", GameMasterCommands.TogglePlayerRoleLimits);
					_commandsNoArgs.Add("ping", PlayerCommands.PingPlayer);
					_commandsNoArgs.Add("rulervote", ModeratorCommands.StartKingVote);
					_commandsNoArgs.Add("stopevent", GameMasterCommands.StopCurrentEvent);
					_commandsNoArgs.Add("cobj", GameMasterCommands.CompleteCurrentGoal);
					_commandsNoArgs.Add("randtech", GameMasterCommands.StartRandomTech);
					_commandsNoArgs.Add("techvote", GameMasterCommands.StartVoteTech);
					_commandsNoArgs.Add("pet", PlayerCommands.PrintPetsList);
					_commandsNoArgs.Add("pets", PlayerCommands.PrintPetsList);
					_commandsNoArgs.Add("gaction", GameMasterCommands.ActionEvent);
					_commandsNoArgs.Add("unlockall", GameMasterCommands.UnlockAllTech);
					_commandsNoArgs.Add("unlockage2", GameMasterCommands.UnlockToAge2);
					_commandsNoArgs.Add("resetcam", RulerCommands.ResetCamera);
					_commandsNoArgs.Add("stuck", PlayerCommands.Unstuck);
					_commandsNoArgs.Add("praise", PlayerCommands.Praise);
					_commandsNoArgs.Add("buildings", BuildingCommands.PrintUnlockedBuildings);
					_commandsNoArgs.Add("rid", RulerCommands.ShowRecruitIds);
					_commandsNoArgs.Add("recruits", RulerCommands.RecruitCount);
					_commandsNoArgs.Add("resign", RulerCommands.Resign);
				}
				return _commandsNoArgs;
			}
		}

		private static Dictionary<string, Action> _simpleCommands;

		/// <summary>
		/// Contains all Game Commands that do not require a created character or arguments.
		/// </summary>
		public static Dictionary<string, Action> SimpleCommands
		{
			get
			{
				if (_simpleCommands == null)
				{
					_simpleCommands = new Dictionary<string, Action>();
					_simpleCommands.Add("stdiscord", MiscCommands.Discord);
					_simpleCommands.Add("help", MiscCommands.Help);
					_simpleCommands.Add("townstats", MiscCommands.TownStats);
				}
				return _simpleCommands;
			}
		}

		/// <summary>
		/// A list of acceptable variants for the word "create".
		/// </summary>
		public static List<string> CreateNameVariants = new List<string>
		{
			"create","crate","crete","join","start","creta","ceate","cate","crtea", "ligma"
		};

		/// <summary>
		/// Allows commands to have multiple variants.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="args"></param>
		/// <param name="command"></param>
		/// <param name="aliases"></param>
		private static void AliasCommand(Player player, string[] args, Action<Player, string[]> command, params string[] aliases)
		{
			var newArgs = args.ToList();
			for (int i = 0; i < aliases.Length; i++)
			{
				newArgs.Insert(i, aliases[i]);
			}

			command(player, newArgs.ToArray());
		}
	}
}