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
		/// <summary>
		/// Attempts to create a player and will set it's role if provided in the arguments.
		/// </summary>
		/// <param name="e"></param>
		public static Player TryCreatePlayer(OnChatCommandReceivedArgs e)
		{
			string command = e.Command.CommandText.ToLower();
			string[] args = e.Command.ArgumentsAsList.ToArray();
			bool isSub = e.Command.ChatMessage.IsSubscriber;
			bool isBroadcaster = e.Command.ChatMessage.IsBroadcaster;

			// Default role to builder.
			PlayerRole role = PlayerRole.Builder;

			// Check if the user has picked a role to start as.
			if (args.Length > 0)
			{
				if (Enum.TryParse(args[0], true, out role))
					if ((int)role > (int)PlayerRole.Count)
						role = PlayerRole.Builder;
			}

			// Create the Twitch user data.
			TwitchUser user = new TwitchUser(e.Command.ChatMessage.UserId, e.Command.ChatMessage.Username);
			if(isBroadcaster)
				user.IsBroadcaster = true;

			// Assign the user their twitch role (Broadcaster, Moderator, etc)
			// If the user is a GameMaster, they will be given that role.
			if (GameManager.GM_IDS.Contains(user.UserID))
			{
				user.GameUserType = GameUserType.GameMaster;
			}
			else if (e.Command.ChatMessage.IsBroadcaster)
			{
				user.GameUserType = GameUserType.Broadcaster;
			}
			else if (e.Command.ChatMessage.IsModerator)
			{
				user.GameUserType = GameUserType.Moderator;
			}
			else if (e.Command.ChatMessage.IsSubscriber)
			{
				user.GameUserType = GameUserType.Subscriber;
			}
			else
			{
				user.GameUserType = GameUserType.Normal;
			}

			user.TwitchUserType = e.Command.ChatMessage.UserType;

			Player player = new Player(user);

			GameManager.Instance.PlayerManager.AddNewPlayer(player, role);

			if (isSub)
				TL_Client.UserIsSubscribed(player.TwitchUser.UserID);

			MessageSender.SendPreBuiltMessage(user.Username, "characterCreated");
			return player;
		}

		/// <summary>
		/// Changes the user's Hair Style.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ChangeHairStyle(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetHairByIndex(index))
					MessageSender.SendPlayerMessage(player, "Hair Style Changed!");
			}
		}

		/// <summary>
		/// Changes the user's Eye Type.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ChangeEyes(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetEyesByIndex(index))
					MessageSender.SendPlayerMessage(player, "Eye Style Changed!");
			}
		}

		/// <summary>
		/// Changes the user's Facial Hair style.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// param name="args"></param>
		public static void ChangeFacialHair(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetFacialHairByIndex(index))
					MessageSender.SendPlayerMessage(player, "Facial Hair Style Changed!");
			}
		}

		/// <summary>
		/// Changes the user's body type.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ChangeBodyType(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetBodyTypeByIndex(index))
					MessageSender.SendPlayerMessage(player, "Body Type Changed!");
			}
		}

		/// <summary>
		/// Changes the user's hair color.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ChangeHairColor(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetHairColorByIndex(index))
					MessageSender.SendPlayerMessage(player, "Hair Color Changed!");
			}
		}

		/// <summary>
		/// Changes the user's eye color.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void ChangeEyeColor(Player player, string command, params string[] args)
		{
			if (int.TryParse(args[0], out int index))
			{
				if (player.EquipmentHandler.SetEyeColorByIndex(index))
					MessageSender.SendPlayerMessage(player, "Eye Color Changed!");
			}
		}

		public static void PingPlayer(Player player)
		{
			UtilDisplayManager.AddPingObject(player);
		}

		public static void Vote(Player player, string command, params string[] args)
		{
			var currentEvent = GameManager.Instance.GameEventManager.CurrentEvent;

			if (currentEvent == null)
			{
				MessageSender.SendPlayerMessage(player, "Failed - No Vote Active");
				return;
			}
			Type typeOf = typeof(VoteEvent);

			if (!currentEvent.GetType().IsSubclassOf(typeof(VoteEvent)) && currentEvent.GetType() != typeOf)
			{
				MessageSender.SendPlayerMessage(player, "Failed - No Vote Active");
				return;
			}

			VoteEvent voteEvent = (VoteEvent)currentEvent;

			if (voteEvent.HasVoted(player))
			{
				MessageSender.SendPlayerMessage(player, "Failed - You have already voted!");
				return;
			}

			voteEvent.Action(new PlayerVote(player, new VoteOption(args[0], null)));
		}

		public static void Unstuck(Player player)
		{
			player.Character.transform.position = UnityEngine.Vector3.zero;
		}

		public static void Praise(Player player)
		{
			EventCommands.HandleFishGodEvent();
		}

		public static void PrintPetsList(Player player)
		{
			string petsString = "Pets: ";
			bool hasPet = false;
			foreach (var v in player.PetsUnlocked)
			{
				if (v.Value)
				{
					if (v.Key != PetType.None)
					{
						hasPet = true;
						petsString += $"{v.Key}, ";
					}
				}
			}

			if (!hasPet)
				petsString = "You have no pets";

			MessageSender.SendMessage($"{player.TwitchUser.Username} {petsString}");
		}

		public static void SwitchPet(Player player, string command, params string[] args)
		{
			PetType type = TwitchUtils.GetPetTypeFromString(args[0]);

			if (type == PetType.Count)
				return;

			if (player.PetsUnlocked[type])
			{
				player.Pet.TrySetActivePet(type);
				MessageSender.SendMessage($"{player.TwitchUser.Username} pet switched!");
			}
		}

		public static void ReviveWithCost(Player player)
		{
			if (player.HealthHandler.Dead)
				if (player.HealthHandler.TryRevive(ReviveType.Self))
					MessageSender.SendMessage($"{player.TwitchUser.Username} you have been successfully revived!");
				else
					MessageSender.SendMessage($"{player.TwitchUser.Username} you cannot afford to revive (requires 400 food)!");
			else
				MessageSender.SendMessage($"{player.TwitchUser.Username} you have to be dead to revive");
		}

		public static void RevivePlayerWithCost(Player player, string command, params string[] args)
		{
			if (player.RoleHandler.CurrentRole == PlayerRole.Priest || player.RoleHandler.CurrentRole == PlayerRole.Paladin)
				if (Utils.TwitchUtils.TryGetPlayer(args[0], out Player targetPlayer))
					if (targetPlayer.HealthHandler.Dead && targetPlayer != player)
						if (targetPlayer.HealthHandler.TryRevive(ReviveType.Others))
						{
							player.RoleHandler.PlayerRoleData.IncreaseExperience(targetPlayer.HealthHandler.MaxHealth);
							MessageSender.SendMessage($"{player.TwitchUser.Username} you have successfully revived {targetPlayer.TwitchUser.Username}! how nice...");
						}
						else
							MessageSender.SendMessage($"{player.TwitchUser.Username} you cannot afford to revive {targetPlayer.TwitchUser.Username} (requires 200 food)!");
					else
						MessageSender.SendMessage($"{player.TwitchUser.Username} to revive others they must be dead! silly.");
				else
					MessageSender.SendMessage($"{player.TwitchUser.Username} cannot find player '{targetPlayer.TwitchUser.Username}'");
			else
				MessageSender.SendMessage($"{player.TwitchUser.Username} you need to be role {PlayerRole.Priest} or {PlayerRole.Paladin} to revive other players!");
		}
	}
}