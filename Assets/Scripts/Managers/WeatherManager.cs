using Scriptables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Managers
{
	[GameManager]
	public static class WeatherManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static WeatherConfig Config = WeatherConfig.Instance;

		[HideInInspector]
        private static VisualEffect _currentVFX;

		private static VisualEffect _autumnVFX => Config.autumnVFX;
		private static VisualEffect _winterVFX => Config.winterVFX;
		private static VisualEffect _summerVFX => Config.summerVFX;
		private static VisualEffect _springVFX => Config.springVFX;

		public static void StartWeather(Season season)
		{
			runner.StartCoroutine(RunWeather(SeasonManager.AllSeasonsData.GetSeasonData(season)));
		}

		public static void StopWeather()
		{
			runner.StopAllCoroutines();

			if (_currentVFX != null)
				_currentVFX.Stop();
		}

		private static IEnumerator RunWeather(SeasonScriptable data)
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

		private static void SetDataVFX()
		{
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Summer).VFX = _summerVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Autumn).VFX = _autumnVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Winter).VFX = _winterVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Spring).VFX = _springVFX;
		}


        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InitializeRunner()
		{
			GameObject runnerObject = new GameObject("WeatherManagerRunner");
			runner = runnerObject.AddComponent<Runner>();
			UnityEngine.Object.DontDestroyOnLoad(runnerObject);
			SetDataVFX();
		}


	}
}