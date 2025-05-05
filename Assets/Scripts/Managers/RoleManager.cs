using System.Collections.Generic;
using Utils;
using UnityEngine;
using Character;
using UnityEngine.Events;
using Scriptables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
	/// <summary>
	/// Manages data for all roles in the game.
	/// </summary>
	[SerializeField, System.Serializable]
	public class RoleManager : MonoBehaviour
	{
		//YES
		public const int MAX_ROLE_LEVEl = 99;

		/// <summary>
		/// The amount of exp used for the last level. EXP table is calculated using this value.
		/// </summary>
		//YES but debugging ReadOnly
		private const int MAX_LEVEL_EXP = 100000;

		//[SerializeField]
		//private RoleDataScriptable[] _storedData = new RoleDataScriptable[(int)PlayerRole.Count];
		[SerializeField]
		private AllRoleDataScriptable _allRoleData;

		/// YES YES YES
        const float ER = 1.2f;
        const float RT = 0.5f;
        const float RM = 100f;

        private Dictionary<PlayerRole, RoleDataScriptable> _roleDataDictionary;

        //YES but debugging ReadOnly
        [SerializeField]
		private int[] _expTableLookup = new int[MAX_ROLE_LEVEl];
		private int expMultiplier = 10;

		private Dictionary<PlayerRole, RoleSlot> _roleSlotsDictionary;

		private UnityEvent<PlayerRole> _onRoleSlotsChangedEvent = new UnityEvent<PlayerRole>();

		public AllRoleDataScriptable AllRoleData => _allRoleData;
		public UnityEvent<PlayerRole> OnRoleSlotsChangedEvent => _onRoleSlotsChangedEvent;

        /// <summary>
        /// Returns the required amount of experience to level up.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetRequiredExperience(int level)
		{
			return _expTableLookup[level - 1];
		}

		/// <summary>
		/// Returns the stored RoleData for that specified PlayerRole.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public RoleDataScriptable GetRoleData(PlayerRole role)
		{
			if (!_roleDataDictionary.ContainsKey(role))
			{
				Debug.LogError($"Attempted to get role data for role {role} but it was not found!");
				return null;
			}

			return _roleDataDictionary[role];
		}

		public List<string> GetAvailableRolesAsString()
		{
			List<string> roles = new List<string>();
			foreach (PlayerRole role in _roleSlotsDictionary.Keys)
			{
				if (_roleSlotsDictionary[role].Available && role != PlayerRole.Ruler)
					roles.Add(role.ToString());
			}
			return roles;
		}

		public List<PlayerRole> GetAvailableRoles()
		{
			List<PlayerRole> roles = new List<PlayerRole>();
			foreach (PlayerRole role in _roleSlotsDictionary.Keys)
			{
				if (_roleSlotsDictionary[role].Available && role != PlayerRole.Ruler)
					roles.Add(role);
			}
			return roles;
		}

		public PlayerRole GetAvailableRoleFromIndex(int index)
		{
			List<PlayerRole> availableRoles = GetAvailableRoles();

			return availableRoles[index];
		}

		public int GetRoleIndex(PlayerRole playerRole)
		{
			List<PlayerRole> availableRoles = GetAvailableRoles();
			for (int i = 0; i < availableRoles.Count; i++)
			{
				if (availableRoles[i].ToString() == playerRole.ToString())
					return i;
			}
			return 0;
		}

		public void TakeFromRole(PlayerRole role)
		{
			_roleSlotsDictionary[role].OnSlotRemoved();
		}

		public bool IsRoleAvailabe(PlayerRole role)
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
        public bool TryChangeRole(PlayerRole previousRole, PlayerRole newRole, out string failureReason, bool decrement = true)
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
        public void AddSlots(PlayerRole role, int amount)
		{
			_roleSlotsDictionary[role].IncreaseMaxSlots(amount);
			_onRoleSlotsChangedEvent.Invoke(role);
		}

		/// <summary>
		/// Removes total slots available for the specified PlayerRole.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="amount"></param>
		public void RemoveSlots(PlayerRole role, int amount)
		{
			_roleSlotsDictionary[role].DecreaseMaxSlots(amount);
			_onRoleSlotsChangedEvent.Invoke(role);
		}

		/// <param name="role"></param>
		/// <returns>true if all slots for the PlayerRole are taken.</returns>
		public bool SlotsFull(PlayerRole role)
		{
			return _roleSlotsDictionary[role].Full;
		}

		/// <param name="role"></param>
		/// <returns>a formatted string displaying number of role slots taken and slots available.</returns>
		public string GetSlotPrint(PlayerRole role)
		{
			return _roleSlotsDictionary[role].SlotDataAsString;
		}

		public int GetMaxSlots(PlayerRole role)
		{
			return _roleSlotsDictionary[role].MaxSlots;
		}

		public bool RoleIsInfinite(PlayerRole role)
		{
			return _roleSlotsDictionary[role].Infinite;
		}

		public bool TryReplaceInactivePlayer(PlayerRole role, out Player player)
		{
			player = null;

			PlayerManager playerManager = GameManager.Instance.PlayerManager;

			for (int i = 0; i < playerManager.PlayerCount(); i++)
			{
				Player targetPlayer = playerManager.GetPlayer(i);

				if (targetPlayer.RoleHandler.CurrentRole != role)
					continue;

				if (targetPlayer.TwitchUser.ActivityStatus != Character.Enumerations.ActivityStatus.Inactive)
					continue;

				player = targetPlayer;
				return true;
			}

			return false;
		}

		private void InitializeRoleData()
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

        //Max role level = 99
        //Expon rate = ER = 1.2
		//Rate temper = RT = 0.5
		//Rate multiplier = RM = 100
        //((ER^(level-1))^RT-(ER^(-1))^RT)*RM
		//This formula starts at zero and increases at a high but reasonable rate, controllable by 3 parameters
		//The expon rate combined with the rate multiplier will determine the overall rate of change of the EXP table
		//The rate multiplier simply scales all the EXP values by a factor
        private void CalculateEXPTable()
		{
			for (int i = 0; i < MAX_ROLE_LEVEl; i++)
			{

				//float t = ((float)i + 2) / 100;
				//float pow = (t * t);
				//float sqrt = 1 - Mathf.Sqrt(1 - pow);
				_expTableLookup[i] = Mathf.RoundToInt(RM * (Mathf.Pow(Mathf.Pow(ER, i - 1),RT) - Mathf.Pow(Mathf.Pow(ER, -1),RT)));

            }
		}

		public void Initialize()
		{
			Debug.Log("Init: " + this);
			InitializeRoleData();
		}
	}
}