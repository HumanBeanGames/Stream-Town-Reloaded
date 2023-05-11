using UnityEngine;
using TMPro;
using Character;
using Managers;
using Units;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PlayerControls.ObjectSelection
{
	public class SelectedPlayerGroup : SelectedObject
	{
		private List<GameObject> _outlines;
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			EnableDisplay();
			UpdateDisplay();
			AttachEvents();
		}

		public void RecruitChange(int index)
		{
			List<RoleHandler> players = ((List<RoleHandler>)_selectedObject);
			for (int i = 0; i < players.Count; i++)
				if (players[i] != null)
					players[i].TrySetRole(GameManager.Instance.RoleManager.GetAvailableRoleFromIndex(index));
		}

		protected override void AttachEvents()
		{
			OnDropDownChange += RecruitChange;
			_selectedObjectTypeUI.SelectionDropdown.onValueChanged.AddListener(OnDropDownChange);

			_selectedObjectTypeUI.SelectionDropdown.ClearOptions();

			_selectedObjectTypeUI.SelectionDropdown.AddOptions(GameManager.Instance.RoleManager.GetAvailableRolesAsString());
			_selectedObjectTypeUI.SelectionDropdown.SetValueWithoutNotify(GameManager.Instance.RoleManager.GetRoleIndex(Utils.PlayerRole.Builder));

			OnButtonClick += OnDismissButtonClick;
			_selectedObjectTypeUI.SelectionButton.onClick.AddListener(OnButtonClick);

			OnCheckConfirm += DisableCheck;
			OnCheckConfirm += DismissRecruits;
			OnCheckDeny += DisableCheck;
		}


		public void OnDismissButtonClick()
		{
			_selectedObjectTypeUI.ConfirmCheck.SetConfirmCheck(OnCheckConfirm, OnCheckDeny, "Do you wish to mass remove these recruits?", "This action is irreversable and will delete all selected recruits!");
		}
		

		public void DismissRecruits()
		{
			List<RoleHandler> players = ((List<RoleHandler>)_selectedObject);
			for (int i = 0; i < players.Count; i++)
				if (players[i] != null)
					GameManager.Instance.PlayerManager.DismissRecruit(players[i].Player);
			_selectedObjectTypeUI.HideContext();
		}

		public void DetachCurrentEvents() { DetachEvents(); }

		protected override void DetachEvents()
		{
			OnDropDownChange -= RecruitChange;
			_selectedObjectTypeUI.SelectionDropdown.onValueChanged.RemoveAllListeners();

			OnButtonClick -= OnDismissButtonClick;
			_selectedObjectTypeUI.SelectionButton.onClick.RemoveAllListeners();

			for (int i = 0; i < _outlines.Count; i++)
			{
				_outlines[i].SetActive(false);
			}
			_outlines.Clear();

			OnCheckConfirm -= DisableCheck;
			OnCheckConfirm -= DismissRecruits;
			OnCheckDeny -= DisableCheck;
			_selectedObjectTypeUI.ConfirmCheck.RemoveListeners();
		}

		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.DropdownHolder.gameObject.SetActive(true);
			_selectedObjectTypeUI.Description.gameObject.SetActive(true);
			_selectedObjectTypeUI.ButtonHolder.gameObject.SetActive(true);
		}

		public override void UpdateDisplay()
		{
			List<RoleHandler> players = ((List<RoleHandler>)_selectedObject);
			_selectedObjectTypeUI.Description.text = $"Mass Selection: {players.Count}";
			_selectedObjectTypeUI.SelectionButtonText.text = "Mass Dismiss";

			_outlines = new List<GameObject>();
			for (int i = 0; i < players.Count; i++)
			{
				_outlines.Add(GameManager.Instance.PoolingManager.GetPooledObject("UI_Selection_Outline").gameObject);
				BoxCollider collider = players[i].GetComponent<BoxCollider>();
				_outlines[i].transform.position = new Vector3(collider.transform.position.x, 0.15f, collider.transform.position.z);
				_outlines[i].transform.rotation = collider.transform.rotation;
				_outlines[i].transform.parent = collider.transform;
				_outlines[i].transform.localScale = new Vector3(collider.size.x * 1.25f, 1, collider.size.z * 1.25f);
				_outlines[i].SetActive(true);
			}
		}
	}
}