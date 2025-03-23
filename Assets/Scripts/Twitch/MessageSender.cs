using Character;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch
{
    /// <summary>
    /// Handles sending messages from the bot to Twitch chat.
    /// </summary>
    public static class MessageSender
    {
        public static bool MessagesAllowed = false;

        public static readonly Dictionary<string, string> CommandResponses = new Dictionary<string, string>
        {
            { "help", " type !create to start your character, then you can choose a role. type !roles to learn more" },
            { "characterCreated", "Welcome to the game, your character was successfully created!"},
            { "characterFailed", "Character already registered into the game!"},
            { "buildingFailedCost",  "Not enough resources to build that!"},
            { "buildingLevelFailedCost", "Not enough resources to upgrade that building!" },
            { "buildingLevelFailedMaxLevel" , "That building is already at max level!" },
            { "buildingLevelFailed", " Building level failed!" },
            { "buildingLevelSuccess" , "Successfully upgraded building!"},
            { "buildingMultiLevelSuccess" , "Successfully upgraded buildings!"},
            { "buildingCancel" , "Building was canceled!" },
            { "buildingSuccessful", "Building was placed successfully!" },
            { "buildingRemoved", " Building was removed!" },
            { "buildingFailedCollision",  "Building can't be placed there!"},
            { "playerUnstuck", "You were unstuck!" },
            { "playerRevived", " You have revived, welcome back to life!" },
            { "roleSwitched" , " Role switched successfully!" },
            { "noCharacter"," You need to create a character first!" },
            { "discord","Stream Town Discord: https://discord.gg/By4jvks" }
        };

        public static void SendPreBuiltMessage(string playerName, string key)
        {
            if (!CommandResponses.ContainsKey(key))
                return;

            string message = $"{playerName} {CommandResponses[key]}";
            SendMessage(message);
        }

        public static void SendPreBuiltMessage(string key)
        {
            if (!CommandResponses.ContainsKey(key))
                return;

            string message = CommandResponses[key];
            SendMessage(message);
        }

        public static void SendMessage(string playerName, string message)
        {
            SendMessage($"{playerName}: {message}");
        }

        public static void SendPlayerMessage(Player player, string message)
        {
            SendMessage($"{player.TwitchUser.Username}: {message}");
        }

        public static void SendMessage(string message)
        {
            if (!MessagesAllowed)
                return;

            if (TL_Client.Client == null)
            {
                Debug.LogWarning($"[MessageSender] Tried to send message but Client is null. Message was: {message}");
                return;
            }

            if (!TL_Client.Client.IsConnected)
            {
                Debug.LogWarning($"[MessageSender] Tried to send message but Client is not connected. Message was: {message}");
                return;
            }

            if (TL_Client.Client.JoinedChannels.Count == 0)
            {
                Debug.LogWarning($"[MessageSender] Tried to send message but no channels joined yet. Message was: {message}");
                return;
            }

            TL_Client.Client.SendMessage(TL_Client.Client.JoinedChannels[0], message);
        }
    }
}
