using Character;
using Managers;
using UnityEngine;

namespace Twitch.Commands
{
    /// <summary>
    /// Handles all Twitch chat commands related to Moderation.
    /// </summary>
    public static class ModeratorCommands
    {
        public static string StartKingVote(Player player)
        {
            if (!player.IsModerator())
                return "You are not authorized to start a king vote.";

            if (PlayerManager.Ruler == null)
            {
                GameManager.Instance.GameEventManager.StartNewRulerVote();
                return "Ruler vote started!";
            }
            else
            {
                GameManager.Instance.GameEventManager.StartKeepRulerVote();
                return "Keep current ruler vote started!";
            }
        }

        public static string ChangePlayerRole(Player player, string command, params string[] args)
        {
            if (!player.IsModerator())
                return "You are not authorized to change player roles.";

            if (args.Length < 2)
                return "Usage: !modrole <playername> <newrole>";

            string playerNameArg = args[0].ToLower();

            if (PlayerManager.PlayerExistsByNameToLower(playerNameArg, out int index))
            {
                Player targetPlayer = PlayerManager.GetPlayer(index);
                string[] newArgs = new string[] { args[1] };
                RoleCommands.TryChangeRole(targetPlayer, command, newArgs);
                return $"{targetPlayer.TwitchUser.Username}'s role has been updated.";
            }
            else
            {
                return $"Player '{playerNameArg}' not found.";
            }
        }

        public static bool IsModerator(this Player player)
        {
            if (player.TwitchUser.TwitchUserType == TwitchLib.Client.Enums.UserType.Moderator)
                return true;
            if (player.TwitchUser.TwitchUserType == TwitchLib.Client.Enums.UserType.Broadcaster)
                return true;
            if (player.TwitchUser.GameUserType == Utils.GameUserType.GameMaster)
                return true;

            return false;
        }
    }
}