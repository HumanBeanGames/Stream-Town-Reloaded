using UnityEngine;
using Character;
using System.Collections.Generic;
using Utils;
using Units;
using Sensors;
using UserInterface;
using System;
using Pets;
using GUIDSystem;
using Utils.Pooling;
using Target;
using System.Linq;

namespace Managers
{
	/// <summary>
	/// Manages all players currently in the game.
	/// </summary>
	public class PlayerManager : MonoBehaviour
	{
		private List<Player> _players = new List<Player>();
		private List<Player> _recruits = new List<Player>();
		private Player _ruler;

		private Dictionary<PlayerRole, StatModifiers> _roleStatModifiers;
		private StatModifiers _globalStatModifier;

		public StatModifiers GlobalStatModifiers => _globalStatModifier;
		public Player Ruler => _ruler;
		public Action<Player> OnRulerChanged;

		private Queue<Player> _playerUpdateQueue;

		public bool CanAddRecruit => _recruits.Count < 200 ? true : false;
		public void ShowRecruitIDs()
		{
			for (int i = 0; i < _recruits.Count; i++)
			{
				UtilDisplayManager.AddTextDisplay(_recruits[i].PlayerTarget, $"{i + 1}");
			}
		}

        public bool PlayerExistsByUserID(string userID)
        {
            return _players.Any(p => p.TwitchUser.UserID == userID) || _recruits.Any(r => r.TwitchUser.UserID == userID);
        }

        public Player GetRecruitByIndex(int index)
		{
			int adjusteIndex = index - 1;
			if (adjusteIndex <= _recruits.Count - 1)
				return _recruits[adjusteIndex];
			else
				return null;
		}

		public void DismissRecruit(Player player)
		{
			if (_recruits.Contains(player))
			{
				_recruits.Remove(player);
				TownResourceManager.RemoveResource(Resource.Recruit, 1);
				RoleManager.TakeFromRole(player.RoleHandler.CurrentRole);
				player.Character.SetActive(false);
			}
		}

		public void SwapRecruitRole(Player player, PlayerRole role)
		{
			if (!player.RoleHandler.TrySetRole(role, out string reason))
				Debug.Log(reason);
		}

		public void Initialize()
		{
			_globalStatModifier = new StatModifiers();
			_roleStatModifiers = new Dictionary<PlayerRole, StatModifiers>();

			for (int i = 0; i < (int)PlayerRole.Count; i++)
			{
				_roleStatModifiers.Add((PlayerRole)i, new StatModifiers());
			}

			_playerUpdateQueue = new Queue<Player>();
		}

		public List<Player> Players => _players;


		/// <summary>
		/// Adds a new player to the game, initializing their character and data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="startingRole"></param>
		public void AddNewPlayer(Player data, PlayerRole startingRole = PlayerRole.Builder)
		{
			if (_players.Contains(data))
				return;

			if (_recruits.Contains(data))
				return;


			// TODO: Optimize this, store it when objects are pooled.
			PoolableObject obj = ObjectPoolingManager.GetPooledObject("Player");
			obj.gameObject.SetActive(true);
			obj.transform.position = GameManager.Instance.PlayerSpawnPosition;
			data.RoleHandler = obj.GetComponent<RoleHandler>();
			data.RoleHandler.Player = data;
			data.StationSensor = obj.GetComponent<StationSensor>();
			data.HealthHandler = obj.GetComponent<HealthHandler>();
			data.HealthHandler.OnDeath += data.OnCharacterDied;
			data.HealthHandler.OnRevived += data.OnCharacterRespawned;
			data.TargetSensor = obj.GetComponent<TargetSensor>();
			data.EquipmentHandler = obj.GetComponent<CharacterModelHandler>();
			data.GUIDComponent = obj.GetComponent<GUIDComponent>();
			data.PlayerTarget = obj.GetComponent<TargetablePlayer>();
			data.RoleHandler.SetStarterRole(startingRole);
			data.Character = obj.gameObject;
			data.StationSensor.Player = data;
			var unitText = obj.GetComponentInChildren<UnitTextDisplay>();

			unitText.gameObject.SetActive(true);
			unitText.SetDisplayText(data.TwitchUser.Username);
			unitText.SetTextColor(Twitch.Utils.UserColours.GetColourByUserType(data.TwitchUser.GameUserType));
			data.UnitTextDisplay = unitText;
			PoolableObject petObj = ObjectPoolingManager.GetPooledObject("Pet");
			Pet pet = petObj.GetComponent<Pet>();

			pet.SetOwner(obj.transform, data);
			petObj.gameObject.SetActive(true);
			_playerUpdateQueue.Enqueue(data);
			data.Pet = pet;

			if (data.TwitchUser.Username == "")
				if (CanAddRecruit)
				{
					_recruits.Add(data);
					TownResourceManager.AddResource(Resource.Recruit, 1);
					data.TwitchUser.GameUserType = Twitch.Utils.GameUserType.Normal;
				}

				else
					return;
			else
				_players.Add(data);
		}

