using UnityEngine;
using Settings;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PlayerControls;
using UserInterface.MainMenu;
using Managers;
using SavingAndLoading;
using System.Collections;
using Sirenix.OdinInspector;

namespace UserInterface 
{
	[GameManager]
    public static class UI_GameMenuManager 
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static UI_GameMenuConfig Config = UI_GameMenuConfig.Instance;

        [HideInInspector]
		private static GameObject _gameMenu;

		[HideInInspector]
		private static GameObject _savePanel;

		[HideInInspector]
		private static GameObject _loadPanel;

		[HideInInspector]
		private static GameObject _mainMenuPanel;
		
		[HideInInspector]
		private static GameObject _settingsPanel;
		[HideInInspector]
		private static SettingsManager _settingsManager;
		[HideInInspector]
		private static CameraApplyChanges _cameraApplyChanges;
		[HideInInspector]
		private static LoadingManager _loadingManager;
		[HideInInspector]
		private static bool _savedGame;

		private static GameObject GameMenu
		{
			get
			{
				if (_gameMenu == null)
				{
					_gameMenu = GameObject.FindWithTag("GameMenu");
				}
				return _gameMenu;
			}
		}

		private static GameObject MainMenuPanel
		{
			get
			{
				if (_mainMenuPanel == null)
				{
					_mainMenuPanel = GameObject.FindWithTag("ExitToMainMenu");
				}
				return _mainMenuPanel;
			}
		}

		public static bool SavedGame
        {
            get { return _savedGame; }
			set { _savedGame = value; }
		}
		public static void ToggleGameMenu()
		{
			GameMenu.SetActive(!GameMenu.activeSelf);
			_cameraApplyChanges.gameObject.GetComponent<CameraController>().enabled = !GameMenu.activeSelf;
		}

		public static void ToggleSettingsPanel()
		{
			_settingsPanel.SetActive(!_settingsPanel.activeSelf);
		}

		public static void ToggleSavePanel()
		{
			_savePanel.SetActive(!_savePanel.activeSelf);
		}

		public static void ToggleLoadPanel()
		{
			_loadPanel.SetActive(!_loadPanel.activeSelf);			
		}

		public static void ToggleMainMenuPanel()
		{
			MainMenuPanel.SetActive(!MainMenuPanel.activeSelf);
		}

		public static void QuitToMainMenu()
        {
			_settingsManager.TogglingConnectionTab(true);
			_loadingManager.LoadNonWorldScenes(1);
		}
		
		public static void ToggleIdleMode(bool toggle)
		{
			GameManager.Instance.IdleMode = toggle;
			//_cameraApplyChanges.gameObject.GetComponent<CameraController>().IsIdle = toggle;
		}

		private class Runner : MonoBehaviour {
            private void OnEnable()
            {
                DontDestroyOnLoad(this);
            }
        }

        [HideInInspector]
        private static Runner runner;

		private static Runner RunnerInstance
		{
			get
			{
				if (runner == null)
				{
					GameObject runnerObject = new GameObject("GameMenuRunner");
					runnerObject.hideFlags = HideFlags.HideAndDontSave;
					runner = runnerObject.AddComponent<Runner>();
				}
				return runner;
			}
		}

		public static Coroutine StartCoroutine(IEnumerator routine)
		{
			return RunnerInstance.StartCoroutine(routine);
		}

		public static void StopCoroutine(Coroutine coroutine)
		{
			RunnerInstance.StopCoroutine(coroutine);
		}

		private static IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				if (Keyboard.current.escapeKey.wasPressedThisFrame)
				{
					if (!MainMenuPanel.activeSelf && !_settingsPanel.activeSelf)
					{
						ToggleGameMenu();
					}
					if (_settingsPanel.activeSelf)
					{
						_settingsManager.ToggleSettingsPanel();
					}
					if (MainMenuPanel.activeSelf)
					{
						ToggleMainMenuPanel();
					}
				}
				yield return null;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InitializeOnLoad()
		{
			//REDO ALL THIS!!!
			_cameraApplyChanges = GameObject.FindAnyObjectByType<CameraApplyChanges>();
			_loadingManager = GameObject.FindAnyObjectByType<LoadingManager>();
			if (GameObject.FindAnyObjectByType<SettingsManager>())
			{
				_settingsManager = GameObject.FindAnyObjectByType<SettingsManager>();
				_settingsPanel = _settingsManager.SettingsPanel;
				_settingsManager.SetUpCamera();
				_settingsManager.GameSettingData(_cameraApplyChanges);
				if (GameManager.Instance)
					SaveManager.SetAutosaveTime(_settingsManager.AutosaveTimeIntervals[GameManager.Instance.SettingsData.autosaveTime] * 60.0f);
			}
			StartCoroutine(UpdateCoroutine());
		}
	}
}