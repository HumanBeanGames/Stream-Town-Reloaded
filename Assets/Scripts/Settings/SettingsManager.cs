using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;
using UnityEngine.UI;
using URP;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;
using System.Collections.Generic;
using TMPro;
using System.IO;
using SavingAndLoading;
using PlayerControls;
using Managers;

namespace Settings
{
	public class SettingsManager : MonoBehaviour
	{
		[SerializeField]
		private List<int> _autosaveTimeIntervals;
		//Render pipeline
		[SerializeField]
		private UniversalRenderPipelineAsset _renderPipeline;

		[SerializeField]
		private SettingPreset[] _settingPreset;

		[SerializeField]
		private UniversalRendererData _forwardRenderer;

		//Post processing
		[SerializeField]
		private Volume _postProcessVolume;

		//Audio
		[SerializeField]
		private AudioMixer _mixer;

		//Camera AA
		[Space, Header("Camera settings")]
		[SerializeField]
		private UniversalAdditionalCameraData _cameraData;

		[SerializeField]
		private Camera _camera;

		//UI
		[Space, Header("UI")]

		[SerializeField]
		private GameObject _settingsPanel;

		[SerializeField]
		private SettingPages _settingPages;

		[SerializeField]
		private GameObject _confirmSettingsPanel;

		//VIDEO UI
		[Space, Header("Video Settings")]

		[SerializeField]
		private List<GameObject> _presetButtons;

		[SerializeField]
		private TMP_Dropdown _resolutionDropdown;

		[SerializeField]
		private TMP_Dropdown _displayModeDropdown;

		[SerializeField]
		private TMP_Dropdown _shadowTypeDropdown;

		[SerializeField]
		private TMP_Dropdown _shadowQualityDropdown;

		[SerializeField]
		private Toggle _vSyncToggle;

		[SerializeField]
		private TMP_Dropdown _fpsLimiterDropdown;

		[SerializeField]
		private TMP_Dropdown _AADropdown;

		[SerializeField]
		private TMP_Dropdown _cameraAADropdown;

		[SerializeField]
		private Toggle _AOToggle;

		[SerializeField]
		private Slider _brightnessSlider;

		[SerializeField]
		private Slider _gammaSlider;
		//AUDIO UI
		[Space, Header("Audio Settings")]

		[SerializeField]
		private TextMeshProUGUI _masterVolumeText;

		[SerializeField]
		private Slider _masterVolumeSlider;

		[SerializeField]
		private TextMeshProUGUI _musicVolumeText;

		[SerializeField]
		private Slider _musicVolumeSlider;

		[SerializeField]
		private TextMeshProUGUI _soundEffectsVolumeText;

		[SerializeField]
		private Slider _soundEffectsVolumeSlider;

		[SerializeField]
		private TextMeshProUGUI _ambienceVolumeText;

		[SerializeField]
		private Slider _ambienceVolumeSlider;

		//GAMEPLAY SETTINGS
		[Space, Header("Gameplay Settings")]

		[SerializeField]
		private TextMeshProUGUI _panningSensitivityText;

		[SerializeField]
		private Slider _panningSensitivitySlider;

		[SerializeField]
		private TextMeshProUGUI _zoomingSensitivityText;

		[SerializeField]
		private Slider _zoomingSensitivitySlider;

		[SerializeField]
		private TextMeshProUGUI _wasdSensitivityText;

		[SerializeField]
		private Slider _wasdSensitivitySlider;

		[SerializeField]
		private TextMeshProUGUI _borderDetectionSensitivityText;

		[SerializeField]
		private Slider _borderDetectionSensitivitySlider;

		[SerializeField]
		private TextMeshProUGUI _fovLevelText;

		[SerializeField]
		private Slider _fovLevelSlider;

		[SerializeField]
		private TMP_Dropdown _autosaveTimerDropdown;

		[SerializeField]
		private Toggle _borderDetectionToggle;

		[SerializeField]
		private Toggle _mouseControlsToggle;

		[SerializeField]
		private TMP_Dropdown _displayNameDropdown;

		[SerializeField]
		private TMP_Dropdown _displayBuildingDropdown;

