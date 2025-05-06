using UnityEngine;
using System.Collections;
using Utils;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Managers;
using System;
using Sirenix.OdinInspector;

namespace Environment
{
	[GameManager]
	public static class DayAndNightManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static DayAndNightConfig Config = DayAndNightConfig.Instance;

		private static float DayPercentage => Config.dayPercentage;

		[HideInInspector]
		private static Light _mainLightSource;

		private static Light MainLightSource
		{
			get
			{
				if (_mainLightSource == null)
				{
					GameObject mainLightObject = GameObject.FindGameObjectWithTag("MainLight");
					if (mainLightObject != null)
					{
						_mainLightSource = mainLightObject.GetComponent<Light>();
					}
				}
				return _mainLightSource;
			}
		}

		[HideInInspector]
		private static float _dayLength;
        [HideInInspector]
        private static float _nightLength;

		public static float TransitionLength => Config?.transitionLength ?? 0;
		public static float NightLightIntensity => Config?.nightLightIntensity ?? 0;
		public static float DayLightIntensity => Config?.dayLightIntensity ?? 0;
		public static Material BuildingMaterial => Config?.buildingMaterial;
		public static float MaxEmissionStrength => Config?.maxEmissionStrength ?? 0;
		
		[HideInInspector]
		private static Volume _postProcessVolume;
		public static Volume PostProcessVolume
		{
			get => GetVolume(ref _postProcessVolume, Config.dayPP);
		}

		//Day and night private properties
		[HideInInspector]
		private static float _transitionTime = 0.0f;
		[HideInInspector]
		private static float _timeTillTransition = 0.0f;
		public static bool IsDayTime { get; private set; } = true;

		//Material private properties
		[HideInInspector]
		private static float _emissionStrength = 0.0f;
		[HideInInspector]
		public static Action OnDayStarted;
		[HideInInspector]
		public static Action OnNightStarted;
		[HideInInspector]
		public static Action OnDayStarting;
		[HideInInspector]
		public static Action OnNightStarting;

		private static Volume GetVolume(ref Volume volume, GameObject prefab)
		{
			if (volume == null && prefab != null)
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					volume = prefab.GetComponent<Volume>();
				}
				else
#endif
				{
					GameObject parent = GameObject.FindWithTag("PostProcessing");
					if (parent != null)
					{
						GameObject instance = GameObject.Instantiate(prefab, parent.transform);
						volume = instance.GetComponent<Volume>();
						volume.weight = 0;
					}
				}
			}
			return volume;
		}

		public static IEnumerator ChangeLight()
		{
			if (IsDayTime)
			{
				OnNightStarting?.Invoke();
				//Transition to night
				while (_transitionTime / TransitionLength < 1)
				{
					_transitionTime += Time.deltaTime;
					//Changing the light source
					MainLightSource.intensity = Mathf.Lerp(DayLightIntensity, NightLightIntensity, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					MainLightSource.transform.parent.eulerAngles = new Vector3(0, Mathf.Lerp(0, -120, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f))), 0);
					//Changing the building material
					_emissionStrength = Mathf.Lerp(0, MaxEmissionStrength, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					BuildingMaterial.SetFloat("_EmissionStrength", _emissionStrength);
					//Changing the post processing
					PostProcessVolume.weight = Mathf.Lerp(0.0f, 1.0f, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					//reset the time till next transition
					_timeTillTransition = 0;

					yield return null;
				}

				IsDayTime = !IsDayTime;
				OnTimeChanged();
				_transitionTime = 0;
			}
			else
			{
				OnDayStarting?.Invoke();
				//Transition to day
				while (_transitionTime / TransitionLength < 1)
				{
					_transitionTime += Time.deltaTime;
					//Changing the light source
					MainLightSource.intensity = Mathf.Lerp(NightLightIntensity, DayLightIntensity, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					MainLightSource.transform.parent.eulerAngles = new Vector3(0, Mathf.Lerp(-120, 0, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f))), 0);
					//Changing the building material
					_emissionStrength = Mathf.Lerp(MaxEmissionStrength, 0, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					BuildingMaterial.SetFloat("_EmissionStrength", _emissionStrength);
					//Changing the post processing
					PostProcessVolume.weight = Mathf.Lerp(1.0f, 0.0f, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / TransitionLength, 0.0f, 1.0f)));
					//reset the time till next transition
					_timeTillTransition = 0;

					yield return null;
				}

				IsDayTime = !IsDayTime;
				OnTimeChanged();
				if(IsDayTime)
				_transitionTime = 0;
			}
		}

		private static void OnTimeChanged()
		{
			if (IsDayTime)
				OnDayStarted?.Invoke();
			else
				OnNightStarted?.Invoke();
		}

		private static void ChangeTime()
		{
			_timeTillTransition += Time.deltaTime;

			if (IsDayTime)
			{
				if (_timeTillTransition > _dayLength)
				{
					TimeManager.StartCoroutine(ChangeLight());
				}
			}
			else
			{
				if (_timeTillTransition > _nightLength)
				{
					TimeManager.StartCoroutine(ChangeLight());
				}
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			BuildingMaterial.SetFloat("_EmissionStrength", 0);
			_dayLength = TimeManager.SecondsPerDay * DayPercentage - TransitionLength;
			_nightLength = TimeManager.SecondsPerDay * (1-DayPercentage) - TransitionLength;
			TimeManager.StartCoroutine(Update());
		}

		private static IEnumerator Update()
		{
			while (true)
			{
				ChangeTime();
				yield return new WaitForEndOfFrame();
			}
		}
	}
}