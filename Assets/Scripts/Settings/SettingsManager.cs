using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using UserInterface;

namespace Settings
{
    [Manager]
    public class SettingsManager
    {
        [ManagedField]
        private static string _settingsFilePath;

        [ManagedField]
        private static SettingsData _currentSettings;
        public static SettingsData CurrentSettings => _currentSettings;

        [ManagedField]
        private static GameObject _connectionTab;
        [ManagedField]
        private static GameObject _settingsPanel;
        [ManagedField]
        private static GameObject _confirmSettingsPanel;
        [ManagedField]
        private static Camera _camera;
        [ManagedField]
        private static UniversalAdditionalCameraData _cameraData;
        [ManagedField]
        private static GameObject[] _settingPages;

        [ManagedField]
        public static float[] AutosaveTimeIntervals = { 5f, 10f, 15f, 30f, 60f };

        [ManagerInitialization]
        public static void Initialize()
        {
            _settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
            Debug.Log(_settingsFilePath);
            LoadSettings();
        }

        public static void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(SettingsManager.CurrentSettings, Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }

        public static void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                string json = File.ReadAllText(_settingsFilePath);
                _currentSettings = JsonConvert.DeserializeObject<SettingsData>(json);
            }
            else
            {
                _currentSettings = new SettingsData(); // Default settings
                SaveSettings();
            }
        }

        public static void TogglingConnectionTab(bool enabled)
        {
            if (_connectionTab != null)
            {
                _connectionTab.SetActive(enabled);
            }
        }

        public static void SetUpCamera()
        {
            if (Camera.main != null)
            {
                _camera = Camera.main;
                _cameraData = _camera.GetUniversalAdditionalCameraData();
                LoadSettings();
            }
        }

        public static void ChangeTab(int v)
        {
            if (_settingPages != null && _settingPages.Length > v)
            {
                foreach (var tab in _settingPages)
                {
                    tab.SetActive(false);
                }
                _settingPages[v].SetActive(true);
            }
        }

        public static void SetChannelName(string name)
        {
            if (name != null)
            {
                CurrentSettings.channelName = name;
                SaveSettings();
            }
            else
            {
                Debug.LogWarning("Name cannot be null!");
            }
        }

        public static void ToggleSettingsPanel()
        {
            if (SettingsManager.CurrentSettings != null)
            {
                _settingsPanel.SetActive(!_settingsPanel.activeSelf);
                if (_settingsPanel.activeSelf == false)
                {
                    ChangeTab(0);
                }
            }
            else
            {
                if (_settingsPanel.activeSelf == true)
                {
                    _confirmSettingsPanel.SetActive(true);
                }
                else
                {
                    _settingsPanel.SetActive(true);
                }
            }
        }

        public static void ConfirmSettings()
        {
            SaveSettings();
            LoadSettings();
            if (_confirmSettingsPanel != null)
            {
                _confirmSettingsPanel.SetActive(false);
            }
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }

        public static void CloseSettingPanel()
        {
            LoadSettings();
            if (_confirmSettingsPanel != null)
            {
                _confirmSettingsPanel.SetActive(false);
            }
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }

        public static GameObject SettingsPanel => _settingsPanel;

        public static void GameSettingData(Action<Camera> applyChanges)
        {
            if (_camera != null)
            {
                applyChanges(_camera);
            }
        }

        [Serializable]
        public class SettingsData
        {
            public int preset;
            public int displayMode;
            public int resolution;
            public int shadowType;
            public int shadowResolution;
            public bool enabledAO;
            public bool vSync;
            public int fpsLimiter;
            public float brightness;
            public float gamma;
            public float masterVolume;
            public float musicVolume;
            public float playerVolume;
            public float environmentVolume;
            public float panSensitivity;
            public float zoomSensitivity;
            public float wasdSensitivity;
            public float borderDetectionSensitivity;
            public int fov = 75;
            public int autosaveTime;
            public UsernameDisplayOption displayName;
            public AntialiasingMode camAA;
            public int displayBuildings;
            public bool borderDetection;
            public bool mouseControls;
            public string channelName;
        }
    }
}