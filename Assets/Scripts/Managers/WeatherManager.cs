using Scriptables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Utils;
using VFX;

namespace Managers
{
	[GameManager]
	public static class WeatherManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static WeatherConfig Config = WeatherConfig.Instance;

		[HideInInspector]
        private static VisualEffect _currentVFX;

		[HideInInspector]
		private static VisualEffect _autumnVFX;
		[HideInInspector]
		private static VisualEffect _winterVFX;
		[HideInInspector]
		private static VisualEffect _summerVFX;
		[HideInInspector]
		private static VisualEffect _springVFX;

		private static VisualEffect GetVFX(ref VisualEffect vfx, GameObject prefab)
		{
			if (vfx == null && prefab != null)
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					vfx = prefab.GetComponent<VisualEffect>();
				}
				else
#endif
				{
					GameObject parent = GameObject.FindWithTag("WeatherVFX");
					if (parent != null)
					{
						GameObject instance = GameObject.Instantiate(prefab, parent.transform);
						vfx = instance.GetComponent<VisualEffect>();
						VfxParticlePosition vfxParticlePosition = instance.GetComponent<VfxParticlePosition>();
						if (vfxParticlePosition != null)
						{
							vfxParticlePosition.transform = Camera.main != null ? Camera.main.transform : GameObject.FindWithTag("MainCamera").transform;
						}
					}
				}
			}
			return vfx;
		}

		private static VisualEffect AutumnVFX => GetVFX(ref _autumnVFX, Config.autumnVFXPrefab);
		private static VisualEffect WinterVFX => GetVFX(ref _winterVFX, Config.winterVFXPrefab);
		private static VisualEffect SummerVFX => GetVFX(ref _summerVFX, Config.summerVFXPrefab);
		private static VisualEffect SpringVFX => GetVFX(ref _springVFX, Config.springVFXPrefab);

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
            SetDataVFX();
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
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Summer).VFX = SummerVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Autumn).VFX = AutumnVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Winter).VFX = WinterVFX;
			SeasonManager.AllSeasonsData.GetSeasonData(Utils.Season.Spring).VFX = SpringVFX;
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
		}
	}
}