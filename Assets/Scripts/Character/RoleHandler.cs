using Animation;
using Behaviours;

using Pathfinding;
using Scriptables;
using Sensors;
using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Character
{
	/// <summary>
	/// Handles the current role of the Player character.
	/// </summary>
	public class RoleHandler : MonoBehaviour
	{
		private Player _player = null;

		[SerializeField]
		private PlayerRole _currentRole = PlayerRole.Builder;
		private PlayerRole _prevRole = PlayerRole.Builder;
		private PlayerRoleType _roleType = PlayerRoleType.Other;

		[SerializeField]
		private UnityEvent<PlayerRole, PlayerRole, BodyType> _onRoleChanged;
		private PlayerInventory _inventory;
		private CollectResource _collectResource;
		private AnimationHandler _animationHandler;
		private CharacterModelHandler _equipmentHandler;
		private RoleManager _roleManager;

		//Sensors
		private TargetSensor _targetSensor;
		private StationSensor _stationSensor;
		private PlayerRole _starterRole = PlayerRole.Builder;
		private RoleDataScriptable _currentRoleData_SO;
		private PlayerRoleData _currentPlayerRoleData;
		private AIPath _aiPath;
		private HealthHandler _healthHandler;

		[SerializeField]
		private PlayerRoleData[] _playerRoleData;

		private Dictionary<StatType, float> _characterGlobalPassives;

		public PlayerRole CurrentRole => _currentRole;
		public PlayerRole PreviousRole => _prevRole;
		public RoleDataScriptable RoleData_SO => _currentRoleData_SO;
		public PlayerRoleData PlayerRoleData => _currentPlayerRoleData;
		public PlayerRoleData[] PlayerRolesData => _playerRoleData;
		public PlayerInventory Inventory => _inventory;
		public AnimationHandler AnimationHandler => _animationHandler;
		public CharacterModelHandler EquipmentHandler => _equipmentHandler;
		public Player Player
		{
			get { return _player; }
			set { _player = value; }
		}

		public event Action<RoleHandler> OnRoleChanged;

        /// <summary>
        /// Attempts to set the role of the character if it is available.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="decrement"></param>
        /// <returns></returns>
        public bool TrySetRole(PlayerRole role, out string failureReason, bool decrement = true)
        {
            if (!_roleManager.TryChangeRole(_currentRole, role, out failureReason, decrement))
                return false;

            _onRoleChanged.Invoke(_currentRole, role, _equipmentHandler.CurrentBodyType);
            _currentRoleData_SO = _roleManager.GetRoleData(role);
            _targetSensor.TargetMask = _currentRoleData_SO.TargetFlags;
            _stationSensor.StationMask = _currentRoleData_SO.StationFlags;
            _roleType = _currentRoleData_SO.RoleFlags;

            _prevRole = (CurrentRole == PlayerRole.Ruler ? role : _currentRole);

            _currentRole = role;
            _currentPlayerRoleData = _playerRoleData[(int)_currentRole];
            _aiPath.maxSpeed = _currentPlayerRoleData.MoveSpeed;
            OnRoleChanged?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Sets the starter role that the character will spawn in as.
        /// </summary>
        /// <param name="role"></param>
        public void SetStarterRole(PlayerRole role)
		{
			if (_roleManager.SlotsFull(role) && GameManager.Instance.PlayerRoleLimits)
				return;

			_starterRole = role;
		}

		/// <summary>
		/// Sets all the role datas from file,
		/// </summary>
		/// <param name="data"></param>
		public void SetRoleData(PlayerRoleData[] data)
		{
			_playerRoleData = data;

			for (int i = 0; i < data.Length; i++)
			{
				_playerRoleData[i].RecalculateStats();
			}
		}

		/// <summary>
		/// Sets all the role datas from file,
		/// </summary>
		/// <param name="data"></param>
		public void RecalculateRoles()
		{
			for (int i = 0; i < _playerRoleData.Length; i++)
			{
				_playerRoleData[i].RecalculateStats();
			}
		}

		public void AddToGlobalPassive(StatType statType, float amount)
		{
			_characterGlobalPassives[statType] += amount;
		}
		public float GetGlobalPassive(StatType statType)
		{
			if (_characterGlobalPassives == null || !_characterGlobalPassives.ContainsKey(statType))
				return 0;

			return _characterGlobalPassives[statType];
		}

		// Unity Events.
		private void Awake()
		{
			_targetSensor = GetComponent<TargetSensor>();
			_stationSensor = GetComponent<StationSensor>();
			_inventory = GetComponent<PlayerInventory>();
			_collectResource = GetComponent<CollectResource>();
			_animationHandler = GetComponentInChildren<AnimationHandler>();
			_equipmentHandler = GetComponent<CharacterModelHandler>();
			_roleManager = GameManager.Instance.RoleManager;
			_healthHandler = GetComponent<HealthHandler>();
			_aiPath = GetComponent<AIPath>();
			_playerRoleData = new PlayerRoleData[(int)PlayerRole.Count];

			for (int i = 0; i < (int)PlayerRole.Count; i++)
			{
				_playerRoleData[i] = new PlayerRoleData((PlayerRole)i, _roleManager, _inventory, _aiPath, _healthHandler, this);
			}

			_characterGlobalPassives = new Dictionary<StatType, float>();

			for (int i = 0; i < (int)StatType.Count; i++)
			{
				_characterGlobalPassives.Add((StatType)i, 0.0f);
			}
		}

		private void Start()
		{
			TrySetRole(_starterRole, out string reason, false);
			if (reason!="")
				Debug.Log(reason);
			_healthHandler.SetHealth(_healthHandler.MaxHealth);
		}

		private void OnEnable()
		{
			GameManager.Instance.TechTreeManager.OnStatBoostUnlocked += OnStatBoostUnlocked;
		}

		private void OnStatBoostUnlocked(PlayerRole role, StatType type)
		{
			if (role == _currentRole)
				_currentPlayerRoleData.RecalculateStats();
		}

		public bool TryGetRoleData(PlayerRole role, out PlayerRoleData data)
		{
			for(int i = 0; i < _playerRoleData.Length; i ++)
				if(role == _playerRoleData[i].Role)
				{
					data = _playerRoleData[i];
					return true;
				}
			data = null;
			return false;
		}
	}
}