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
using Reflex.Attributes;
using Scriptables;

namespace Settings
{
	public class SettingsManager : MonoBehaviour
	{
		//private variables
		private Resolution[] _resolutions;

		private LiftGammaGain _gammaAndBrightness;

		private bool _presetChange;

		private SavePreset _savePreset;

		//save variables

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

		private CameraApplyChanges _cameraApplyChanges; //PER SCENE

		/// <summary>
		/// VIDEO SETTINGS
		/// </summary>
		/// 
		private bool _apply = false;

		private int _preset = 0;

		[Inject] ProjectCamera _camera;
		[Inject] PresetButtons _presetButtons; //REVISIT TO CHANGE TYPE

        public void CameraAAOnChange(int v)
        {
            if (!_presetChange)
            {
                for (int i = 0; i < _presetButtons.Buttons.Count; i++)
                {
                    //REVISIT
                    _presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
                _preset = 5;
            }

            if (_camera.Exists)
                if (_apply)
                {
                    switch (v)
                    {
                        case 1:
                            _camera.Data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                            break;
                        case 2:
                            _camera.Data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                            break;
                        default:
                            _camera.Data.antialiasing = AntialiasingMode.None;
                            break;
                    }

                    _camera.Data.antialiasingQuality = AntialiasingQuality.High;
                }
            _cameraAA = v;

            SaveState.SafeSave();
        }

		[Inject] Access_PanningSensitivitySlider _panningSensitivitySlider;
		[Inject] Access_PanningSensitivityText _panningSensitivityText;
        public void PanSensitivityOnChange(float v)
        {
            _panningSensitivitySlider.val = v;
            _panningSensitivityText.val = (v / 10).ToString("F1");
            _panSensitivity = v;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().PanSensitivity = v;
                    }
                }
            }

