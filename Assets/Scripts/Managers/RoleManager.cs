using System.Collections.Generic;
using Utils;
using UnityEngine;
using Character;
using UnityEngine.Events;
using Scriptables;
using Sirenix.OdinInspector;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
	/// <summary>
	/// Manages data for all roles in the game.
	/// </summary>
	[SerializeField, System.Serializable]
	[GameManager]
	public static class RoleManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static RoleConfig Config = RoleConfig.Instance;

		public static int MaxRoleLevel => RoleConfig.maxRoleLevel;

		[SerializeField]
		private static AllRoleDataScriptable _allRoleData => Config.allRoleData;

        private static int[] _expTableLookup => Config.expTableLookup;

        [HideInInspector]
        private static Dictionary<PlayerRole, RoleDataScriptable> _roleDataDictionary;
        [HideInInspector]
        private static Dictionary<PlayerRole, RoleSlot> _roleSlotsDictionary;
        [HideInInspector]
        private static UnityEvent<PlayerRole> _onRoleSlotsChangedEvent = new UnityEvent<PlayerRole>();

		public static AllRoleDataScriptable AllRoleData => _allRoleData;
		public static UnityEvent<PlayerRole> OnRoleSlotsChangedEvent => _onRoleSlotsChangedEvent;

        /// <summary>
        /// Returns the required amount of experience to level up.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetRequiredExperience(int level)
		{
			return _expTableLookup[level - 1];
		}

		/// <summary>
		/// Returns the stored RoleData for that specified PlayerRole.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static RoleDataScriptable GetRoleData(PlayerRole role)
		{
			if (!_roleDataDictionary.ContainsKey(role))
			{
				Debug.LogError($"Attempted to get role data for role {role} but it was not found!");
				return null;
			}

			return _roleDataDictionary[role];
		}

		public static List<string> GetAvailableRolesAsString()
		{
			List<string> roles = new List<string>();
			foreach (PlayerRole role in _roleSlotsDictionary.Keys)
			{
				if (_roleSlotsDictionary[role].Available && role != PlayerRole.Ruler)
					roles.Add(role.ToString());
			}
			return roles;
		}

		public static List<PlayerRole> GetAvailableRoles()
		{
			List<PlayerRole> roles = new List<PlayerRole>();
			foreach (PlayerRole role in _roleSlotsDictionary.Keys)
			{
				if (_roleSlotsDictionary[role].Available && role != PlayerRole.Ruler)
					roles.Add(role);
			}
			return roles;
		}

		public static PlayerRole GetAvailableRoleFromIndex(int index)
		{
			List<PlayerRole> availableRoles = GetAvailableRoles();

			return availableRoles[index];
		}

		public static int GetRoleIndex(PlayerRole playerRole)
		{
			List<PlayerRole> availableRoles = GetAvailableRoles();
			for (int i = 0; i < availableRoles.Count; i++)
			{
				if (availableRoles[i].ToString() == playerRole.ToString())
					return i;
			}
			return 0;
		}

		public static void TakeFromRole(PlayerRole role)
		{
			_roleSlotsDictionary[role].OnSlotRemoved();
		}

		public static bool IsRoleAvailabe(PlayerRole role)
		{
			return !_roleSlotsDictionary[role].Full;
		}
        /// <summary>
        /// Called when a character unit attempts to change it's role.
        /// </summary>
        /// <param name="previousRole"></param>
        /// <param name="newRole"></param>
        /// <param name="decrement"></param>
        /// <returns>true if the role can be switched too.</returns>
        public static bool TryChangeRole(PlayerRole previousRole, PlayerRole newRole, out string failureReason, bool decrement = true)
        {
            failureReason = null;

            if (!_roleSlotsDictionary.ContainsKey(newRole))
            {
                failureReason = $"⚠️ Role '{newRole}' does not exist in role slot dictionary.";
                Debug.LogError(failureReason);
                return false;
            }

            if (_roleSlotsDictionary[newRole].Full && GameManager.Instance.PlayerRoleLimits)
            {
                // Slot is full, try replacing an inactive player
                if (!TryReplaceInactivePlayer(newRole, out Player player))
                {
                    failureReason = $"❌ No available slots for role '{newRole}' and no inactive player could be replaced.";
                    return false;
                }

                if (!player.RoleHandler.TrySetRole(PlayerRole.Builder, out string swapReason, decrement))
                {
                    failureReason = $"❌ Replacing inactive player failed: {swapReason}";
                    return false;
                }
            }


            if (decrement)
                _roleSlotsDictionary[previousRole].OnSlotRemoved();

            _roleSlotsDictionary[newRole].OnSlotTaken();
            _onRoleSlotsChangedEvent.Invoke(previousRole);
            _onRoleSlotsChangedEvent.Invoke(newRole);

            return true;
        }

        /// <summary>
        /// Adds more total slots to the specified PlayerRole.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="amount"></param>
        public static void AddSlots(PlayerRole role, int amount)
		{
			_roleSlotsDictionary[role].IncreaseMaxSlots(amount);
			_onRoleSlotsChangedEvent.Invoke(role);
		}

		/// <summary>
		/// Removes total slots available for the specified PlayerRole.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="amount"></param>
		public static void RemoveSlots(PlayerRole role, int amount)
		{
			_roleSlotsDictionary[role].DecreaseMaxSlots(amount);
			_onRoleSlotsChangedEvent.Invoke(role);
		}

		/// <param name="role"></param>
		/// <returns>true if all slots for the PlayerRole are taken.</returns>
		public static bool SlotsFull(PlayerRole role)
		{
			return _roleSlotsDictionary[role].Full;
		}

		/// <param name="role"></param>
		/// <returns>a formatted string displaying number of role slots taken and slots available.</returns>
		public static string GetSlotPrint(PlayerRole role)
		{
			return _roleSlotsDictionary[role].SlotDataAsString;
		}

		public static int GetMaxSlots(PlayerRole role)
		{
			return _roleSlotsDictionary[role].MaxSlots;
		}

		public static bool RoleIsInfinite(PlayerRole role)
		{
			return _roleSlotsDictionary[role].Infinite;
		}

		public static bool TryReplaceInactivePlayer(PlayerRole role, out Player player)
		{
			player = null;

			for (int i = 0; i < PlayerManager.PlayerCount(); i++)
			{
				Player targetPlayer = PlayerManager.GetPlayer(i);

				if (targetPlayer.RoleHandler.CurrentRole != role)
					continue;

				if (targetPlayer.TwitchUser.ActivityStatus != Character.Enumerations.ActivityStatus.Inactive)
					continue;

				player = targetPlayer;
				return true;
			}

			return false;
		}

		private static void InitializeRoleData()
		{
			_roleDataDictionary = new Dictionary<PlayerRole, RoleDataScriptable>();
			_roleSlotsDictionary = new Dictionary<PlayerRole, RoleSlot>();

			for (int i = 0; i < _allRoleData.RoleData.Length; i++)
			{
				var role = (PlayerRole)i;
				if (_roleDataDictionary.ContainsKey(role))
				{
					Debug.LogError($"Attempted to add the same role multiple times {role}.");
					continue;
				}
				_roleDataDictionary.Add(role, _allRoleData.GetDataByRoleType(role));
				_roleSlotsDictionary.Add(role, new RoleSlot(role, _roleDataDictionary[role].BaseMaxUserLimit, !_roleDataDictionary[role].HasUserLimit));
			}
		}

		public static void Initialize()
		{
			InitializeRoleData();
		}
	}
}