		/// <summary>
		/// Adds an existing player to the game, initializing their character and data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="startingRole"></param>
		public Player AddExistingPlayer(Player data, PlayerRole startingRole = PlayerRole.Builder)
		{
				if (_players.Contains(data))
				return null;

			if (_recruits.Contains(data))
				return null;

			if (data.TwitchUser.Username == "")
				if (CanAddRecruit)
				{
					_recruits.Add(data);
					TownResourceManager.AddResource(Resource.Recruit, 1);
				}
				else
					return null;
			else
				_players.Add(data);

			data.StationSensor.Player = data;
			var unitText = data.Character.GetComponentInChildren<UnitTextDisplay>();
			unitText.SetDisplayText(data.TwitchUser.Username);
			unitText.SetTextColor(Twitch.Utils.UserColours.GetColourByUserType(data.TwitchUser.GameUserType));
			data.UnitTextDisplay = unitText;
			_playerUpdateQueue.Enqueue(data);
			return data;
		}

		/// <summary>
		/// Removes a player from the game.
		/// </summary>
		/// <param name="data"></param>
		public void RemovePlayer(Player data)
		{
			if (!_players.Contains(data))
				return;

			_players.Remove(data);
		}

		/// <summary>
		/// Gets a Player by index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Player</returns>
		public Player GetPlayer(int index)
		{
			if (_players.Count <= index)
				return null;

			return _players[index];
		}

		/// <summary>
		/// Checks if a player exists.
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Player</returns>
		public bool PlayerExists(Player data)
		{
			return _players.Contains(data);
		}

		public void SetRuler(Player player)
		{
			if (_ruler != null)
				if (!_ruler.RoleHandler.TrySetRole(_ruler.RoleHandler.PreviousRole, out string reason))
					Debug.Log(reason);

			_ruler = player;
			OnRulerChanged?.Invoke(player);
		}

		/// <summary>
		/// Checks if a player with the specified userID eixsts and outputs an index.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="index"></param>
		/// <returns>bool</returns>
		public bool PlayerExistsByID(string userID, out int index)
		{
			index = -1;

			for (int i = 0; i < _players.Count; i++)
			{
				if (_players[i].TwitchUser.UserID == userID)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		public bool PlayerExistsByNameToLower(string playerName, out int index)
		{
			index = -1;

			for (int i = 0; i < _players.Count; i++)
			{
				if (_players[i].TwitchUser.Username.ToLower() == playerName)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the current number of Players.
		/// </summary>
		/// <returns></returns>
		public int PlayerCount()
		{
			return _players.Count;
		}

		public int RecruitCount()
		{
			return _recruits.Count;
		}

		/// <summary>
		/// Returns the Player's Twitch name by index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetPlayerTwitchName(int index)
		{
			if (_players.Count <= index)
				return "";

			return _players[index].TwitchUser.Username;
		}

		public StatModifiers GetStatModifiers(PlayerRole role)
		{
			return _roleStatModifiers[role];
		}

		private void Update()
		{
			if (_playerUpdateQueue.Count > 0)
			{
				var playerToUpdate = _playerUpdateQueue.Dequeue();

				playerToUpdate.TwitchUser.UpdateActivity();

				_playerUpdateQueue.Enqueue(playerToUpdate);
			}
		}
	}
}