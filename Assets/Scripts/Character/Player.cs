using GUIDSystem;
using Pets;
using Pets.Enumerations;
using Sensors;
using System;
using System.Collections.Generic;
using Target;
using Twitch;
using Units;
using UnityEngine;
using UserInterface;
using Utils.Pooling;

namespace Character
{
	/// <summary>
	/// Holds all player data for a Twitch user.
	/// </summary>
	[System.Serializable]
	public class Player
	{
		private TwitchUser _user;

		private Vector3 _lastBuildingPlacement = -Vector3.right * 5;

		public Vector3 LastBuildingPlacement
		{
			get { return _lastBuildingPlacement; }
			set { _lastBuildingPlacement = value; }
		}

		public GameObject Character { get; set; }
		public RoleHandler RoleHandler { get; set; }
		public HealthHandler HealthHandler { get; set; }
		public StationSensor StationSensor { get; set; }
		public TargetSensor TargetSensor { get; set; }
		public CharacterModelHandler EquipmentHandler { get; set; }
		public GUIDComponent GUIDComponent { get; set; }
		public TargetablePlayer PlayerTarget { get; set; }
		public TwitchUser TwitchUser => _user;
		public Dictionary<PetType, bool> PetsUnlocked;
		public Pet Pet { get; set; }
		public PoolableObject PoolableObject { get; set; }
		public bool IsNPC { get; set; }
		public int TotalBuildingRotation { get; set; }

		public UnitTextDisplay UnitTextDisplay { get; set; }

		// Constructor.
		public Player(TwitchUser user, bool IsNPC = false)
		{
			this.IsNPC = IsNPC;

			_user = user;

			PetsUnlocked = new Dictionary<PetType, bool>();

			for (int i = 0; i < (int)PetType.Count; i++)
				PetsUnlocked.Add((PetType)i, false);

			if (TwitchUser.GameUserType == Twitch.Utils.GameUserType.Subscriber || TwitchUser.GameUserType == Twitch.Utils.GameUserType.GameMaster)
				PetsUnlocked[PetType.RedPanda] = true;

			if (IsNPC)
				TwitchUser.ActivityStatus = Enumerations.ActivityStatus.Inactive;

			PetsUnlocked[PetType.None] = true;
		}

		public List<PetType> GetUnlockedPetTypes()
		{
			List<PetType> type = new List<PetType>();

			for (int i = 0; i < (int)PetType.Count; i++)
			{
				if (PetsUnlocked.TryGetValue((PetType)i, out bool yes))
					if (yes)
						type.Add((PetType)i);
			}
			return type;
		}

		public void OnCharacterDied(bool attacked)
		{
			if (IsNPC || !attacked)
				return;

			MessageSender.SendPlayerMessage(this, "You died!");
		}

		public void OnCharacterRespawned()
		{
			if (IsNPC)
				return;

			MessageSender.SendPlayerMessage(this, "You have revived!");
		}
	}
}