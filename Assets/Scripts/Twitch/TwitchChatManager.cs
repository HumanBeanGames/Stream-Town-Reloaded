using Character;
using Managers;
using Twitch.Commands;
using TwitchLib.Client.Events;

namespace Twitch
{
	/// <summary>
	/// General Manager for all functions relating to Twitch Chat integration.
	/// </summary>
	public static class TwitchChatManager
	{
		/// <summary>
		/// Processes any command sent in chat on Twitch and invokes the required action is the command is valid.
		/// </summary>
		/// <param name="e"></param>
		public static void ProcessCommand(OnChatCommandReceivedArgs e)
		{
			if (!GameStateManager.PlayerReady)
				return;

			string command = e.Command.CommandText.ToLower();

			if (!MessageSender.MessagesAllowed)
			{
				if (e.Command.ArgumentsAsList == null || e.Command.ArgumentsAsList.Count < 0)
					return;
#if UNITY_EDITOR
				BroadcasterCommands.Connect("", e);
#else
				if (e.Command.CommandText.ToLower() == "connect" && e.Command.ChatMessage.IsBroadcaster)
					BroadcasterCommands.Connect(e.Command.ArgumentsAsList[0], e);
				return;
#endif

			}

			if (CommandDictionary.SimpleCommands.ContainsKey(command))
			{
				CommandDictionary.SimpleCommands[command].Invoke();
				return;
			}

			// Check that player exists
			if (GameManager.Instance.PlayerManager.PlayerExistsByID(e.Command.ChatMessage.UserId, out int index))
			{
				Player player = GameManager.Instance.PlayerManager.GetPlayer(index);
				if (player == null)
					return;
				player.TwitchUser.TimeSinceLastMessage = GameManager.Instance.TimeManager.WorldTimePassed;

				UpdateUserType(player, e);


				// Check if the command has arguments.
				if (e.Command.ArgumentsAsList.Count > 0 && CommandDictionary.CommandsWithArgs.ContainsKey(command))
				{
					string[] argsToLower = e.Command.ArgumentsAsList.ToArray();

					//Lowecase all arguments
					for (int i = 0; i < argsToLower.Length; i++)
					{
						argsToLower[i] = argsToLower[i].ToLower();
					}

					CommandDictionary.CommandsWithArgs[command].Invoke(player, command, argsToLower);
				}
				else if (CommandDictionary.CommandsNoArgs.ContainsKey(command))
				{
					CommandDictionary.CommandsNoArgs[command].Invoke(player);
				}
			}
			// Check if player is trying to create character or call a simple command
			else
			{
				if (CommandDictionary.CreateNameVariants.Contains(command))
					PlayerCommands.TryCreatePlayer(e);
			}
		}

		public static void ProcessMessage(OnMessageReceivedArgs e)
		{
			if (GameManager.Instance.PlayerManager.PlayerExistsByID(e.ChatMessage.UserId, out int index))
			{
				Player player = GameManager.Instance.PlayerManager.GetPlayer(index);
				player.TwitchUser.TimeSinceLastMessage = GameManager.Instance.TimeManager.WorldTimePassed;
			}
			// Check for event
			if (EventCommands.EventMessage(e))
				return;
		}

		private static void UpdateUserType(Player player, OnChatCommandReceivedArgs e)
		{
			player.TwitchUser.TwitchUserType = e.Command.ChatMessage.UserType;

			if (GameMasterCommands.IsGameMaster(player))
			{
				player.TwitchUser.GameUserType = Utils.GameUserType.GameMaster;
			}
			else
			{
				switch (player.TwitchUser.TwitchUserType)
				{
					case TwitchLib.Client.Enums.UserType.Viewer:
						player.TwitchUser.GameUserType = Utils.GameUserType.Normal;
						break;
					case TwitchLib.Client.Enums.UserType.Moderator:
						player.TwitchUser.GameUserType = Utils.GameUserType.Moderator;
						break;
					case TwitchLib.Client.Enums.UserType.Broadcaster:
						player.TwitchUser.GameUserType = Utils.GameUserType.Broadcaster;
						break;
					default:
						player.TwitchUser.GameUserType = Utils.GameUserType.Normal;
						break;
				}
			}
			player.UnitTextDisplay.SetTextColor(Utils.UserColours.GetColourByUserType(player.TwitchUser.GameUserType));
		}
	}
}