            SaveState.SafeSave();
        }

		[Inject] Access_ZoomingSensitivitySlider _zoomingSensitivitySlider;
		[Inject] Access_ZoomingSensitivityText _zoomingSensitivityText;
        public void ZoomSensitivityOnChange(float v)
        {
            _zoomingSensitivitySlider.val = v;
            _zoomingSensitivityText.val = (v / 10).ToString("F1");
            _zoomSensitivity = v;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().ZoomSensitivity = v;
                    }
                }
            }

            SaveState.SafeSave();
        }

		[Inject] Access_WasdSensitivitySlider _wasdSensitivitySlider;
		[Inject] Access_WasdSensitivityText _wasdSensitivityText;
        public void WASDSensitivityOnChange(float v)
        {
            _wasdSensitivitySlider.val = v;
            _wasdSensitivityText.val = (v / 10).ToString("F1");
            _wasdSensitivity = v;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().WasdSensitivity = v;
                    }
                }
            }

            SaveState.SafeSave();
        }

		[Inject] Access_BorderDetectionSensitivitySlider _borderDetectionSensitivitySlider;
		[Inject] Access_BorderDetectionSensitivityText _borderDetectionSensitivityText;
        public void BorderDetectionSensitivityOnChange(float v)
        {
            _borderDetectionSensitivitySlider.val = v;
            _borderDetectionSensitivityText.val = (v / 10).ToString("F1");
            _borderDetectionSensitivity = v;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().BorderDetectionSensitivity = v;
                    }
                }
            }

            SaveState.SafeSave();
        }

		[Inject] Access_FOVLevelSlider _fovLevelSlider;
        [Inject] Access_FOVLevelText _fovLevelText;
        public void FOVOnChange(float v)
        {
            _fovLevelSlider.val = v;
            _fovLevelText.val = ((int)v).ToString();

            if (_apply)
            {
                if (_camera.Exists)
                {
                    _camera.Cam.fieldOfView = (int)v;
                }
            }

            _fov = (int)v;

            SaveState.SafeSave();
        }

		/*[Inject] Access_BorderDetectionToggle _borderDetectionToggle; //REVISIT - THERE IS NO TOGGLE
        public void ToggleBorderDectionMovement(bool toggle)
        {
            _borderDetectionToggle.isOn = toggle;

            _borderDetection = toggle;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().BorderDetection = toggle;
                    }
                }
            }

            SaveState.SafeSave();
        }*/

		[Inject] Access_MouseControlsToggle _mouseControlsToggle;
        public void ToggleMouseControls(bool toggle)
        {
            _mouseControlsToggle.isOn = toggle;

            _mouseControls = toggle;

            if (_apply)
            {
                if (_camera.Exists)
                {
                    if (_camera.Cam.GetComponent<CameraController>())
                    {
                        _camera.Cam.GetComponent<CameraController>().MouseControls = toggle;
                    }
                }
            }

            SaveState.SafeSave();
        }

        [Inject] SaveState SaveState;
		[Inject] SettingPreset[] _settingPreset;
		public void PresetOnChange(int v)
		{
			if (v < 5)
			{
				_presetChange = true;

				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				ShadowsOnChange(_settingPreset[v].shadowType);
				ShadowQualityOnChange(_settingPreset[v].shadowResolution);
				AAOnChange(_settingPreset[v].antiAliasing);
				//REVISIT
				//CameraAAOnChange(_settingPreset[v].cameraAA);
				AOToggle(_settingPreset[v].enabledAO);
				VSyncToggle(_settingPreset[v].vSync);
				_preset = v;
				_presetButtons.Buttons[v].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				_presetChange = false;

                SaveState.SafeSave();
            }
		}

        [Inject] UniversalRenderPipelineAsset _renderPipeline;
		[Inject] Access_AADropdown _AADropdown;
		public void AAOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_AADropdown.val = v;
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

            SaveState.SafeSave();
        }

		[Inject] Access_ShadowQualityDropdown _shadowQualityDropdown;
		//Shadow settings
		public void ShadowQualityOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_shadowQualityDropdown.val = v;

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

            SaveState.SafeSave();
        }

		[Inject] Access_ShadowTypeDropdown _shadowTypeDropdown;
		public void ShadowsOnChange(int v)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_shadowTypeDropdown.val = v;
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
					_shadowQualityDropdown.dropDown.interactable = true;
					break;
				case 2: // soft shadows
					_shadowQualityDropdown.dropDown.interactable = true;
					break;
				default: // no shadows
					_shadowQualityDropdown.dropDown.interactable = false;
					break;
			}

			_shadowType = v;

            SaveState.SafeSave();
        }

		[Inject] Access_VsyncToggle _vSyncToggle;
		//FPS modifiers
		public void VSyncToggle(bool vSyncEnabled)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
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

            SaveState.SafeSave();
        }

		[Inject] Access_FPSLimiterDropdown _fpsLimiterDropdown;
		public void FPSOnChange(int v)
		{
			_fpsLimiterDropdown.val = v;

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


            SaveState.SafeSave();
        }

		[Inject] private UniversalRendererData _forwardRenderer;
		[Inject] Access_AOToggle _AOToggle;
		public void AOToggle(bool toggle)
		{
			if (!_presetChange)
			{
				for (int i = 0; i < _presetButtons.Buttons.Count; i++)
				{
					_presetButtons.Buttons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				}
				_preset = 5;
			}

			_AOToggle.isOn = toggle;

			if (_apply)
			{
				_forwardRenderer.rendererFeatures[0].SetActive(toggle);
			}

			_enabledAO = toggle;

            SaveState.SafeSave();
        }

		[Inject] Access_DisplayModeDropdown _displayModeDropdown;
		public void DisplayModeOnChange(int v)
		{
			_displayModeDropdown.val = v;

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

            SaveState.SafeSave();
        }

		[Inject] Access_ResolutionDropdown _resolutionDropdown;
		public void ResolutionOnChange(int v)
		{
			_resolutionDropdown.val = v;

			Resolution resolution = _resolutions[v];

			if (_apply)
			{
				Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
			}

			_resolution = v;

            SaveState.SafeSave();
        }

		//Post processing effects
		[Inject] Access_GammaSlider _gammaSlider;
		public void GammaOnChange(float v)
		{
			_gammaSlider.val = v;

			if (_apply)
			{
				_gammaAndBrightness.gamma.value = new Vector4(1, 1, 1, v);
			}

			_gamma = v;

            SaveState.SafeSave();
        }

		[Inject] Access_BrightnessSlider _brightnessSlider;
		public void BrightnessOnChange(float v)
		{
			_brightnessSlider.val = v;

			if (_apply)
			{
				_gammaAndBrightness.gain.value = new Vector4(1, 1, 1, v);
			}

			_brightness = v;

            SaveState.SafeSave();
        }

		/// <summary>
		/// AUDIO SETTINGS
		/// </summary>
		[Inject] Access_MasterVolumeSlider _masterVolumeSlider;
		[Inject] Access_MasterVolumeText _masterVolumeText;
		[Inject] private AudioMixer _mixer;
		public void MasterVolumeOnChange(float v)
		{
			_masterVolumeSlider.val = v;
			_masterVolumeText.val = Mathf.RoundToInt(v * 50).ToString();

				
			_mixer.SetFloat("_masterVolume", Mathf.Log10(v) * 40);


			_masterVolume = v;

            SaveState.SafeSave();
        }

        [Inject] Access_MusicVolumeSlider _musicVolumeSlider;
        [Inject] Access_MusicVolumeText _musicVolumeText;
        public void MusicVolumeOnChange(float v)
		{
			_musicVolumeSlider.val = v;
			_musicVolumeText.val = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_musicVolume", Mathf.Log10(v) * 40);


			_musicVolume = v;

            SaveState.SafeSave();
        }

        [Inject] Access_SoundEffectsVolumeSlider _soundEffectsVolumeSlider;
        [Inject] Access_SoundEffectsVolumeText _soundEffectsVolumeText;
        public void SoundEffectsVolumeOnChange(float v)
		{
			_soundEffectsVolumeSlider.val = v;
			_soundEffectsVolumeText.val = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_soundEffectsVolume", Mathf.Log10(v) * 40);


			_playerVolume = v;

            SaveState.SafeSave();
        }

        [Inject] Access_AmbienceVolumeSlider _ambienceVolumeSlider;
        [Inject] Access_AmbienceVolumeText _ambienceVolumeText;
        public void AmbienceVolumeOnChange(float v)
		{
			_ambienceVolumeSlider.val = v;
			_ambienceVolumeText.val = Mathf.RoundToInt(v * 50).ToString();


			_mixer.SetFloat("_ambienceVolume", Mathf.Log10(v) * 40);


			_environmentVolume = v;

            SaveState.SafeSave();
        }

		/// <summary>
		/// GAMEPLAY SETTINGS
		/// </summary>



		[Inject] private Autosave _autoSave;
		[Inject] Access_AutosaveTimerDropdown _autosaveTimerDropdown;
		public void AutoSaveTimer(int v)
		{
			_autosaveTimerDropdown.val = v;
			_autosaveTime = v;

			if (_apply)
			{
				if (GameManager.Instance)
					GameManager.Instance.SaveManager.SetAutosaveTime(_autoSave.Intervals[_autosaveTime] * 60.0f);
			}

            SaveState.SafeSave();
        }

		[Inject] Access_DisplayNameDropdown _displayNameDropdown;
		public void DisplayUserNames(int v)
		{
			_displayNameDropdown.val = v;
			_displayName = v;

            SaveState.SafeSave();
        }

		[Inject] Access_DisplayBuildingDropdown _displayBuildingDropdown;
		public void DisplayBuildingData(int v)
		{
			_displayBuildingDropdown.val = v;
			_displayBuildings = v;

            SaveState.SafeSave();
        }

		[Inject] ChannelData _channelData;
		[Inject] Access_ChannelNameInput _channelNameInput;
		public void SetChannelName(string name)
		{
			_channelData.SetChannelName(name);
			_channelNameInput.text = name;

            SaveState.SafeSave();
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
		[Inject] ConnectionTab _connectionTab;
		public void TogglingConnectionTab(bool enabled)
		{
			_connectionTab.enabled = true;
		}

		/// <summary>
		/// Enable settings panel
		/// </summary>
		[Inject] SettingsPanel _settingsPanel;
		[Inject] ConfirmSettingsPanel _confirmSettingsPanel;
		public void ToggleSettingsPanel()
		{
			if (SaveState._savedData)
			{
				_settingsPanel.Enabled = !_settingsPanel.Enabled;
				if (_settingsPanel.Enabled == false)
				{
					ChangeTab(0);
				}
			}
			else
			{
				if (_settingsPanel.Enabled == true)
				{
					_confirmSettingsPanel.Enabled = true;
				}
				else
				{
					_settingsPanel.Enabled = true;
				}
			}
		}

		/// <summary>
		/// SET UP FUNCTIONS
		/// </summary>

		[Inject] Volume _postProcessVolume;
		private void SetUpPipelineAndPostProcessing()
		{
			_postProcessVolume = GetComponentInChildren<Volume>();
			_postProcessVolume.profile.TryGet(out _gammaAndBrightness);

			GraphicsSettings.defaultRenderPipeline = _renderPipeline;
			QualitySettings.renderPipeline = _renderPipeline;
		}


		private void SetUpResolution()
		{
			List<Resolution> res = new List<Resolution>();
			_resolutions = Screen.resolutions;

			_resolutionDropdown.dropDown.ClearOptions();

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

			_resolutionDropdown.dropDown.AddOptions(options);
			_resolutionDropdown.val = currentResolutionIndex;
			_resolution = currentResolutionIndex;
			_resolutionDropdown.dropDown.RefreshShownValue();
			_resolutions = res.ToArray();
		}
        public void ConfirmSettings()
        {
            SaveSettings();
            LoadSettings();
            ChangeTab(0);
            _confirmSettingsPanel.Enabled = false;
            _settingsPanel.Enabled = false;
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
            ResolutionOnChange(_resolution);
            DisplayModeOnChange(_displayMode);
            GammaOnChange(_gamma);
            BrightnessOnChange(_brightness);

            //Audio
            MasterVolumeOnChange(_masterVolume);
            MusicVolumeOnChange(_musicVolume);
            SoundEffectsVolumeOnChange(_playerVolume);
            AmbienceVolumeOnChange(_environmentVolume);

            //Gameplay
            //REVISIT
            //PanSensitivityOnChange(_panSensitivity);
            //ZoomSensitivityOnChange(_zoomSensitivity);
            //WASDSensitivityOnChange(_wasdSensitivity);
            //BorderDetectionSensitivityOnChange(_borderDetectionSensitivity);
            //FOVOnChange(_fov);
            DisplayBuildingData(_displayBuildings);
            DisplayUserNames(_displayName);
            AutoSaveTimer(_autosaveTime);
            //ToggleMouseControls(_mouseControls);
            //ToggleBorderDectionMovement(_borderDetection);


            //Twitch
            SetChannelName(_channelData.name);

            if (GameManager.Instance)
                GameManager.Instance.SaveManager.SetAutosaveTime(_autoSave.Intervals[_autosaveTime] * 60.0f);
            _apply = false;
        }

        /// <summary>
        /// Saving and loading
        /// </summary>

        public void SaveSettings()
        {
            SetUp_SavePreset();
            ApplySettings();
            GameIO.SaveSettingsData(_savePreset);

            UpdateSettingScriptableObject();

            SaveState._savedData = true;
        }

        public void CloseSettingPanel()
        {
            ChangeTab(0);
            LoadSettings();
            _confirmSettingsPanel.SetActive(false);
            _settingsPanel.SetActive(false);
        }


        public void LoadSettings()
        {
            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly/Stream Town/SettingsData.json"))
            {
                //_savePreset = GameIO.LoadSettingsData();
                SaveState._loadingData = true;
                _apply = true;
                //Visual

                ShadowsOnChange(_savePreset.shadowType);
                ShadowQualityOnChange(_savePreset.shadowResolution);
                AAOnChange(_savePreset.antiAliasing);

                //REVISIT
                //CameraAAOnChange(_savePreset.cameraAA);
                AOToggle(_savePreset.enabledAO);
                VSyncToggle(_savePreset.vSync);

                PresetOnChange(_savePreset.preset);


                FPSOnChange(_savePreset.fpsLimiter);
                ResolutionOnChange(_savePreset.resoultion);
                DisplayModeOnChange(_savePreset.displayMode);
                GammaOnChange(_savePreset.gamma);
                BrightnessOnChange(_savePreset.brightness);

                //Audio
                MasterVolumeOnChange(_savePreset.masterVolume);
                MusicVolumeOnChange(_savePreset.musicVolume);
                SoundEffectsVolumeOnChange(_savePreset.playerVolume);
                AmbienceVolumeOnChange(_savePreset.environmentVolume);

                //Gameplay
                //REVISIT
                //PanSensitivityOnChange(_savePreset.panSensitivity);
                //ZoomSensitivityOnChange(_savePreset.zoomSensitivity);
                //WASDSensitivityOnChange(_savePreset.wasdSensitivity);
                //BorderDetectionSensitivityOnChange(_savePreset.borderDetectionSensitivity);
                //FOVOnChange(_savePreset.fov);
                DisplayBuildingData(_savePreset.displayBuildings);
                DisplayUserNames(_savePreset.displayName);
                AutoSaveTimer(_savePreset.autosaveTime);
                //ToggleMouseControls(_savePreset.mouseMovement);
                //ToggleBorderDectionMovement(_savePreset.keyboardMovement);
                //Twitch
                SetChannelName(_savePreset.channelName);
                UpdateSettingScriptableObject();

                if (_cameraApplyChanges)
                    _cameraApplyChanges.UpdateCameraSettings();

                SaveState._loadingData = false;
                SaveState._savedData = true;
                _apply = false;

                if (GameManager.Instance)
                    GameManager.Instance.SaveManager.SetAutosaveTime(_autoSave.Intervals[_autosaveTime] * 60.0f);
            }
            else
            {
                Debug.Log("No setting file located");
                SetUpSettingsDataFile();
            }
        }

		private bool mouseControls = true; //REVISIT
		//private bool borderDetection = true; //REVISIT
		[Inject] Access_CameraAADropdown _cameraAADropdown;
        // Setting all the settings values to the UI values
        private void SetUpSettingsDataFile()
        {
            Debug.Log("Creating settings save file");

            _preset = 5;

            _savePreset.displayMode = _displayModeDropdown.val;

            _savePreset.shadowType = _shadowTypeDropdown.val;
            _savePreset.shadowResolution = _shadowQualityDropdown.val;

            _savePreset.enabledAO = _AOToggle.isOn;

            _savePreset.vSync = _vSyncToggle.isOn;
            _savePreset.fpsLimiter = _fpsLimiterDropdown.val;

            _savePreset.brightness = _brightnessSlider.val;
            _savePreset.gamma = _gammaSlider.val;

            _savePreset.antiAliasing = _AADropdown.val;
            _savePreset.cameraAA = _cameraAADropdown.val;

            _savePreset.masterVolume = _masterVolumeSlider.val;
            _savePreset.musicVolume = _musicVolumeSlider.val;
            _savePreset.playerVolume = _soundEffectsVolumeSlider.val;
            _savePreset.environmentVolume = _ambienceVolumeSlider.val;

            _savePreset.panSensitivity = _panningSensitivitySlider.val;
            _savePreset.zoomSensitivity = _zoomingSensitivitySlider.val;
            _savePreset.wasdSensitivity = _wasdSensitivitySlider.val;
            _savePreset.borderDetectionSensitivity = _borderDetectionSensitivitySlider.val;

            _savePreset.fov = (int)_fovLevelSlider.val;

            _savePreset.autosaveTime = _autosaveTimerDropdown.val;

            _savePreset.displayName = _displayNameDropdown.val;
            _savePreset.displayBuildings = _displayBuildingDropdown.val;

            //borderDetection = _borderDetectionToggle.isOn; //REVISIT - THERE IS NO TOGGLE??
            mouseControls = _mouseControlsToggle.isOn; //Private bool

            _channelData.name = _channelNameInput.text;

            SaveSettings();

            LoadSettings();
            Debug.Log("Created and loaded new settings save file");
        }

        private void UpdateSettingScriptableObject()
        {
            SettingsScriptable data = Resources.Load<Scriptables.SettingsScriptable>("ScriptableObjects/Settings/SettingsData");
            data.panSensitivity = _panSensitivity;
            data.zoomSensitivity = _zoomSensitivity;
            data.wasdSensitivity = _wasdSensitivity;
            data.borderDetectionSensitivity = _borderDetectionSensitivity;
            data.mouseControls = _mouseControls;
            data.borderDetection = _borderDetection;
            data.camAA = _cameraAA;
            data.fov = _fov;
            data.displayBuildings = _displayBuildings;
            data.displayName = _displayName;
            data.channelName = _channelData.name;
            data.autosaveTime = _autosaveTime;
        }

		//UI Functionality
		[Inject] SettingPages _settingPages;
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

        private void SetUp_SavePreset()
        {
            _savePreset = new SavePreset();

            _savePreset.preset = _preset;

            _savePreset.displayMode = _displayModeDropdown.val;
            _savePreset.resoultion = _resolutionDropdown.val;

            _savePreset.shadowType = _shadowTypeDropdown.val;
            _savePreset.shadowResolution = _shadowQualityDropdown.val;

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

            _savePreset.displayName = _displayNameDropdown.val;
            _savePreset.displayBuildings = _displayBuildingDropdown.val;

            _savePreset.keyboardMovement = _borderDetection;
            _savePreset.mouseMovement = _mouseControls;

            _savePreset.channelName = _channelData.name;
        }

        /// <summary>
        /// UNITY FUNCTIONS
        /// </summary>

        private void Start()
		{
			if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + GameIO.SAVE_FILEPATH))
			{
				Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + GameIO.SAVE_FILEPATH);
			}
			SetUpPipelineAndPostProcessing();
			SetUpResolution();

			if (!File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly" + "/Stream Town" + "/SettingsData.json"))
			{
				SetUpSettingsDataFile();
			}

			LoadSettings();

			DontDestroyOnLoad(gameObject);
		}
	}
}