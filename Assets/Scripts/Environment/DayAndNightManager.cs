using UnityEngine;
using System.Collections;
using Utils;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Managers;
using System;

namespace Environment
{
	public class DayAndNightManager : MonoBehaviour
	{
		private static float _DAY_PERCENTAGE = 0.666f;
		[Header("Day and Night Properties")]

		[SerializeField]
		private Light _mainLightSource;

		[SerializeField]
		private float _dayLength;
		[SerializeField]
		private float _nightLength;

		[SerializeField]
		private float _transitionLength;

		[SerializeField]
		private float _nightLightIntensity;
		[SerializeField]
		private float _dayLightIntensity;

		[Header("Material Properties"), Space(5)]

		[SerializeField]
		private Material _buildingMaterial;
		[SerializeField]
		private float _maxEmissionStrength;

		[Header("Post processing"), Space(5)]

		[SerializeField]
		private Volume _postProcessVolume;

		//Day and night private properties
		private float _transitionTime = 0.0f;
		private float _timeTillTransition = 0.0f;
		public bool IsDayTime { get; private set; } = true;

		//Material private properties
		private float _emissionStrength = 0.0f;

		public Action OnDayStarted;
		public Action OnNightStarted;
		public Action OnDayStarting;
		public Action OnNightStarting;

		IEnumerator ChangeLight()
		{
			if (IsDayTime)
			{
				OnNightStarting?.Invoke();
				//Transition to night
				while (_transitionTime / _transitionLength < 1)
				{
					_transitionTime += Time.deltaTime;
					//Changing the light source
					_mainLightSource.intensity = Mathf.Lerp(_dayLightIntensity, _nightLightIntensity, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
					_mainLightSource.transform.parent.eulerAngles = new Vector3(0, Mathf.Lerp(0, -120, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f))), 0);
					//Changing the building material
					_emissionStrength = Mathf.Lerp(0, _maxEmissionStrength, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
					_buildingMaterial.SetFloat("_EmissionStrength", _emissionStrength);
					//Changing the post processing
					_postProcessVolume.weight = Mathf.Lerp(0.0f, 1.0f, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
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
				while (_transitionTime / _transitionLength < 1)
				{
					_transitionTime += Time.deltaTime;
					//Changing the light source
					_mainLightSource.intensity = Mathf.Lerp(_nightLightIntensity, _dayLightIntensity, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
					_mainLightSource.transform.parent.eulerAngles = new Vector3(0, Mathf.Lerp(-120, 0, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f))), 0);
					//Changing the building material
					_emissionStrength = Mathf.Lerp(_maxEmissionStrength, 0, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
					_buildingMaterial.SetFloat("_EmissionStrength", _emissionStrength);
					//Changing the post processing
					_postProcessVolume.weight = Mathf.Lerp(1.0f, 0.0f, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _transitionLength, 0.0f, 1.0f)));
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

		private void OnTimeChanged()
		{
			if (IsDayTime)
				OnDayStarted?.Invoke();
			else
				OnNightStarted?.Invoke();
		}

		private void ChangeTime()
		{
			_timeTillTransition += Time.deltaTime;

			if (IsDayTime)
			{
				if (_timeTillTransition > _dayLength)
				{
					StartCoroutine(ChangeLight());
				}
			}
			else
			{
				if (_timeTillTransition > _nightLength)
				{
					StartCoroutine(ChangeLight());
				}
			}
		}

		private void Awake()
		{
			_buildingMaterial.SetFloat("_EmissionStrength", 0);
			_postProcessVolume.weight = 0;
		}

		private void Start()
		{
			_dayLength = TimeManager.SecondsPerDay * _DAY_PERCENTAGE - _transitionLength;
			_nightLength = TimeManager.SecondsPerDay * (1-_DAY_PERCENTAGE) - _transitionLength;
		}

		private void Update()
		{
			ChangeTime();
		}
	}
}