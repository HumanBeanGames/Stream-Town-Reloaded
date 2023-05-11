using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

namespace PlayerControls
{
	public class ConfirmCheck : MonoBehaviour
	{
		[SerializeField]
		private Button _confirmButton;

		[SerializeField]
		private Button _denyButton;

		[SerializeField]
		private TMP_Text _title;

		[SerializeField]
		private TMP_Text _description;

		[SerializeField]
		private GameObject _uIObject;

		public Button ConfirmButton => _confirmButton;
		public Button DenyButton => _denyButton;

		public void EnableCheck()
		{
			_uIObject.SetActive(true);
		}

		public void DisableCheck()
		{
			_uIObject.SetActive(false);
		}

		private void SetPrompt(string prompt, string description)
		{
			_title.text = prompt;
			_description.text = description;
		}

		private void SetConfirmFunction(UnityAction confirm)
		{
			_confirmButton?.onClick.AddListener(confirm);
		}

		private void SetDenyFunction(UnityAction deny)
		{
			_denyButton?.onClick.AddListener(deny);
		}

		public void RemoveListeners()
		{
			_confirmButton.onClick.RemoveAllListeners();
			_denyButton.onClick.RemoveAllListeners();
		}

		public void SetConfirmCheck(UnityAction confirm, UnityAction deny, string title, string description)
		{
			SetPrompt(title, description);
			SetConfirmFunction(confirm);
			SetDenyFunction(deny);
			EnableCheck();
		}
	}
}