		//TWITCH SETTINGS
		[Space, Header("Twitch Settings")]

		[SerializeField]
		private GameObject _connectionTab;

		[SerializeField]
		private TMP_InputField _channelNameInput;

		//private variables
		private Resolution[] _resolutions;

		private LiftGammaGain _gammaAndBrightness;

		private bool _presetChange;

		private SavePreset _savePreset;

		private bool _savedData;
		private bool _loadingData;
		private bool _apply;
		//save variables
		private int _preset;

		private int _displayMode;
		private int _resolution;

		private int _shadowType;
		private int _shadowResolution;//Shadow Res

		private bool _enabledAO; // Bool

		private bool _vSync; // Bool
		private int _fpsLimiter;

		private float _brightness;
		private float _gamma;

		private int _antiAliasing;
		private int _cameraAA;

		private float _masterVolume;
		private float _musicVolume;
		private float _playerVolume;
		private float _environmentVolume;

		private float _panSensitivity;
		private float _zoomSensitivity;
		private float _wasdSensitivity;
		private float _borderDetectionSensitivity;

		private int _fov;

		private int _autosaveTime;

		private int _displayName;
		private int _displayBuildings;

		private bool _borderDetection;
		private bool _mouseControls;

		private string _channelName;

		private CameraApplyChanges _cameraApplyChanges;
		public GameObject SettingsPanel => _settingsPanel;
		public List<int> AutosaveTimeIntervals => _autosaveTimeIntervals;

		[System.Serializable]
		class SettingPreset
		{
			public int antiAliasing;
			public int shadowType;
			public int shadowResolution;
			public bool vSync;
			public bool enabledAO;
			public int cameraAA;
			public SettingPreset(int _antiAliasing, int _shadowType, int _shadowResolution, bool _vSync, bool _enableAO, int _cameraAA)
			{
				antiAliasing = _antiAliasing;
				shadowType = _shadowType;
				shadowResolution = _shadowResolution;
				vSync = _vSync;
				enabledAO = _enableAO;
				cameraAA = _cameraAA;
			}
		}
		[System.Serializable]
		class SettingPages
		{
			public List<GameObject> UIPanels;
			public List<GameObject> tabs;

			public SettingPages(List<GameObject> UIPanels, List<GameObject> tabs)
			{
				this.UIPanels = UIPanels;
				this.tabs = tabs;
			}
		}
		[System.Serializable]
		public class SavePreset
		{
			//VIDEO SETTINGS

			public int preset;

			public int displayMode;
			public int resoultion;

			public int shadowType;
			public int shadowResolution;//Shadow Res

			public bool enabledAO; // Bool

			public bool vSync; // Bool
			public int fpsLimiter;

			public float brightness;
			public float gamma;

			public int antiAliasing;
			public int cameraAA;

			//AUDIO SETTINGS

			public float masterVolume;
			public float musicVolume;
			public float playerVolume;
			public float environmentVolume;

			//GAMEPLAY SETTINGS

			public float panSensitivity;
			public float zoomSensitivity;
			public float wasdSensitivity;
			public float borderDetectionSensitivity;
			public int fov;

			public int autosaveTime;

			public int displayName;
			public int displayBuildings;

			public bool keyboardMovement;
			public bool mouseMovement;

			public string channelName;
		}

