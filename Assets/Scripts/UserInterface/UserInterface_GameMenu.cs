using UnityEngine;
using Settings;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PlayerControls;
using UserInterface.MainMenu;


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
			SettingsManager.TogglingConnectionTab(true);
			_loadingManager.LoadNonWorldScenes(1);
		}
		
		public void ToggleIdleMode(bool toggle)
		{
			GameManager.Instance.IdleMode = toggle;
			//_cameraApplyChanges.gameObject.GetComponent<CameraController>().IsIdle = toggle;
		}

		private void Awake()
		{
            _cameraApplyChanges = FindFirstObjectByType<CameraApplyChanges>();
			_settingsPanel = SettingsManager.SettingsPanel;
			SettingsManager.SetUpCamera();
			SettingsManager.GameSettingData(_cameraApplyChanges.UpdateCameraSettings);
			SaveManager.SetAutosaveTime(SettingsManager.AutosaveTimeIntervals[SettingsManager.CurrentSettings.autosaveTime] * 60.0f);
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
					SettingsManager.ToggleSettingsPanel();
				}
				if (_mainMenuPanel.activeSelf)
				{
					ToggleMainMenuPanel();
				}
			}
		}
	}
}