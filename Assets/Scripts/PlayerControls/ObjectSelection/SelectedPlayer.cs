using UnityEngine;
using TMPro;
using Character;

using Units;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PlayerControls.ObjectSelection
{
	public class SelectedPlayer : SelectedObject
	{
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			EnableDisplay();
			UpdateDisplay();
			AttachEvents();
		}

		public void RecruitChange(int index)
		{
			RoleHandler roleHandler = ((RoleHandler)_selectedObject);
			if (roleHandler.TrySetRole(GameManager.Instance.RoleManager.GetAvailableRoleFromIndex(index), out string reason))
			{
				UpdateExperience(roleHandler);
			}
			else
				Debug.Log(reason);
		}

		protected override void AttachEvents()
		{
			RoleHandler roleHandler = ((RoleHandler)_selectedObject);
			roleHandler.PlayerRoleData.OnExperienceChange += UpdateExperience;
			roleHandler.PlayerRoleData.HealthHandler.OnHealthChange += UpdateHealth;
			roleHandler.OnRoleChanged += UpdateRole;

			// Is player a recruit?
			if (string.IsNullOrEmpty(roleHandler.Player.TwitchUser.Username) || roleHandler.Player.TwitchUser.TwitchUserType == TwitchLib.Client.Enums.UserType.Broadcaster)
			{
				_selectedObjectTypeUI.DropdownHolder.SetActive(true);
				OnDropDownChange += RecruitChange;
				_selectedObjectTypeUI.SelectionDropdown.onValueChanged.AddListener(OnDropDownChange);

				_selectedObjectTypeUI.SelectionDropdown.ClearOptions();

				_selectedObjectTypeUI.SelectionDropdown.AddOptions(GameManager.Instance.RoleManager.GetAvailableRolesAsString());
				_selectedObjectTypeUI.SelectionDropdown.SetValueWithoutNotify(GameManager.Instance.RoleManager.GetRoleIndex(roleHandler.CurrentRole));

				if (string.IsNullOrEmpty(roleHandler.Player.TwitchUser.Username))
				{
					OnButtonClick += OnDismissButtonClick;
					_selectedObjectTypeUI.SelectionButton.onClick.AddListener(OnButtonClick);
					_selectedObjectTypeUI.SelectionButtonText.text = "Dismiss";
					_selectedObjectTypeUI.ButtonHolder.SetActive(true);

					OnCheckConfirm += DisableCheck;
					OnCheckConfirm += DismissRecruits;
					OnCheckDeny += DisableCheck;
;				}
			}

			UpdateExperience(roleHandler);
			roleHandler.PlayerRoleData.HealthHandler.OnDeath += OnPlayerDeathOrRemove;
		}

		public void OnDismissButtonClick()
		{
			_selectedObjectTypeUI.ConfirmCheck.SetConfirmCheck(OnCheckConfirm, OnCheckDeny, "Do you wish to Dismiss this recruit?", "This action is irreversable and will delete this recruit!");	
		}

		public void DismissRecruits()
		{
			RoleHandler roleHandler = ((RoleHandler)_selectedObject);
			OnPlayerDeathOrRemove(true);
			GameManager.Instance.PlayerManager.DismissRecruit(roleHandler.Player);
		}

		public void OnPlayerDeathOrRemove(bool died)
		{
			DetachCurrentEvents();
			_selectedObjectTypeUI.HideContext();
		}

		public void DetachCurrentEvents() { DetachEvents(); }

		protected override void DetachEvents()
		{
			RoleHandler roleHandler = ((RoleHandler)_selectedObject);
			roleHandler.PlayerRoleData.OnExperienceChange -= UpdateExperience;
			roleHandler.PlayerRoleData.HealthHandler.OnHealthChange -= UpdateHealth;
			roleHandler.OnRoleChanged -= UpdateRole;

			// Is player a recruit
			if (string.IsNullOrEmpty(roleHandler.Player.TwitchUser.Username) || roleHandler.Player.TwitchUser.TwitchUserType == TwitchLib.Client.Enums.UserType.Broadcaster)
			{
				OnDropDownChange -= RecruitChange;
				_selectedObjectTypeUI.SelectionDropdown.onValueChanged.RemoveAllListeners();

				if (string.IsNullOrEmpty(roleHandler.Player.TwitchUser.Username))
				{
					OnButtonClick -= OnDismissButtonClick;
					_selectedObjectTypeUI.SelectionButton.onClick.RemoveAllListeners();

					OnCheckConfirm -= DisableCheck;
					OnCheckConfirm -= DismissRecruits;
					OnCheckDeny -= DisableCheck;
					_selectedObjectTypeUI.ConfirmCheck.RemoveListeners();
				}
			}
		}
		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.Title.gameObject.SetActive(true);
			_selectedObjectTypeUI.RoleContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.Description.gameObject.SetActive(true);
			_selectedObjectTypeUI.RedSliderContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.GreenSliderContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.ID.gameObject.SetActive(true);
		}

		public override void UpdateDisplay()
		{
			RoleHandler roleHandler = ((RoleHandler)_selectedObject);
			UpdatePlayerName(roleHandler);
			UpdateHealth(roleHandler.PlayerRoleData.HealthHandler);
			UpdateRole(roleHandler);
			UpdateExperience(roleHandler);
			UpdateActivity(roleHandler);
		}

		public void UpdatePlayerName(RoleHandler roleHandler)
		{
			if (string.IsNullOrEmpty(roleHandler.Player.TwitchUser.Username))
				_selectedObjectTypeUI.Title.text = "RECRUIT";
			else
				_selectedObjectTypeUI.Title.text = (roleHandler.Player.TwitchUser.Username).ToUpper();
		}

		public void UpdateHealth(HealthHandler health)
		{
			_selectedObjectTypeUI.RedSliderValue.text = health.Health + " / " + health.MaxHealth;
			if (health.Health != 0.0f)
				_selectedObjectTypeUI.RedSlider.value = health.HealthPercentage;
			else
				_selectedObjectTypeUI.RedSlider.value = 0.0f;
		}

		public void UpdateRole(RoleHandler roleHandler)
		{
			_selectedObjectTypeUI.Role.text = "Role: " + roleHandler.CurrentRole.ToString();
			_selectedObjectTypeUI.RoleImage.sprite = GameManager.Instance.RoleManager.AllRoleData.GetDataByRoleType(roleHandler.CurrentRole).DisplayIcon;
		}

		public void UpdateExperience(RoleHandler roleHandler)
		{
			_selectedObjectTypeUI.Description.text = "Lvl. " + roleHandler.PlayerRoleData.CurrentLevel + " /  Lvl. " + RoleManager.MAX_ROLE_LEVEL;
			_selectedObjectTypeUI.GreenSliderValue.text = roleHandler.PlayerRoleData.CurrentExp + " / " + roleHandler.PlayerRoleData.RequiredExp;
			if (roleHandler.PlayerRoleData.CurrentExp != 0.0f)
				_selectedObjectTypeUI.GreenSlider.value = ((float)roleHandler.PlayerRoleData.CurrentExp / roleHandler.PlayerRoleData.RequiredExp);
			else
				_selectedObjectTypeUI.GreenSlider.value = 0.0f;
		}

		public void UpdateActivity(RoleHandler roleHandler)
		{
			// Get if the player is currently active and  enable role switching is inactive
		}
	}
}