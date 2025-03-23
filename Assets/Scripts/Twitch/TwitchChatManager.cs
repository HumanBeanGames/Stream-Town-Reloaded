using TwitchLib.Client.Events;
using Twitch.Commands;
using Character;
using UnityEngine;
using Managers;
using static UnityEngine.Rendering.GPUSort;

namespace Twitch
{
    public static class TwitchChatManager
    {
        public static bool TwitchGameConnected = false;
        private static string _expectedConnectCode = null;

        public static void SetExpectedConnectCode(string code)
        {
            _expectedConnectCode = code;
            Debug.Log($"[TwitchChatManager] Expected connect code set to: {_expectedConnectCode}");
        }

        public static void ProcessMessage(OnMessageReceivedArgs e)
        {
            Debug.Log($"[Twitch] Message from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        public static void ProcessCommand(OnChatCommandReceivedArgs e)
        {
            if (!GameStateManager.PlayerReady)
                return;

            string command = e.Command.CommandText.ToLower();
            string[] args = e.Command.ArgumentsAsList.ToArray();

            string response = "";
            if (TwitchGameConnected)
            {
                response =
                    TryHandleSimpleCommand(command)
                    ?? TryHandleConnectionCommand(command, args, e)
                    ?? TryHandlePlayerCommand(command, args, e)
                    ?? TryHandleCreatePlayerCommand(command, e)
                    ?? $"❌ Unknown command: !{command}";
            }
            else
            {
                response = TryHandleSimpleCommand(command)
                    ?? TryHandleConnectionCommand(command, args, e)
                    ?? $"❌ Unknown command: !{command}";

            }

            if (!string.IsNullOrEmpty(response))
                MessageSender.SendMessage(response);
        }

        private static string TryHandleSimpleCommand(string command)
        {
            return CommandDictionary.SimpleCommands.TryGetValue(command, out var simpleCmd)
                ? simpleCmd?.Invoke()
                : null;
        }

        private static string TryHandleConnectionCommand(string command, string[] args, OnChatCommandReceivedArgs e)
        {
            if (command != "connect") return null;

            if (!e.Command.ChatMessage.IsBroadcaster)
                return "Only the broadcaster can initiate connection.";
            if (TwitchGameConnected)
                return "Game is already connected.";
            if (_expectedConnectCode != null && e.Command.ArgumentsAsString.Trim() != _expectedConnectCode)
                return "❌ Incorrect connection code.";
            if (args.Length == 0)
                return "Usage: !connect <code>";

            BroadcasterCommands.Connect(args[0], e);
            TwitchGameConnected = true;
            return "✅ Game successfully connected!";
        }

        private static string TryHandlePlayerCommand(string command, string[] args, OnChatCommandReceivedArgs e)
        {
            if (!GameManager.Instance.PlayerManager.PlayerExistsByID(e.Command.ChatMessage.UserId, out int index))
                return null;

            var player = GameManager.Instance.PlayerManager.GetPlayer(index);
            if (player == null)
                return null;

            player.TwitchUser.TimeSinceLastMessage = GameManager.Instance.TimeManager.WorldTimePassed;
            UpdateUserType(player, e);

            if (args.Length > 0 && CommandDictionary.CommandsWithArgs.TryGetValue(command, out var withArgsCmd))
            {
                for (int i = 0; i < args.Length; i++)
                    args[i] = args[i].ToLower();

                return withArgsCmd.Invoke(player, command, args);
            }

            if (CommandDictionary.CommandsNoArgs.TryGetValue(command, out var noArgsCmd))
                return noArgsCmd.Invoke(player);

            return null;
        }

        private static string TryHandleCreatePlayerCommand(string command, OnChatCommandReceivedArgs e)
        {
            if (!CommandDictionary.CreateNameVariants.Contains(command))
                return null;

            return PlayerCommands.TryCreatePlayer(e, out _);
        }


        private static void UpdateUserType(Player player, OnChatCommandReceivedArgs e)
        {
            // Optional: Enhance user-type tracking (mod, vip, etc.)
        }
    }
}
