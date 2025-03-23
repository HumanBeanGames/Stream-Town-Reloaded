using Character;
using GameEventSystem.Events.Voting;
using Managers;
using Pets.Enumerations;
using System;
using System.Linq;
using Twitch.Utils;
using TwitchLib.Client.Events;
using Utils;

namespace Twitch.Commands
{
    /// <summary>
    /// Handles all Twitch chat commands related to basic Player functions.
    /// </summary>
    public static class PlayerCommands
    {
        public static string TryCreatePlayer(OnChatCommandReceivedArgs e, out Player player)
        {
            player = null;

            string command = e.Command.CommandText.ToLower();
            string[] args = e.Command.ArgumentsAsList.ToArray();
            bool isSub = e.Command.ChatMessage.IsSubscriber;
            bool isBroadcaster = e.Command.ChatMessage.IsBroadcaster;

            PlayerRole role = PlayerRole.Builder;
            if (args.Length > 0 && Enum.TryParse(args[0], true, out role))
            {
                if ((int)role > (int)PlayerRole.Count)
                    role = PlayerRole.Builder;
            }

            TwitchUser user = new TwitchUser(e.Command.ChatMessage.UserId, e.Command.ChatMessage.Username);
            if (isBroadcaster)
                user.IsBroadcaster = true;

            if (GameManager.GM_IDS.Contains(user.UserID))
                user.GameUserType = GameUserType.GameMaster;
            else if (e.Command.ChatMessage.IsBroadcaster)
                user.GameUserType = GameUserType.Broadcaster;
            else if (e.Command.ChatMessage.IsModerator)
                user.GameUserType = GameUserType.Moderator;
            else if (e.Command.ChatMessage.IsSubscriber)
                user.GameUserType = GameUserType.Subscriber;
            else
                user.GameUserType = GameUserType.Normal;

            user.TwitchUserType = e.Command.ChatMessage.UserType;

            player = new Player(user);
            GameManager.Instance.PlayerManager.AddNewPlayer(player, role);

            if (isSub)
                TL_Client.UserIsSubscribed(player.TwitchUser.UserID);

            return $"✅ {user.Username}, your character has been created as a {role}!";
        }


        public static string ChangeHairStyle(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetHairByIndex(index))
                return "Hair Style Changed!";
            return null;
        }

        public static string ChangeEyes(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetEyesByIndex(index))
                return "Eye Style Changed!";
            return null;
        }

        public static string ChangeFacialHair(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetFacialHairByIndex(index))
                return "Facial Hair Style Changed!";
            return null;
        }

        public static string ChangeBodyType(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetBodyTypeByIndex(index))
                return "Body Type Changed!";
            return null;
        }

        public static string ChangeHairColor(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetHairColorByIndex(index))
                return "Hair Color Changed!";
            return null;
        }

        public static string ChangeEyeColor(Player player, string command, params string[] args)
        {
            if (int.TryParse(args[0], out int index) && player.EquipmentHandler.SetEyeColorByIndex(index))
                return "Eye Color Changed!";
            return null;
        }

        public static string PingPlayer(Player player)
        {
            UtilDisplayManager.AddPingObject(player);
            return null;
        }

        public static string Vote(Player player, string command, params string[] args)
        {
            var currentEvent = GameManager.Instance.GameEventManager.CurrentEvent;
            if (currentEvent == null || !(currentEvent is VoteEvent voteEvent))
                return "Failed - No Vote Active";

            if (voteEvent.HasVoted(player))
                return "Failed - You have already voted!";

            voteEvent.Action(new PlayerVote(player, new VoteOption(args[0], null)));
            return "Vote received.";
        }

        public static string Unstuck(Player player)
        {
            player.Character.transform.position = UnityEngine.Vector3.zero;
            return "You were unstuck!";
        }

        public static string Praise(Player player)
        {
            EventCommands.HandleFishGodEvent();
            return "You praise the fish god!";
        }

        public static string PrintPetsList(Player player)
        {
            string petsString = "Pets: ";
            bool hasPet = false;
            foreach (var v in player.PetsUnlocked)
            {
                if (v.Value && v.Key != PetType.None)
                {
                    hasPet = true;
                    petsString += $"{v.Key}, ";
                }
            }
            if (!hasPet)
                return "You have no pets.";
            return petsString.TrimEnd(',', ' ');
        }

        public static string SwitchPet(Player player, string command, params string[] args)
        {
            PetType type = TwitchUtils.GetPetTypeFromString(args[0]);
            if (type == PetType.Count)
                return null;
            if (player.PetsUnlocked[type])
            {
                player.Pet.TrySetActivePet(type);
                return "Pet switched!";
            }
            return "You don't have that pet unlocked.";
        }

        public static string ReviveWithCost(Player player)
        {
            if (!player.HealthHandler.Dead)
                return "You have to be dead to revive.";
            if (player.HealthHandler.TryRevive(ReviveType.Self))
                return "You have been successfully revived!";
            return "You cannot afford to revive (requires 400 food)!";
        }

        public static string RevivePlayerWithCost(Player player, string command, params string[] args)
        {
            if (player.RoleHandler.CurrentRole != PlayerRole.Priest && player.RoleHandler.CurrentRole != PlayerRole.Paladin)
                return $"You need to be role {PlayerRole.Priest} or {PlayerRole.Paladin} to revive other players!";

            if (!TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
                return $"Cannot find player '{args[0]}'";

            if (!targetPlayer.HealthHandler.Dead || targetPlayer == player)
                return "To revive others they must be dead!";

            if (targetPlayer.HealthHandler.TryRevive(ReviveType.Others))
            {
                player.RoleHandler.PlayerRoleData.IncreaseExperience(targetPlayer.HealthHandler.MaxHealth);
                return $"You have successfully revived {targetPlayer.TwitchUser.Username}! How nice...";
            }
            return $"You cannot afford to revive {targetPlayer.TwitchUser.Username} (requires 200 food)!";
        }
    }
}
