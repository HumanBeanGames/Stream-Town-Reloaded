using Character;
using Scriptables;
using Settings;
using UnityEngine;

namespace UserInterface 
{
    public class UserInterface_DisplayUsernames : MonoBehaviour 
	{
		[SerializeField]
		private GameObject _displayUI;


		private bool _initialized = false;

		private UsernameDisplayOption _displaySettingOption;
		
		private Player _player;

		/// <summary>
		/// Checks and changes the display option
		/// </summary>
		private void CheckWhatDisplayOption()
		{
			if (!_initialized || !_displayUI)
				return;
			if (_player == null)
				return;
			
			switch (SettingsManager.CurrentSettings.displayName)
			{
				case UsernameDisplayOption.Subscribers:
					if(_player.TwitchUser.GameUserType != Twitch.Utils.GameUserType.Normal)
						_displayUI.SetActive(true);
					else
						_displayUI.SetActive(false);
					break;
				case UsernameDisplayOption.All:
					_displayUI.SetActive(true);
					break;
				default:
					_displayUI.SetActive(false);
					break;
			}
		}

		private void Awake()
		{
			_player = GetComponent<RoleHandler>().Player;
			_initialized = true;
		}
		private void Start()
		{
			_player = GetComponent<RoleHandler>().Player;
		}
		private void Update()
		{
			CheckWhatDisplayOption();
		}
	}

	public enum UsernameDisplayOption
	{
		None,
		Subscribers,
		All
	}
}