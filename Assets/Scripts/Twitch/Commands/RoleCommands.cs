using Character;

using System;
using Utils;

namespace Twitch.Commands
{
    /// <summary>
    /// Handles all Twitch chat commands related to Player Roles.
    /// </summary>
    public static class RoleCommands
    {
        public static string TryChangeRole(Player player, string command, params string[] args)
        {
            if (args.Length == 0)
                return "Usage: !role <RoleName>";

            string r = char.ToUpper(args[0][0]) + args[0].Substring(1);
            if (Enum.TryParse(r, out PlayerRole role))
            {
                if (role == PlayerRole.Ruler && GameManager.Instance.PlayerManager.Ruler != player)
                    return "You cannot switch to Ruler unless you're the current ruler.";

                if (player.RoleHandler.TrySetRole(role, out string failureReason))
                    return $"{player.TwitchUser.Username}, your role was successfully changed to {role}.";
                else
                    return $"{player.TwitchUser.Username}, role switch failed: {failureReason}";
            }

            return $"{player.TwitchUser.Username}, unknown role '{args[0]}'";
        }

        public static string Role(Player player)
        {
            return $"{player.TwitchUser.Username}, you are currently a level {player.RoleHandler.PlayerRoleData.CurrentLevel} {player.RoleHandler.CurrentRole}.";
        }

        public static string Health(Player player)
        {
            return $"{player.TwitchUser.Username}, your health is: ({player.HealthHandler.Health}/{player.HealthHandler.MaxHealth}).";
        }

        public static string Experience(Player player)
        {
            return $"{player.TwitchUser.Username}, you are a level ({player.RoleHandler.PlayerRoleData.CurrentLevel}/{RoleManager.MAX_ROLE_LEVEL}) {player.RoleHandler.CurrentRole}. Current Exp: ({player.RoleHandler.PlayerRoleData.CurrentExp}/{player.RoleHandler.PlayerRoleData.RequiredExp}).";
        }

        public static string ExperienceForRole(Player player, PlayerRole role)
        {
            if (player.RoleHandler.TryGetRoleData(role, out PlayerRoleData data))
            {
                return $"{player.TwitchUser.Username}, you are a level ({data.CurrentLevel}/{RoleManager.MAX_ROLE_LEVEL}) {data.Role}. Current Exp: ({data.CurrentExp}/{data.RequiredExp}).";
            }
            return $"{player.TwitchUser.Username}, you currently don't have data for {role}.";
        }

        public static string DisplayStationIDs(Player player)
        {
            GameManager.Instance.StationManager.DisplayStationIdByType(player.StationSensor.StationMask);
            return "Station IDs displayed on screen.";
        }

        public static string SwitchStation(Player player, string command, params string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int index))
                return "Usage: !station <id>";

            var station = GameManager.Instance.StationManager.GetStationByFlaggedIndex(player.StationSensor.StationMask, index - 1);
            player.StationSensor.UpdateStation = false;
            player.StationSensor.TrySetStation(station);
            return $"{player.TwitchUser.Username}, station switched!";
        }

        public static string DisplayTargetIDs(Player player)
        {
            player.StationSensor.CurrentStation.DisplayTargetIDsByMask(player.RoleHandler.RoleData_SO.TargetFlags);
            return "Target IDs displayed on screen.";
        }

        public static string SwitchTarget(Player player, string command, params string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int index))
                return "Usage: !target <id>";

            var targetable = player.StationSensor.CurrentStation.GetTargetByFlaggedIndex(player.RoleHandler.RoleData_SO.TargetFlags, index - 1);

            if (targetable && player.TargetSensor.TrySetTarget(targetable))
                return $"{player.TwitchUser.Username}, target switched!";
            else
                return $"{player.TwitchUser.Username}, failed to switch target.";
        }
    }
}
