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
        public static PlayerManager PlayerManager => _playerManager ??= GameManager.Instance.PlayerManager;

        private static RoleManager _roleManager;
        public static RoleManager RoleManager => _roleManager ??= GameManager.Instance.RoleManager;

        public static string SellResource(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "You are not authorized to sell resources.";
            if (args.Length < 2) return "Usage: !rsell <amount> <resource>";

            Resource resource = TwitchUtils.GetResourceFromString(args[1]);
            if (resource == Resource.Gold || resource == Resource.None) return "Invalid resource type.";
            if (int.TryParse(args[0], out int amount) && amount > 0)
            {
                TradeHandler.SellResource(resource, amount, out string message);
                return $"{player.TwitchUser.Username}: {message}";
            }
            return "Invalid amount.";
        }

        public static string BuyResource(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "You are not authorized to buy resources.";
            if (args.Length < 2) return "Usage: !rbuy <amount> <resource>";

            Resource resource = TwitchUtils.GetResourceFromString(args[1]);
            if (resource == Resource.Gold || resource == Resource.None) return "Invalid resource type.";
            if (int.TryParse(args[0], out int amount) && amount > 0)
            {
                TradeHandler.BuyResource(resource, amount, out string message);
                return $"{player.TwitchUser.Username}: {message}";
            }
            return "Invalid amount.";
        }

        public static string RecruitNPC(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "You are not authorized to recruit.";
            if (args.Length < 1) return "Usage: !rrecruit <role> [amount]";

            string r = char.ToUpper(args[0][0]) + args[0].Substring(1);
            if (!Enum.TryParse(r, out PlayerRole role)) return "Invalid role.";
            if (role == PlayerRole.Ruler) return "You cannot recruit a Ruler.";

            int amount = 1;
            if (args.Length >= 2) int.TryParse(args[1], out amount);

            int successCount = 0;
            for (int i = 0; i < amount; i++)
            {
                if (RoleManager.SlotsFull(role) || TownResourceManager.ResourceFull(Resource.Recruit)) break;
                Player recruit = new Player(new TwitchUser($"{UnityEngine.Random.Range(int.MinValue, 0)}", $""), true);
                PlayerManager.AddNewPlayer(recruit, role);
                TownResourceManager.AddResource(Resource.Recruit, 1);
                successCount++;
            }
            return successCount > 0 ? $"Recruited {successCount} {role}(s)." : "Failed to recruit any NPCs.";
        }

        public static string ResetCamera(Player player)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            GameManager.Instance.CameraController.ResetCamera();
            return "Camera reset.";
        }

        public static string MoveCamera(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            if (args.Length < 1) return "Usage: !rcam <direction> [amount]";

            Vector3 moveVector = Vector3.zero;
            int zoomFactor = 0;

            for (int i = 0; i < args.Length; i += 2)
            {
                int value = ((i + 1) < args.Length && int.TryParse(args[i + 1], out int v)) ? v : 1;
                switch (args[i])
                {
                    case "up": moveVector += Vector3.right * value; break;
                    case "down": moveVector += Vector3.left * value; break;
                    case "left": moveVector += Vector3.forward * value; break;
                    case "right": moveVector += Vector3.back * value; break;
                    case "out": zoomFactor += value; break;
                    case "in": zoomFactor -= value; break;
                }
            }
            GameManager.Instance.CameraController.ZoomCamera(zoomFactor);
            GameManager.Instance.CameraController.MoveCamera(moveVector);
            return "Camera moved.";
        }

        public static string RecruitCount(Player player)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            return $"{player.TwitchUser.Username} The town has {GameManager.Instance.PlayerManager.RecruitCount()} recruits!";
        }

        public static string ShowRecruitIds(Player player)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            GameManager.Instance.PlayerManager.ShowRecruitIDs();
            return "Recruit IDs displayed.";
        }

        public static string DismissRecruit(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            if (args.Length == 0) return "Usage: !rdismiss <id>";
            if (int.TryParse(args[0], out int id))
            {
                Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);
                GameManager.Instance.PlayerManager.DismissRecruit(recruit);
                return $"{player.TwitchUser.Username} Successfully Dismissed recruit {id}!";
            }
            return $"{args[0]} is not a valid id";
        }

        public static string SwapRecruitRole(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            if (args.Length < 2) return "Usage: !rswap <id> <role>";
            if (int.TryParse(args[0], out int id))
            {
                Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);
                string r = char.ToUpper(args[1][0]) + args[1].Substring(1);
                if (Enum.TryParse(r, out PlayerRole role))
                {
                    if (GameManager.Instance.RoleManager.IsRoleAvailabe(role))
                    {
                        GameManager.Instance.PlayerManager.SwapRecruitRole(recruit, role);
                        return $"{player.TwitchUser.Username} Successfully changed recruit {id} to {role}!";
                    }
                    return $"{role} is full";
                }
                return $"{args[1]} is not a valid role";
            }
            return $"{args[0]} is not a valid id";
        }

        public static string DisplayRecruitInfo(Player player, string command, params string[] args)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            if (args.Length == 0) return "Usage: !rinfo <id>";
            if (int.TryParse(args[0], out int id))
            {
                Player recruit = GameManager.Instance.PlayerManager.GetRecruitByIndex(id);
                return $"{player.TwitchUser.Username} ----- Recruit {id} | Current role {recruit.RoleHandler.CurrentRole} | Health: {recruit.HealthHandler.Health}/{recruit.HealthHandler.MaxHealth} | Level: {recruit.RoleHandler.PlayerRoleData.CurrentLevel}/{RoleManager.MAX_ROLE_LEVEl} | Experience: {recruit.RoleHandler.PlayerRoleData.CurrentExp}/{recruit.RoleHandler.PlayerRoleData.RequiredExp}";
            }
            return $"{args[0]} is not a valid id";
        }

        public static string Resign(Player player)
        {
            if (PlayerManager.Ruler != player && !GameMasterCommands.IsGameMaster(player)) return "Unauthorized.";
            GameManager.Instance.GameEventManager.StartNewRulerVote();
            if (player.RoleHandler.TrySetRole(player.RoleHandler.PreviousRole, out string reason))
            {
                GameManager.Instance.PlayerManager.SetRuler(null);
                return $"{player.TwitchUser.Username} you have been successfully resigned!";
            }
            else
            {
                return $"Could not resign: {reason}";
            }
        }
    }
}