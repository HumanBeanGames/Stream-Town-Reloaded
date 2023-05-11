using Character;
using Managers;
using System;
using Utils;

namespace Twitch.Commands
{
	/// <summary>
	/// Handles all Twitch chat commands related to Player Roles.
	/// </summary>
	public static class RoleCommands
	{
		/// <summary>
		/// Attempts to change the role of the User.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void TryChangeRole(Player player, string command, params string[] args)
		{
			// Convert first letter to Uppercase to work with enum parse
			string r = char.ToUpper(args[0][0]) + args[0].Substring(1);

			if (Enum.TryParse(r, out PlayerRole role))
			{
				if (role == PlayerRole.Ruler && GameManager.Instance.PlayerManager.Ruler != player)
					return;

				if (player.RoleHandler.TrySetRole(role))
				{
					MessageSender.SendPreBuiltMessage(player.TwitchUser.Username, "roleSwitched");
				}
			}
		}

		/// <summary>
		/// Sends a message detailing the User's role and level.
		/// </summary>
		/// <param name="player"></param>
		public static void Role(Player player)
		{
			string message = $"{player.TwitchUser.Username} you are currently a level {player.RoleHandler.PlayerRoleData.CurrentLevel} {player.RoleHandler.CurrentRole}";
			MessageSender.SendMessage(message);
		}

		/// <summary>
		/// Sends a message detailing the User's health.
		/// </summary>
		/// <param name="player"></param>
		public static void Health(Player player)
		{
			string message = $"{player.TwitchUser.Username} your health is: ({player.HealthHandler.Health}/{player.HealthHandler.MaxHealth})";
			MessageSender.SendMessage(message);
		}

		/// <summary>
		/// Sends a message detailing the User's Role, level and experience.
		/// </summary>
		/// <param name="player"></param>
		public static void Experience(Player player)
		{
			string message = $"{player.TwitchUser.Username} you are a level ({player.RoleHandler.PlayerRoleData.CurrentLevel}/{RoleManager.MAX_ROLE_LEVEl}) {player.RoleHandler.CurrentRole}. Current Exp: ({player.RoleHandler.PlayerRoleData.CurrentExp}/{player.RoleHandler.PlayerRoleData.RequiredExp}).";
			MessageSender.SendMessage(message);

		}

		/// <summary>
		/// Sends a message detailing the User's Role, level and experience.
		/// </summary>
		/// <param name="player"></param>
		public static void ExperienceForRole(Player player, PlayerRole role)
		{
			if (player.RoleHandler.TryGetRoleData(role, out PlayerRoleData data))
			{
				string message = $"{player.TwitchUser.Username} you are a level ({data.CurrentLevel}/{RoleManager.MAX_ROLE_LEVEl}) {data.Role}. Current Exp: ({data.CurrentExp}/{data.RequiredExp}).";
				MessageSender.SendMessage(message);
			}
			else
				MessageSender.SendMessage($"{player.TwitchUser.Username} you currenty don't have data for {role}");
		}

		/// <summary>
		/// Displays the Station IDs based on Player's Station Flags.
		/// </summary>
		/// <param name="player"></param>
		public static void DisplayStationIDs(Player player)
		{
			GameManager.Instance.StationManager.DisplayStationIdByType(player.StationSensor.StationMask);
		}

		/// <summary>
		/// Attempts to change the User's station.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void SwitchStation(Player player, string command, params string[] args)
		{

			if (int.TryParse(args[0], out int index))
			{
				var station = GameManager.Instance.StationManager.GetStationByFlaggedIndex(player.StationSensor.StationMask, index - 1);
				player.StationSensor.UpdateStation = false;
				player.StationSensor.TrySetStation(station);
				MessageSender.SendPlayerMessage(player, "Station Switched!");
			}
		}

		/// <summary>
		/// Attempts to display the ID's of the targets currently stored in their station.
		/// </summary>
		/// <param name="player"></param>
		public static void DisplayTargetIDs(Player player)
		{
			player.StationSensor.CurrentStation.DisplayTargetIDsByMask(player.RoleHandler.RoleData_SO.TargetFlags);
		}

		/// <summary>
		/// Attempts to switch the User's current Target.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void SwitchTarget(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				var targetable = player.StationSensor.CurrentStation.GetTargetByFlaggedIndex(player.RoleHandler.RoleData_SO.TargetFlags, index - 1);

				if (targetable)
				{
					if (player.TargetSensor.TrySetTarget(targetable))
						MessageSender.SendPlayerMessage(player, "Target Switched!");
				}
			}
		}
	}
}