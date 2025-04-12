using MetaData;
using SavingAndLoading;
using Scriptables;
using Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UserInterface.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private int _sceneIndex = 0;

        [SerializeField] private Button _loadButton;
        [SerializeField] private GameObject _channelNameUI;

        private GameObject _loadingScreen;
        private bool _loading = false;

        private LoadingManager _loadingManager;
        private MetaData.MetaData _metaData;
        private LoadType _loadType;
        private string _channelName;

        public void ConfirmChannelName()
        {
            if (!string.IsNullOrEmpty(_channelName))
            {
                SettingsManager.SetChannelName(_channelName);
                SettingsManager.SaveSettings();

                _loading = true;
                _metaData.LoadType = _loadType;
                SettingsManager.TogglingConnectionTab(false);

                ShowLoadingScreen();
                _loadingManager.LoadWorldScene(_sceneIndex);
            }
        }

        public void SetChannelName(string name)
        {
            _channelName = name.ToLower();
        }

        public void ToggleChannelName()
        {
            _channelNameUI.SetActive(!_channelNameUI.activeSelf);
        }

        public void GenerateWorld()
        {
            if (_loading) return;

            if (!string.IsNullOrEmpty(SettingsManager.CurrentSettings.channelName))
            {
                _loading = true;
                _metaData.LoadType = LoadType.Generate;
                Debug.Log("Generating World");
                SettingsManager.TogglingConnectionTab(false);

                ShowLoadingScreen();
                _loadingManager.LoadWorldScene(_sceneIndex);
            }
            else
            {
                _loadType = LoadType.Generate;
                ToggleChannelName();
            }
        }

        public void LoadWorld()
        {
            if (_loading) return;

            if (!string.IsNullOrEmpty(SettingsManager.CurrentSettings.channelName))
            {
                _loading = true;
                _metaData.LoadType = LoadType.Load;
                Debug.Log("Loading World");
                SettingsManager.TogglingConnectionTab(false);

                ShowLoadingScreen();
                _loadingManager.LoadWorldScene(_sceneIndex);
            }
            else
            {
                _loadType = LoadType.Load;
                ToggleChannelName();
            }
        }

        public void LoadCredits(int creditsSceneIndex)
        {
            _loadingManager.LoadNonWorldScenes(creditsSceneIndex);
        }

        public void OptionMenuToggle()
        {
            SettingsManager.SettingsPanel.SetActive(!SettingsManager.SettingsPanel.activeSelf);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Awake()
        {
            _loadingManager = FindAnyObjectByType<LoadingManager>();
            _metaData = MetaData.MetaData.Instance;

            if (_metaData == null)
            {
                Debug.LogWarning("MetaData instance not found! Make sure it exists in the scene or is created at runtime.");
            }

            _loadingScreen = GameObject.FindWithTag("LoadingScreen");
            if (_loadingScreen == null)
            {
                Debug.LogWarning("Loading screen GameObject not found. Make sure it's tagged 'LoadingScreen' and present in the scene.");
            }

            SettingsManager.LoadSettings();
        }

        private void Start()
        {
            if (!GameIO.DoesSaveFileExist(GameIO.SaveFileType.GameSave))
            {
                _loadButton.interactable = false;
                _loadButton.image.color = new Color(191f / 255f, 191f / 255f, 191f / 255f, 1f);
            }
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (SettingsManager.SettingsPanel.activeSelf)
                {
                    SettingsManager.ToggleSettingsPanel();
                }
            }
        }

        private void ShowLoadingScreen()
        {
            if (_loadingScreen != null)
                _loadingScreen.SetActive(true);
        }

        private void HideLoadingScreen()
        {
            if (_loadingScreen != null)
                _loadingScreen.SetActive(false);
        }
    }
}
