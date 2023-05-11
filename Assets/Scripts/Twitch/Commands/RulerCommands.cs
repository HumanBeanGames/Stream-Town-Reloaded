using Character;
using Managers;
using System;
using Twitch.Utils;
using UnityEngine;
using Utils;

namespace Twitch.Commands
{
	public static class RulerCommands
	{
		private static PlayerManager _playerManager;
		public static PlayerManager PlayerManager
		{
			get
			{
				if (_playerManager == null)
					_playerManager = GameManager.Instance.PlayerManager;

				return _playerManager;
			}
		}

		public static RoleManager _roleManager;

		public static RoleManager RoleManager
		{
			get
			{
				if (_roleManager == null)
					_roleManager = GameManager.Instance.RoleManager;

				return _roleManager;
			}
		}
		/// <summary>
		/// Used for allowing Rulers to sell resources for gold.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void SellResource(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			if (args.Length < 2)
				return;

			Resource resource = TwitchUtils.GetResourceFromString(args[1]);

			if (resource == Resource.Gold || resource == Resource.None)
				return;

			if (int.TryParse(args[0], out int amount))
			{
				if (amount <= 0)
					return;

				TradeHandler.SellResource(resource, amount, out string message);

				MessageSender.SendMessage($"{player.TwitchUser.Username} : {message}");
			}
		}

		/// <summary>
		/// Used for allowing Rulers to Purchase Resources for gold.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public static void BuyResource(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			if (args.Length < 2)
				return;

			Resource resource = TwitchUtils.GetResourceFromString(args[1]);


			if (resource == Resource.Gold || resource == Resource.None)
				return;

			if (int.TryParse(args[0], out int amount))
			{
				TradeHandler.BuyResource(resource, amount, out string message);

				MessageSender.SendMessage($"{player.TwitchUser.Username} : {message}");
			}
		}

		public static void RecruitNPC(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			if (args.Length < 1)
				return;
			string r = char.ToUpper(args[0][0]) + args[0].Substring(1);
			if (Enum.TryParse(r, out PlayerRole role))
			{
				if (role == PlayerRole.Ruler)
					return;

				int amount = 1;

				if (args.Length >= 2)
					int.TryParse(args[1], out amount);

				for (int i = 0; i < amount; i++)
				{
					if (RoleManager.SlotsFull(role) || GameManager.Instance.TownResourceManager.ResourceFull(Resource.Recruit))
						break;

					Player recruit = new Player(new TwitchUser($"{UnityEngine.Random.Range(int.MinValue, 0)}", $""), true);
					PlayerManager.AddNewPlayer(recruit, role);
					GameManager.Instance.TownResourceManager.AddResource(Resource.Recruit, 1);
				}
			}
		}

		public static void ResetCamera(Player player)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			GameManager.Instance.CameraController.ResetCamera();
		}

		public static void MoveCamera(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			if (args.Length < 1)
				return;

			Vector3 moveVector = Vector3.zero;
			int zoomFactor = 0;

			for (int i = 0; i < args.Length; i += 2)
			{
				int value = 0;

				if (!((i + 1) < args.Length && int.TryParse(args[i + 1], out value)))
					value = 1;

				switch (args[i])
				{
					case "up":
						moveVector += Vector3.right * value;
						break;
					case "down":
						moveVector += Vector3.left * value;
						break;
					case "left":
						moveVector += Vector3.forward * value;
						break;
					case "right":
						moveVector += Vector3.back * value;
						break;
					case "out":
						zoomFactor = 1 * value;
						break;
					case "in":
						zoomFactor = -1 * value;
						break;
				}
			}
			GameManager.Instance.CameraController.ZoomCamera(zoomFactor);
			GameManager.Instance.CameraController.MoveCamera(moveVector);
		}

		public static void RecruitCount(Player player)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;
			MessageSender.SendMessage($"{player.TwitchUser.Username} The town has {GameManager.Instance.PlayerManager.RecruitCount()} recruits!");
		}

		// show recruit ids
		public static void ShowRecruitIds(Player player)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;
			GameManager.Instance.PlayerManager.ShowRecruitIDs();
		}

		// Dismiss recruit
		public static void DismissRecruit(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;
			if (args.Length == 0)
				MessageSender.SendMessage($"!rdismiss <id>");
			if (int.TryParse(args[0], out int id))
			{
				Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);

				GameManager.Instance.PlayerManager.DismissRecruit(recruit);
				MessageSender.SendMessage($"{player.TwitchUser.Username} Successfully Dismissed recruit {id}!");
			}
			else
				MessageSender.SendMessage($"{args[0]} is not a valid id");
		}

		// swap recruit role
		public static void SwapRecruitRole(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;
			if (args.Length == 0)
				MessageSender.SendMessage("!rswap <id> <role>");
			if (args.Length > 0)
			{
				if (int.TryParse(args[0], out int id))
				{
					Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);
					string r = char.ToUpper(args[1][0]) + args[1].Substring(1);
					if (Enum.TryParse(r, out PlayerRole role))
					{
						if (GameManager.Instance.RoleManager.IsRoleAvailabe(role))
						{
							GameManager.Instance.PlayerManager.SwapRecruitRole(recruit, role);
							MessageSender.SendMessage($"{player.TwitchUser.Username} Successfully changed recruit {id} to {role}!");
						}
						else
							MessageSender.SendMessage($"{role} is full");
					}
					else
						MessageSender.SendMessage($"{args[1]} is not a valid role");
				}
				else
					MessageSender.SendMessage($"{args[0]} is not a valid id");
			}
		}


		// show recruit information (role, level) using ID
		public static void DisplayRecruitInfo(Player player, string command, params string[] args)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;
			if (args.Length == 0)
				MessageSender.SendMessage("!rinfo <id> ");
			if (args.Length > 0)
			{
				if (int.TryParse(args[0], out int id))
				{
					Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);

					string info = $"{player.TwitchUser.Username} ----- Recruit {id} | " +
						$"Current role {recruit.RoleHandler.CurrentRole} | " +
						$" Health: {recruit.HealthHandler.Health} / {recruit.HealthHandler.MaxHealth} | " +
						$" Level: {recruit.RoleHandler.PlayerRoleData.CurrentLevel} / {RoleManager.MAX_ROLE_LEVEl} | " +
						$" Experience: {recruit.RoleHandler.PlayerRoleData.CurrentExp} / {recruit.RoleHandler.PlayerRoleData.RequiredExp}";

					MessageSender.SendMessage(info);
				}
				else
					MessageSender.SendMessage($"{args[0]} is not a valid id");
			}

		}

		public static void Resign(Player player)
		{
			if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player))
				return;

			GameManager.Instance.GameEventManager.StartNewRulerVote();
			player.RoleHandler.TrySetRole(player.RoleHandler.PreviousRole);
			GameManager.Instance.PlayerManager.SetRuler(null);
			MessageSender.SendMessage($"{player.TwitchUser.Username} you have been succesfully resigned!");
		}

	}
}