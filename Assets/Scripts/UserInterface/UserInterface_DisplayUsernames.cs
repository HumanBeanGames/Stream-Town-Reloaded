using Character;
using Scriptables;
using UnityEngine;

namespace UserInterface 
{
    public class UserInterface_DisplayUsernames : MonoBehaviour 
	{
		[SerializeField]
		private GameObject _displayUI;
		
		[SerializeField]
		private SettingsScriptable _settingsScriptable;

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
			
			switch (_settingsScriptable.displayName)
			{
				case 1:
					_displaySettingOption = UsernameDisplayOption.Subscribers;
					break;
				case 2:
					_displaySettingOption = UsernameDisplayOption.All;
					break;
				default:
					_displaySettingOption = UsernameDisplayOption.None;
					break;
			}
			
			switch (_displaySettingOption)
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