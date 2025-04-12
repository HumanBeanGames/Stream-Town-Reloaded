
using Pathfinding;
using Scriptables;
using System;
using Units;
using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Holds all data relating to a player's role and ther role's statistics.
	/// </summary>
	[System.Serializable]
	public class PlayerRoleData
	{
		private const float ACCELERATION_MULTIPLIER = 0.33f;

		private PlayerRole _role;
		private PlayerRoleType _roleType;

		private bool _ranged;
		private int _level;
		private int _experience;
		private int _requiredExp;
		private int _actionAmount;
		private float _actionRate;
		private float _actionRange;
		private int _maxHealth;
		private float _healthRegen;
		private float _movementSpeed;
		private int _damageReduction;
		private AudioClip[] _actionClips;

		private RoleManager _roleManager;
		private PlayerInventory _playerInventory;
		private AIPath _aiPath;
		private HealthHandler _healthHandler;
		private RoleHandler _roleHandler;

		public PlayerRole Role => _role;
		public int ActionAmount => _actionAmount;
		public float ActionRate => _actionRate;
		public float ActionRange => _actionRange;
		public int DamageReduction => _damageReduction;
		public float HealthRegen => _healthRegen;
		public int MaxHealth => _maxHealth;
		public float MoveSpeed => _movementSpeed;
		public int CurrentLevel => _level;
		public int CurrentExp => _experience;
		public int RequiredExp => _requiredExp;
		public bool IsMaxLevel => (_level >= RoleManager.MAX_ROLE_LEVEL);
		public AudioClip[] ActionClips => _actionClips;

		public event Action<RoleHandler> OnExperienceChange;

		public HealthHandler HealthHandler => _healthHandler;

		// Constructors.
		public PlayerRoleData(PlayerRole role, RoleManager roleManager, PlayerInventory inventory, AIPath aiPath, HealthHandler healthHandler, RoleHandler roleHandler)
		{
			_role = role;
			_roleManager = roleManager;
			_level = 1;
			_experience = 0;
			_requiredExp = _roleManager.GetRequiredExperience(_level);
			_roleType = roleManager.GetRoleData(role).RoleFlags;
			_playerInventory = inventory;
			_aiPath = aiPath;
			_healthHandler = healthHandler;
			_roleHandler = roleHandler;
			_actionClips = roleManager.GetRoleData(role).ActionClips;
			//TODO: Implement ranged check
			RecalculateStats();
		}

		public PlayerRoleData(PlayerRole role, int level, int experience, RoleManager roleManager, PlayerInventory inventory, AIPath aiPath, HealthHandler healthHandler)
		{
			_role = role;
			_roleManager = roleManager;
			_level = level;
			_experience = experience;
			_requiredExp = _roleManager.GetRequiredExperience(_level);

			RecalculateStats();
		}

		/// <summary>
		/// Increases the amount of current experience.
		/// </summary>
		/// <param name="amount"></param>
		public void IncreaseExperience(int amount)
		{
			amount = Mathf.Max(1,(int)( amount * _roleManager.GetRoleData(_role).ExpModifier));
			if (IsMaxLevel)
			{
				_experience = 0;
				return;
			}

			_experience += amount;
			OnExperienceChanged();
		}

		/// <summary>
		/// Levels up the player's role by one.
		/// </summary>
		public void LevelUp()
		{
			IncreaseExperience(_requiredExp - _experience);
		}

		/// <summary>
		/// Called when the player's role experience has changed.
		/// </summary>
		private void OnExperienceChanged()
		{
			if (_experience >= _requiredExp)
			{
				_experience -= _requiredExp;
				OnLevelUp();
				OnExperienceChanged();
			}
			OnExperienceChange?.Invoke(_roleHandler);
		}

		/// <summary>
		/// Called when the player's role has leveled up.
		/// </summary>
		private void OnLevelUp()
		{
			if (IsMaxLevel)
				return;

			_level++;
			_requiredExp = _roleManager.GetRequiredExperience(_level);
			RecalculateStats();
			_healthHandler.SetHealth(_healthHandler.MaxHealth);
		}

		/// <summary>
		/// Recalculates all role stats based on the role's level.
		/// </summary>
		public void RecalculateStats()
		{
			RoleDataScriptable data = _roleManager.GetRoleData(_role);
			StatModifiers statMod = GameManager.Instance.PlayerManager.GetStatModifiers(_role);

			_actionAmount = data.BaseActionAmount + (int)(data.ActionAmountPerLevel * (_level - 1));
			_actionAmount += AddStatModifiersInt(statMod, StatType.ActionAmount, _actionAmount);
			_actionAmount += AddStatModifiersInt(_roleHandler.GetGlobalPassive(StatType.ActionAmount), _actionAmount);

			_actionRate = Mathf.Max(0.1f, data.BaseActionSpeed - (data.ActionSpeedPerLevel * (_level - 1)));
			_actionRate -= AddStatModifiersFloat(statMod, StatType.ActionSpeed, _actionRate);
			_actionRate += AddStatModifiersFloat(_roleHandler.GetGlobalPassive(StatType.ActionSpeed), _actionRate);

			_actionRange = data.BaseActionRange + (_ranged ? (data.ActionRangePerLevel * (_level - 1)) : 0);
			_actionRange += AddStatModifiersFloat(statMod, StatType.ActionRange, _actionRange);
			_actionRange += AddStatModifiersFloat(_roleHandler.GetGlobalPassive(StatType.ActionRange), _actionRange);

			_maxHealth = data.BaseHealth + (int)(data.HealthPerLevel * (_level - 1));
			_maxHealth += AddStatModifiersInt(statMod, StatType.Health, _maxHealth);
			_maxHealth += AddStatModifiersInt(_roleHandler.GetGlobalPassive(StatType.Health), _maxHealth);

			_healthRegen = data.BaseHealthRegen + (data.HealthRegenPerLevel * (_level - 1));
			_healthRegen += AddStatModifiersFloat(statMod, StatType.HealthRegen, _healthRegen);
			_healthRegen += AddStatModifiersFloat(_roleHandler.GetGlobalPassive(StatType.HealthRegen), _healthRegen);

			_damageReduction = data.BaseDamageReduction + (int)(data.DamageReductionPerLevel * (_level - 1));
			_damageReduction += AddStatModifiersInt(statMod, StatType.Defense, _damageReduction);
			_damageReduction += AddStatModifiersInt(_roleHandler.GetGlobalPassive(StatType.Defense), _damageReduction);

			_movementSpeed = data.BaseMovementSpeed + (data.MovementSpeedPerLevel * (_level - 1));
			_movementSpeed += AddStatModifiersFloat(statMod, StatType.MovementSpeed, _movementSpeed);
			_movementSpeed += AddStatModifiersFloat(_roleHandler.GetGlobalPassive(StatType.MovementSpeed), _movementSpeed);

			if (_roleType == PlayerRoleType.Resource)
			{
				_playerInventory.SetMaxStorage(data.Resource, data.BaseMaxResource + (int)(data.MaxResourcePerLevel * (_level - 1)));
			}

			_aiPath.maxSpeed = _movementSpeed;
			_aiPath.maxAcceleration = _movementSpeed * ACCELERATION_MULTIPLIER;
			_healthHandler.SetMaxHealth(_maxHealth);
			_healthHandler.SetHealthRegen(_healthRegen);
			//TODO:: Add globals here, also implement damage reduction, health regen, max health and movement speed
		}

		/// <summary>
		/// Sets the role
		/// </summary>
		public void SetRole(PlayerRole role)
		{
			_role = role;
		}

		/// <summary>
		/// Sets the experience
		/// </summary>
		public void SetExperience(int experience)
		{
			_experience = experience;
		}

		/// <summary>
		/// Sets the level
		/// </summary>
		public void SetLevel(int level)
		{
			_level = level;
		}


		private int AddStatModifiersInt(StatModifiers statMod, StatType statType, int baseValue)
		{
			return (int)(baseValue * (statMod.GetModifier(statType) / 100.0f));
		}

		private float AddStatModifiersFloat(StatModifiers statMod, StatType statType, float baseValue)
		{
			return baseValue * (float)(statMod.GetModifier(statType) / 100.0f);
		}

		private int AddStatModifiersInt(float mod, int baseValue)
		{
			return (int)(baseValue * (mod / 100.0f));
		}
		private float AddStatModifiersFloat(float mod, int baseValue)
		{
			return (float)(baseValue * (mod / 100.0f));
		}

		private float AddStatModifiersFloat(float mod, float baseValue)
		{
			return (float)(baseValue * (mod / 100.0f));
		}
		private float AddStatModifiersFloat(int mod, int baseValue)
		{
			return (float)(baseValue * (mod / 100.0f));
		}
	}
}