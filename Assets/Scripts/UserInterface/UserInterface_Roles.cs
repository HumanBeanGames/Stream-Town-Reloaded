using UnityEngine;
using Utils;
using TMPro;
using Managers;
using System.Collections.Generic;
using Scriptables;
using Character;

namespace UserInterface
{
	/// <summary>
	/// Handles the Player Roles User Interface.
	/// </summary>
	public class UserInterface_Roles : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rolePanel;

		//private Dictionary<PlayerRole, Text>

		[SerializeField]
		private GameObject _roleUIPrefab;

		[SerializeField]
		private Transform _roleUITransform;

		private Dictionary<PlayerRole, UIRoleDisplay> _roleDisplays;

		/// <summary>
		/// Called when the number of role slots has changed and updates the text to display the new slot count.
		/// </summary>
		/// <param name="role"></param>
		private void OnRoleSlotsChanged(PlayerRole role)
		{
			if (_roleDisplays == null || !_roleDisplays.ContainsKey(role))
				return;
			if (role == PlayerRole.Ruler)
				return;
			_roleDisplays[role].RoleAmount.text = $"{RoleManager.GetSlotPrint(role)}";

			if (RoleManager.GetMaxSlots(role) == 0 && !RoleManager.RoleIsInfinite(role))
				_roleDisplays[role].gameObject.SetActive(false);
			else
				_roleDisplays[role].gameObject.SetActive(true);
		}

		private void Start()
		{
			_roleDisplays = new Dictionary<PlayerRole, UIRoleDisplay>();
			RoleManager.OnRoleSlotsChangedEvent.AddListener(OnRoleSlotsChanged);
			List<RoleDataScriptable> _resourceRoles = new List<RoleDataScriptable>();
			List<RoleDataScriptable> _combatRoles = new List<RoleDataScriptable>();
			List<RoleDataScriptable> _otherRoles = new List<RoleDataScriptable>();
			RoleDataScriptable _ruler = null;
			RoleDataScriptable _builder = null;

			// organize roles by type.

			for (int i = 0; i < (int)PlayerRole.Count; i++)
			{
				RoleManager.OnRoleSlotsChangedEvent.Invoke((PlayerRole)i);
				RoleDataScriptable rds = RoleManager.GetRoleData((PlayerRole)i);

				if (rds.RoleFlags == PlayerRoleType.Damage || rds.RoleFlags == PlayerRoleType.Healer)
				{
					if (rds.Role == PlayerRole.Ruler)
						_ruler = rds;
					else if (rds.Role == PlayerRole.Builder)
						_builder = rds;
					else
						_combatRoles.Add(rds);
				}
				else if (rds.RoleFlags == PlayerRoleType.Resource)
				{
					_resourceRoles.Add(rds);
				}
				else
					_otherRoles.Add(rds);
			}

			// Ruler
			AddNewRoleDataUI(_ruler, new Color32(255, 242, 197, 255));
			PlayerManager.OnRulerChanged += OnRulerChanged;
			AddNewRoleDataUI(_builder, Color.white);
			for (int i = 0; i < _resourceRoles.Count; i++)
				AddNewRoleDataUI(_resourceRoles[i], new Color32(230, 255, 210, 255));

			for (int i = 0; i < _combatRoles.Count; i++)
				AddNewRoleDataUI(_combatRoles[i], new Color32(255, 219, 219, 255));

			for (int i = 0; i < _otherRoles.Count; i++)
				AddNewRoleDataUI(_otherRoles[i], Color.white);
		}

		private void AddNewRoleDataUI(RoleDataScriptable rds, Color32 textColor)
		{
			var go = GameObject.Instantiate(_roleUIPrefab, _roleUITransform);
			var component = go.GetComponent<UIRoleDisplay>();

			component.Icon.sprite = rds.DisplayIcon;

			component.RoleNameTMP.text = rds.Role.ToString();
			if (rds.Role == PlayerRole.Ruler)
			{
				component.RoleAmount.text = "No Ruler";
			}
			else
			{
				component.RoleAmount.text = $"0";

				if (rds.HasUserLimit)
					component.RoleAmount.text += $"   /   {rds.BaseMaxUserLimit}";
			}

			component.RoleNameTMP.color = textColor;
			component.RoleAmount.color = textColor;

			_roleDisplays.Add(rds.Role, component);

			if (rds.HasUserLimit && rds.BaseMaxUserLimit == 0)
				component.gameObject.SetActive(false);
		}

		private void OnRulerChanged(Player player)
		{
			_roleDisplays[PlayerRole.Ruler].RoleAmount.text = player == null ? "No Ruler" : $"{player.TwitchUser.Username}";
		}
	}
}