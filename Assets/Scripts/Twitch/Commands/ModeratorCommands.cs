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
		public static void StartKingVote(Player player)
		{
			if (!player.IsModerator())
				return;

			if (GameManager.Instance.PlayerManager.Ruler == null)
				GameManager.Instance.GameEventManager.StartNewRulerVote();
			else
				GameManager.Instance.GameEventManager.StartKeepRulerVote();
		}

		public static void ChangePlayerRole(Player player, string command, params string[] args)
		{
			if (!player.IsModerator())
				return;

			if (args.Length < 2)
				return;

			string playerNameArg = args[0].ToLower();

			if (GameManager.Instance.PlayerManager.PlayerExistsByNameToLower(playerNameArg, out int index))
			{
				Player targetPlayer = GameManager.Instance.PlayerManager.GetPlayer(index);
				string[] newArgs = new string[] { args[1] };

				RoleCommands.TryChangeRole(targetPlayer, command, newArgs);
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
