using Scriptables;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Managers
{
	public class WeatherManager : MonoBehaviour
	{
		private SeasonManager _seasonManager;
		private VisualEffect _currentVFX;

		[SerializeField]
		private VisualEffect _autumnVFX;
		[SerializeField]
		private VisualEffect _winterVFX;
		[SerializeField]
		private VisualEffect _summerVFX;
		[SerializeField]
		private VisualEffect _springVFX;
		public void StartWeather(Season season)
		{
			StartCoroutine(RunWeather(_seasonManager.AllSeasonsData.GetSeasonData(season)));
		}

		public void StopWeather()
		{
			StopCoroutine("RunWeather");

			if (_currentVFX != null)
				_currentVFX.Stop();
		}

		private IEnumerator RunWeather(SeasonScriptable data)
		{
			_currentVFX = data.VFX;
			_currentVFX.Play();
			float runTime = Random.Range(data.MinRunTime, data.MaxRunTime);
			if (_currentVFX != null)
				_currentVFX.SetFloat("AmountOfParticles", 0);
			float lerpValue = 0;
			while (runTime > 0)
			{
				if (lerpValue < 1)
					lerpValue += Time.deltaTime / data.ParticleLerpTime;
				if (lerpValue > 1)
					lerpValue = 1;

				if (_currentVFX != null)
					_currentVFX.SetFloat("AmountOfParticles", Mathf.Lerp(0, data.MaxParticleCount, lerpValue));
				runTime -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			data.VFX.Stop();
			_currentVFX = null;
		}

		private void SetDataVFX()
		{
			_seasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Summer).VFX = _summerVFX;
			_seasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Autumn).VFX = _autumnVFX;
			_seasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Winter).VFX = _winterVFX;
			_seasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Spring).VFX = _springVFX;
		}

		private void Start()
		{
			_seasonManager = GameManager.Instance.SeasonManager;
			SetDataVFX();
		}
	}
}