using UnityEngine;
using Settings;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PlayerControls;
using UserInterface.MainMenu;
using Managers;

namespace UserInterface 
{
    public class UserInterface_GameMenu : MonoBehaviour 
	{
		[SerializeField]
		private GameObject _gameMenu;

		[SerializeField]
		private GameObject _savePanel;

		[SerializeField]
		private GameObject _loadPanel;

		[SerializeField]
		private GameObject _mainMenuPanel;
		
		private GameObject _settingsPanel;
		private SettingsManager _settingsManager;
		private CameraApplyChanges _cameraApplyChanges;
		private LoadingManager _loadingManager;
		private bool _savedGame;

		public bool SavedGame
        {
            get { return _savedGame; }
			set { _savedGame = value; }
		}
		public void ToggleGameMenu()
		{
			_gameMenu.SetActive(!_gameMenu.activeSelf);
			_cameraApplyChanges.gameObject.GetComponent<CameraController>().enabled = !_gameMenu.activeSelf;
		}

		public void ToggleSettingsPanel()
		{
			_settingsPanel.SetActive(!_settingsPanel.activeSelf);
		}

		public void ToggleSavePanel()
		{
			_savePanel.SetActive(!_savePanel.activeSelf);
		}

		public void ToggleLoadPanel()
		{
			_loadPanel.SetActive(!_loadPanel.activeSelf);			
		}

		public void ToggleMainMenuPanel()
		{
			_mainMenuPanel.SetActive(!_mainMenuPanel.activeSelf);
		}

		public void QuitToMainMenu()
        {
			_settingsManager.TogglingConnectionTab(true);
			_loadingManager.LoadNonWorldScenes(1);
		}
		
		public void ToggleIdleMode(bool toggle)
		{
			GameManager.Instance.IdleMode = toggle;
			//_cameraApplyChanges.gameObject.GetComponent<CameraController>().IsIdle = toggle;
		}

		private void Awake()
		{
			_cameraApplyChanges = FindAnyObjectByType<CameraApplyChanges>();
			_loadingManager = FindAnyObjectByType<LoadingManager>();
			if (FindAnyObjectByType<SettingsManager>())
			{
				_settingsManager = FindAnyObjectByType<SettingsManager>();
				_settingsPanel = _settingsManager.SettingsPanel;
				_settingsManager.SetUpCamera();
				_settingsManager.GameSettingData(_cameraApplyChanges);
				if (GameManager.Instance)
					GameManager.Instance.SaveManager.SetAutosaveTime(_settingsManager.AutosaveTimeIntervals[GameManager.Instance.SettingsData.autosaveTime] * 60.0f);
			}
		}

		private void Update()
		{
			if(Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				if (!_mainMenuPanel.activeSelf && !_settingsPanel.activeSelf)
				{
					ToggleGameMenu();
				}
				if (_settingsPanel.activeSelf)
				{
					_settingsManager.ToggleSettingsPanel();
				}
				if (_mainMenuPanel.activeSelf)
				{
					ToggleMainMenuPanel();
				}
			}
		}
	}
}