		//UI Functionality
		public void ChangeTab(int v)
		{
			//Enables and disables tabs
			List<GameObject> tabs = new List<GameObject>(_settingPages.tabs);

			tabs.RemoveAt(v);

			for (int i = 0; i < tabs.Count; i++)
			{
				tabs[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			}

			_settingPages.tabs[v].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

			//Enables and disables pages

			List<GameObject> pages = new List<GameObject>(_settingPages.UIPanels);

			pages.RemoveAt(v);

			for (int i = 0; i < pages.Count; i++)
			{
				pages[i].SetActive(false);
			}

			_settingPages.UIPanels[v].SetActive(true);
		}

		/// <summary>
		/// VIDEO SETTINGS
		/// </summary>

		public void PresetOnChange(int v)
		{
			if (v < 5)
			{
				_presetChange = true;

				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				ShadowsOnChange(_settingPreset[v].shadowType);
				ShadowQualityOnChange(_settingPreset[v].shadowResolution);
				AAOnChange(_settingPreset[v].antiAliasing);
				CameraAAOnChange(_settingPreset[v].cameraAA);
				AOToggle(_settingPreset[v].enabledAO);
				VSyncToggle(_settingPreset[v].vSync);
				_preset = v;
				_presetButtons[v].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				_presetChange = false;

				if (!_loadingData)
				{
					_savedData = false;
				}
			}
		}

		public void AAOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_AADropdown.value = v;
			if (_apply)
			{
				switch (v)
				{
					case 1:
						_renderPipeline.msaaSampleCount = 2;
						break;
					case 2:
						_renderPipeline.msaaSampleCount = 4;
						break;
					case 3:
						_renderPipeline.msaaSampleCount = 8;
						break;
					default:
						_renderPipeline.msaaSampleCount = 0;
						break;
				}
			}
			_antiAliasing = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}
		public void CameraAAOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_cameraAADropdown.value = v;
			if (_camera)
				if (_apply)
				{
					switch (v)
					{
						case 1:
							_cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
							break;
						case 2:
							_cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
							break;
						default:
							_cameraData.antialiasing = AntialiasingMode.None;
							break;
					}

					_cameraData.antialiasingQuality = AntialiasingQuality.High;
				}
			_cameraAA = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		//Shadow settings
		public void ShadowQualityOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_shadowQualityDropdown.value = v;

			if (_apply)
			{
				switch (v)
				{
					case 1:
						UnityGraphics.MainLightShadowResolution = ShadowResolution._512;
						UnityGraphics.AdditionalLightShadowResolution = ShadowResolution._512;
						break;
					case 2:
						UnityGraphics.MainLightShadowResolution = ShadowResolution._1024;
						UnityGraphics.AdditionalLightShadowResolution = ShadowResolution._1024;
						break;
					case 3:
						UnityGraphics.MainLightShadowResolution = ShadowResolution._2048;
						UnityGraphics.AdditionalLightShadowResolution = ShadowResolution._2048;
						break;
					case 4:
						UnityGraphics.MainLightShadowResolution = ShadowResolution._4096;
						UnityGraphics.AdditionalLightShadowResolution = ShadowResolution._4096;
						break;
					default:
						UnityGraphics.MainLightShadowResolution = ShadowResolution._256;
						UnityGraphics.AdditionalLightShadowResolution = ShadowResolution._256;
						break;
				}
			}
			_shadowResolution = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void ShadowsOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_shadowTypeDropdown.value = v;
			if (_apply)
			{
				switch (v)
				{
					case 1: // hard shadows
						UnityGraphics.MainLightCastShadows = true;
						UnityGraphics.AdditionalLightCastShadows = true;
						UnityGraphics.SoftShadowsEnabled = false;
						break;
					case 2: // soft shadows
						UnityGraphics.MainLightCastShadows = true;
						UnityGraphics.AdditionalLightCastShadows = true;
						UnityGraphics.SoftShadowsEnabled = true;
						break;
					default: // no shadows
						UnityGraphics.AdditionalLightCastShadows = false;
						UnityGraphics.MainLightCastShadows = false;
						break;
				}
			}

			switch (v)
			{
				case 1: // hard shadows
					_shadowQualityDropdown.interactable = true;
					break;
				case 2: // soft shadows
					_shadowQualityDropdown.interactable = true;
					break;
				default: // no shadows
					_shadowQualityDropdown.interactable = false;
					break;
			}

			_shadowType = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		//FPS modifiers
		public void VSyncToggle(bool vSyncEnabled)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_vSyncToggle.isOn = vSyncEnabled;

			if (_apply)
			{
				if (vSyncEnabled)
				{
					QualitySettings.vSyncCount = 1;
				}
				else
				{
					QualitySettings.vSyncCount = 0;
				}
			}

