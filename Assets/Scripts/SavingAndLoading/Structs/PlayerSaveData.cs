using Character;
using UnityEngine;
using System.Collections.Generic;
using Utils;
using Twitch;
using Managers;
using Units;
using Sensors;
using Twitch.Utils;
using GUIDSystem;
using Buildings;
using Target;
using Pets.Enumerations;
using Pets;
using Utils.Pooling;
using TwitchLib.Client.Enums;
using Twitch.Commands;

namespace SavingAndLoading.Structs
{
	/// <summary>
	/// Struct holding information on the player, role id, etc
	/// </summary>
	[System.Serializable]
	public struct PlayerSaveData
	{
		public string TwitchID;
		public string TwitchName;
		public UserType TwitchUserType;
		public GameUserType GameUserType;
		public bool IsBroadcaster;
		public uint GUID;
		public uint TargetGUID;
		public uint StationGUID;

		public bool PetActive;
		public PetType CurrentPet;
		public List<PetType> UnlockedPets;

		public TransformSaveData Transform;
		public PlayerRole CurrentRole;
		public PlayerRole PreviousRole;

		public List<PlayerRoleSaveData> Roles;
		public InventorySaveData Inventory;

		public PlayerCustomizationSaveData Customization;

		public int Health;
		public bool RegenRequiresFood;

		// Current Target,
		// Current work

		/// <summary>
		/// Overloaded Constructor,
		/// </summary>
		/// <param name="twitchID">The players TwitchID</param>
		/// <param name="twitchName">The players Twitch username</param>
		/// <param name="currentRole">The players current role</param>
		/// <param name="previousRole">The players previouse role</param>
		/// <param name="roles">All the players roles</param>
		/// <param name="playerInventory">The players inventory</param>
		/// <param name="health">The players health</param>
		/// <param name="regenRequiresFood">Does the player require food to regen</param>
		public PlayerSaveData(string twitchID, string twitchName, UserType twitchUserType, GameUserType gameUserType,bool isBroadcaster, uint gUID, bool petActive, PetType currentPet, List<PetType> unlockedPets, TransformSaveData transform, PlayerRole currentRole, PlayerRole previousRole, List<PlayerRoleSaveData> roles, InventorySaveData playerInventory, PlayerCustomizationSaveData customization, int health, bool regenRequiresFood)
		{
			TwitchID = twitchID;
			TwitchName = twitchName;
			TwitchUserType = twitchUserType;
			GameUserType = gameUserType;
			IsBroadcaster = isBroadcaster;
			GUID = gUID;
			TargetGUID = 0;
			StationGUID = 0;

			PetActive = petActive;
			CurrentPet = currentPet;
			UnlockedPets = unlockedPets;

			Transform = transform;
			CurrentRole = currentRole;
			PreviousRole = previousRole;

			Roles = roles;
			Inventory = playerInventory;
			Customization = customization;
			Health = health;
			RegenRequiresFood = regenRequiresFood;
		}

		/// <summary>
		/// Converts PlayerSaveData to Player,
		/// </summary>
		/// <returns>PlayerSaveData to class</returns>
		public Player ToPlayer(uint gUID, uint targetGUID, uint stationGUID)
		{
			GameManager manager = GameManager.Instance;
			Player player = new Player(new TwitchUser(TwitchID, TwitchName));

			if (GameUserType == GameUserType.GameMaster && !GameMasterCommands.IsGameMaster(player))
				GameUserType = GameUserType.Normal;
			else if (GameMasterCommands.IsGameMaster(player))
				GameUserType = GameUserType.GameMaster;

			player.TwitchUser.TwitchUserType = TwitchUserType;
			player.TwitchUser.GameUserType = GameUserType;
			player.TwitchUser.IsBroadcaster = IsBroadcaster;

			player.Character = ObjectPoolingManager.GetPooledObject("Player").gameObject;
			player.HealthHandler = player.Character.GetComponent<HealthHandler>();
			player.StationSensor = player.Character.GetComponent<StationSensor>();
			player.TargetSensor = player.Character.GetComponent<TargetSensor>();
			player.EquipmentHandler = player.Character.GetComponent<CharacterModelHandler>();
			player.RoleHandler = player.Character.GetComponent<RoleHandler>();
			player.RoleHandler.SetStarterRole(CurrentRole);
			player.Character.SetActive(true);
			player.Character.transform.position = Vector3SaveData.ToUnityVec3(Transform.Position);
			player.Character.transform.eulerAngles = Vector3SaveData.ToUnityVec3(Transform.Rotation);
			player.Character.transform.localScale = Vector3SaveData.ToUnityVec3(Transform.LossyScale);
			player.RoleHandler.Inventory.SetResources(Inventory.ToDictionary());
			PlayerRoleSaveData.ToPlayerRoleDatas(Roles.ToArray(), player.RoleHandler.PlayerRolesData);
			player.RoleHandler.RecalculateRoles();

			player.EquipmentHandler.LoadFromSaveData(Customization);
			// TODO: Set CurrentPet and Unlocked pet, Test this
			Dictionary<PetType, bool> unlockedPets = new Dictionary<PetType, bool>();
			for (int i = 0; i < (int)PetType.Count; i++)
			{
				if (UnlockedPets.Contains((PetType)i))
					unlockedPets.Add((PetType)i, true);
				else
					unlockedPets.Add((PetType)i, false);
			}

			PoolableObject petObj = ObjectPoolingManager.GetPooledObject("Pet");
			Pet pet = petObj.GetComponent<Pet>();

			player.Pet = pet;
			if (PetActive)
				pet.gameObject.SetActive(true);

			pet.SetOwner(player.Character.transform, player);
			if (PetActive)
			{
				player.Pet.TrySetActivePet(CurrentPet);
				pet.ActivatePet();
			}

			if (IsBroadcaster)
				GameManager.Instance.SetUserPlayer(player);

			player.Pet = pet;
			player.PetsUnlocked = unlockedPets;
			GUIDComponent comp = player.Character.GetComponent<GUIDComponent>();
			comp.SetGUID(gUID);

			//if (stationGUID != 0)
			//	player.StationSensor.TrySetStation(GUIDManager.GetComponentFromID(stationGUID).gameObject.GetComponent<Station>());

			//if (targetGUID != 0)
			//	player.TargetSensor.TrySetTarget(GUIDManager.GetComponentFromID(TargetGUID).gameObject.GetComponent<Targetable>());

			player.RoleHandler.Player = player;

			return player;
		}

		public void SetTargetGUID(uint gUID)
		{
			TargetGUID = gUID;
		}

		public void SetStationGUID(uint gUID)
		{
			StationGUID = gUID;
		}
	}
}