			_vSync = vSyncEnabled;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void FPSOnChange(int v)
		{
			_fpsLimiterDropdown.value = v;

			if (_apply)
			{
				switch (v)
				{
					case 0:
						Application.targetFrameRate = 24;
						break;
					case 1:
						Application.targetFrameRate = 30;
						break;
					case 2:
						Application.targetFrameRate = 60;
						break;
					case 3:
						Application.targetFrameRate = 120;
						break;
					case 4:
						Application.targetFrameRate = 240;
						break;
					default:
						Application.targetFrameRate = -1;
						break;
				}
			}

			_fpsLimiter = v;


			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void AOToggle(bool toggle)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Count; i++)
				{
					_presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_AOToggle.isOn = toggle;

			if (_apply)
			{
				_forwardRenderer.rendererFeatures[0].SetActive(toggle);
			}

			_enabledAO = toggle;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void DisplayModeOnChange(int v)
		{
			_displayModeDropdown.value = v;

			if (_apply)
			{
				switch (v)
				{
					case 1:
						Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
						break;
					case 2:
						Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
						break;
					default:
						Screen.fullScreenMode = FullScreenMode.Windowed;
						break;
				}
			}

			_displayMode = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void ResoultionOnChange(int v)
		{
			_resolutionDropdown.value = v;

			Resolution resolution = _resolutions[v];

			if (_apply)
			{
				Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
			}

			_resolution = v;

			_savedData = false;
		}

		//Post processing effects
		public void GammaOnChange(float v)
		{
			_gammaSlider.value = v;

			if (_apply)
			{
				_gammaAndBrightness.gamma.value = new Vector4(1, 1, 1, v);
			}

			_gamma = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void BrightnessOnChange(float v)
		{
			_brightnessSlider.value = v;

			if (_apply)
			{
				_gammaAndBrightness.gain.value = new Vector4(1, 1, 1, v);
			}

			_brightness = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		/// <summary>
		/// AUDIO SETTINGS
		/// </summary>

		public void MasterVolumeOnChange(float v)
		{
			_masterVolumeSlider.value = v;
			_masterVolumeText.text = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_masterVolume", Mathf.Log10(v) * 40);


			_masterVolume = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void MusicVolumeOnChange(float v)
		{
			_musicVolumeSlider.value = v;
			_musicVolumeText.text = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_musicVolume", Mathf.Log10(v) * 40);


			_musicVolume = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void SoundEffectsVolumeOnChange(float v)
		{
			_soundEffectsVolumeSlider.value = v;
			_soundEffectsVolumeText.text = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_soundEffectsVolume", Mathf.Log10(v) * 40);


			_playerVolume = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void AmbienceVolumeOnChange(float v)
		{
			_ambienceVolumeSlider.value = v;
			_ambienceVolumeText.text = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_ambienceVolume", Mathf.Log10(v) * 40);


			_environmentVolume = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		/// <summary>
		/// GAMEPLAY SETTINGS
		/// </summary>

		public void PanSensitivityOnChange(float v)
		{
			_panningSensitivitySlider.value = v;
			_panningSensitivityText.text = (v / 10).ToString("F1");
			_panSensitivity = v;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().PanSensitivity = v;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void ZoomSensitivityOnChange(float v)
		{
			_zoomingSensitivitySlider.value = v;
			_zoomingSensitivityText.text = (v / 10).ToString("F1");
			_zoomSensitivity = v;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().ZoomSensitivity = v;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void WASDSensitivityOnChange(float v)
		{
			_wasdSensitivitySlider.value = v;
			_wasdSensitivityText.text = (v / 10).ToString("F1");
			_wasdSensitivity = v;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().WasdSensitivity = v;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void BorderDetectionSensitivityOnChange(float v)
		{
			_borderDetectionSensitivitySlider.value = v;
			_borderDetectionSensitivityText.text = (v / 10).ToString("F1");
			_borderDetectionSensitivity = v;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().BorderDetectionSensitivity = v;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void FOVOnChange(float v)
		{
			_fovLevelSlider.value = v;
			_fovLevelText.text = ((int)v).ToString();

			if (_apply)
			{
				if (_camera)
				{
					_camera.fieldOfView = (int)v;
				}
			}

			_fov = (int)v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void AutoSaveTimer(int v)
		{
			_autosaveTimerDropdown.value = v;
			_autosaveTime = v;

			if (_apply)
			{
				if (GameManager.Instance)
					GameManager.Instance.SaveManager.SetAutosaveTime(_autosaveTimeIntervals[_autosaveTime] * 60.0f);
			}
			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void DisplayUserNames(int v)
		{
			_displayNameDropdown.value = v;
			_displayName = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void DisplayBuildingData(int v)
		{
			_displayBuildingDropdown.value = v;
			_displayBuildings = v;

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void ToggleBorderDectionMovement(bool toggle)
		{
			_borderDetectionToggle.isOn = toggle;

			_borderDetection = toggle;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().BorderDetection = toggle;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void ToggleMouseControls(bool toggle)
		{
			_mouseControlsToggle.isOn = toggle;

			_mouseControls = toggle;

			if (_apply)
			{
				if (_camera)
				{
					if (_camera.GetComponent<CameraController>())
					{
						_camera.GetComponent<CameraController>().MouseControls = toggle;
					}
				}
			}

			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		public void SetChannelName(string name)
		{
			_channelName = name.ToLower();
			_channelNameInput.text = name;
			if (!_loadingData)
			{
				_savedData = false;
			}
		}

		/// <summary>
		/// Update the data with the game scene
		/// </summary>
		public void GameSettingData(CameraApplyChanges cameraApplyChanges)
		{
			_cameraApplyChanges = cameraApplyChanges;
		}

		/// <summary>
		/// Disables and enables the connection tab in world game
		/// </summary>
		public void TogglingConnectionTab(bool enabled)
		{
			_connectionTab.SetActive(enabled);
		}

		/// <summary>
		/// Enable settings panel
		/// </summary>

		public void ToggleSettingsPanel()
		{
			if (_savedData)
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

		public void ConfirmSettings()
		{
			SaveSettings();
			LoadSettings();
			ChangeTab(0);
			_confirmSettingsPanel.SetActive(false);
			_settingsPanel.SetActive(false);
		}

		public void CloseSettingPanel()
		{
			ChangeTab(0);
			LoadSettings();
			_confirmSettingsPanel.SetActive(false);
			_settingsPanel.SetActive(false);
		}

		/// <summary>
		/// Saving and loading
		/// </summary>

		public void SaveSettings()
		{
			SetUpSavePreset();
			ApplySettings();
			GameIO.SaveSettingsData(_savePreset);

			UpdateSettingScriptableObject();

			_savedData = true;
		}

		public void LoadSettings()
		{
			if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly/Stream Town/SettingsData.json"))
			{
				_savePreset = GameIO.LoadSettingsData();
				_loadingData = true;
				_apply = true;
				//Visual

				ShadowsOnChange(_savePreset.shadowType);
				ShadowQualityOnChange(_savePreset.shadowResolution);
				AAOnChange(_savePreset.antiAliasing);
				CameraAAOnChange(_savePreset.cameraAA);
				AOToggle(_savePreset.enabledAO);
				VSyncToggle(_savePreset.vSync);

				PresetOnChange(_savePreset.preset);


				FPSOnChange(_savePreset.fpsLimiter);
				ResoultionOnChange(_savePreset.resoultion);
				DisplayModeOnChange(_savePreset.displayMode);
				GammaOnChange(_savePreset.gamma);
				BrightnessOnChange(_savePreset.brightness);

				//Audio
				MasterVolumeOnChange(_savePreset.masterVolume);
				MusicVolumeOnChange(_savePreset.musicVolume);
				SoundEffectsVolumeOnChange(_savePreset.playerVolume);
				AmbienceVolumeOnChange(_savePreset.environmentVolume);

				//Gameplay
				PanSensitivityOnChange(_savePreset.panSensitivity);
				ZoomSensitivityOnChange(_savePreset.zoomSensitivity);
				WASDSensitivityOnChange(_savePreset.wasdSensitivity);
				BorderDetectionSensitivityOnChange(_savePreset.borderDetectionSensitivity);
				FOVOnChange(_savePreset.fov);
				DisplayBuildingData(_savePreset.displayBuildings);
				DisplayUserNames(_savePreset.displayName);
				AutoSaveTimer(_savePreset.autosaveTime);
				ToggleMouseControls(_savePreset.mouseMovement);
				ToggleBorderDectionMovement(_savePreset.keyboardMovement);
				//Twitch
				SetChannelName(_savePreset.channelName);

				UpdateSettingScriptableObject();

				if (_cameraApplyChanges)
					_cameraApplyChanges.UpdateCameraSettings();

				_loadingData = false;
				_savedData = true;
				_apply = false;

				if (GameManager.Instance)
					GameManager.Instance.SaveManager.SetAutosaveTime(_autosaveTimeIntervals[_autosaveTime] * 60.0f);
			}
			else
			{
				Debug.Log("No setting file located");
				SetUpSettingsDataFile();
			}
		}

		private void UpdateSettingScriptableObject()
		{
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").panSensitivity = _panSensitivity;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").zoomSensitivity = _zoomSensitivity;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").wasdSensitivity = _wasdSensitivity;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").borderDetectionSensitivity = _borderDetectionSensitivity;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").mouseControls = _mouseControls;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").borderDetection = _borderDetection;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").camAA = _cameraAA;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").fov = _fov;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").displayBuildings = _displayBuildings;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").displayName = _displayName;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").channelName = _channelName;
			Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData").autosaveTime = _autosaveTime;
		}

		private void ApplySettings()
		{
			_apply = true;
			//Video
			ShadowsOnChange(_shadowType);
			ShadowQualityOnChange(_shadowResolution);
			AAOnChange(_antiAliasing);
			CameraAAOnChange(_cameraAA);
			AOToggle(_enabledAO);
			VSyncToggle(_vSync);
			PresetOnChange(_preset);
			FPSOnChange(_fpsLimiter);
			ResoultionOnChange(_resolution);
			DisplayModeOnChange(_displayMode);
			GammaOnChange(_gamma);
			BrightnessOnChange(_brightness);

			//Audio
			MasterVolumeOnChange(_masterVolume);
			MusicVolumeOnChange(_musicVolume);
			SoundEffectsVolumeOnChange(_playerVolume);
			AmbienceVolumeOnChange(_environmentVolume);

			//Gameplay
			PanSensitivityOnChange(_panSensitivity);
			ZoomSensitivityOnChange(_zoomSensitivity);
			WASDSensitivityOnChange(_wasdSensitivity);
			BorderDetectionSensitivityOnChange(_borderDetectionSensitivity);
			FOVOnChange(_fov);
			DisplayBuildingData(_displayBuildings);
			DisplayUserNames(_displayName);
			AutoSaveTimer(_autosaveTime);
			ToggleMouseControls(_mouseControls);
			ToggleBorderDectionMovement(_borderDetection);
			

			//Twitch
			SetChannelName(_channelName);

			if (GameManager.Instance)
				GameManager.Instance.SaveManager.SetAutosaveTime(_autosaveTimeIntervals[_autosaveTime] * 60.0f);
			_apply = false;
		}

		/// <summary>
		/// SET UP FUNCTIONS
		/// </summary>

		private void SetUpPipelineAndPostProcessing()
		{
			_postProcessVolume = GetComponentInChildren<Volume>();
			_postProcessVolume.profile.TryGet(out _gammaAndBrightness);

			GraphicsSettings.defaultRenderPipeline = _renderPipeline;
			QualitySettings.renderPipeline = _renderPipeline;
		}

		private void SetUpResoultion()
		{
			List<Resolution> res = new List<Resolution>();
			_resolutions = Screen.resolutions;

			_resolutionDropdown.ClearOptions();

			List<string> options = new List<string>();

			int currentResolutionIndex = 0;

			for (int i = 0; i < _resolutions.Length; i++)
			{
				string option = _resolutions[i].width + " x " + _resolutions[i].height;

				if (!options.Contains(option))
				{
					options.Add(option);
					res.Add(_resolutions[i]);
				}

				if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
				{
					currentResolutionIndex = res.Count - 1;
				}
			}

			_resolutionDropdown.AddOptions(options);
			_resolutionDropdown.value = currentResolutionIndex;
			_resolution = currentResolutionIndex;
			_resolutionDropdown.RefreshShownValue();
			_resolutions = res.ToArray();
		}

		private void SetUpSavePreset()
		{
			_savePreset = new SavePreset();

			_savePreset.preset = _preset;

			_savePreset.displayMode = _displayMode;
			_savePreset.resoultion = _resolution;

			_savePreset.shadowType = _shadowType;
			_savePreset.shadowResolution = _shadowResolution;

			_savePreset.enabledAO = _enabledAO;

			_savePreset.vSync = _vSync;
			_savePreset.fpsLimiter = _fpsLimiter;

			_savePreset.brightness = _brightness;
			_savePreset.gamma = _gamma;

			_savePreset.antiAliasing = _antiAliasing;
			_savePreset.cameraAA = _cameraAA;

			_savePreset.masterVolume = _masterVolume;
			_savePreset.musicVolume = _musicVolume;
			_savePreset.playerVolume = _playerVolume;
			_savePreset.environmentVolume = _environmentVolume;

			_savePreset.panSensitivity = _panSensitivity;
			_savePreset.zoomSensitivity = _zoomSensitivity;
			_savePreset.wasdSensitivity = _wasdSensitivity;
			_savePreset.borderDetectionSensitivity = _borderDetectionSensitivity;
			_savePreset.fov = _fov;

			_savePreset.autosaveTime = _autosaveTime;

			_savePreset.displayName = _displayName;
			_savePreset.displayBuildings = _displayBuildings;

			_savePreset.keyboardMovement = _borderDetection;
			_savePreset.mouseMovement = _mouseControls;

			_savePreset.channelName = _channelName;
		}

		// Setting all the settings values to the UI values
		private void SetUpSettingsDataFile()
		{
			Debug.Log("Creating settings save file");

			_preset = 5;

			_displayMode = _displayModeDropdown.value;

			_shadowType = _shadowTypeDropdown.value;
			_shadowResolution = _shadowQualityDropdown.value;

			_enabledAO = _AOToggle.isOn;

			_vSync = _vSyncToggle.isOn;
			_fpsLimiter = _fpsLimiterDropdown.value;

			_brightness = _brightnessSlider.value;
			_gamma = _gammaSlider.value;

			_antiAliasing = _AADropdown.value;
			_cameraAA = _cameraAADropdown.value;

			_masterVolume = _masterVolumeSlider.value;
			_musicVolume = _musicVolumeSlider.value;
			_playerVolume = _soundEffectsVolumeSlider.value;
			_environmentVolume = _ambienceVolumeSlider.value;

			_panSensitivity = _panningSensitivitySlider.value;
			_zoomSensitivity = _zoomingSensitivitySlider.value;
			_wasdSensitivity = _wasdSensitivitySlider.value;
			_borderDetectionSensitivity = _borderDetectionSensitivitySlider.value;

			_fov = (int)_fovLevelSlider.value;

			_autosaveTime = _autosaveTimerDropdown.value;

			_displayName = _displayNameDropdown.value;
			_displayBuildings = _displayBuildingDropdown.value;

			_borderDetection = _borderDetectionToggle.isOn;
			_mouseControls = _mouseControlsToggle.isOn;

			_channelName = _channelNameInput.text;

			SaveSettings();

			LoadSettings();
			Debug.Log("Created and loaded new settings save file");
		}

		public void SetUpCamera()
		{
			_camera = new Camera();
			_cameraData = new UniversalAdditionalCameraData();
			if (Camera.main)
			{
				_camera = Camera.main;
				_cameraData = _camera.GetUniversalAdditionalCameraData();
				LoadSettings();
			}
		}

		/// <summary>
		/// UNITY FUNCTIONS
		/// </summary>

		private void Awake()
		{
			if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + GameIO.SAVE_FILEPATH))
			{
				Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + GameIO.SAVE_FILEPATH);
			}

			SetUpCamera();
			SetUpPipelineAndPostProcessing();
			SetUpResoultion();

			if (!File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly" + "/Stream Town" + "/SettingsData.json"))
			{
				SetUpSettingsDataFile();
			}

			LoadSettings();

			DontDestroyOnLoad(gameObject);
		}
	}
}