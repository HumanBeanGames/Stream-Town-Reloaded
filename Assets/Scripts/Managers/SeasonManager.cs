using System;
using System.Collections;
using UnityEngine;
using Utils;
using Scriptables;
using Managers;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
	[GameManager]
	public static class SeasonManager
	{
		private static readonly float _winterTint = 0.42f;
		private static readonly float _restTint = -0.08f;

		[SerializeField]
		private static Season _startingSeason = Season.Summer;
		[SerializeField]
		private static int _daysPerSeason = 3;
		[SerializeField]
		private static float _seasonTransitionTime = 10;
		[SerializeField]
		private static AllSeasonsScriptable _allSeasonsData;
		[SerializeField]
		private static Material _grassMaterial;
		[SerializeField]
		private static Material _terrainMaterial;
		[SerializeField]
		private static Material _treeMaterial;
		[SerializeField]
		private static Material _buildingMaterial;
		[SerializeField]
		private static Material _waterMaterial;

		private static Season _currentSeason;
		private static bool _seasonChanging = false;

		public static event Action<Season> OnSeasonChanged;
		public static event Action<Season> OnSeasonChanging;

		public static bool SeasonChanging => _seasonChanging;
		public static int DaysPerSeason => _daysPerSeason;
		public static Season CurrentSeason => _currentSeason;

		public static AllSeasonsScriptable AllSeasonsData => _allSeasonsData;

        private class Runner : MonoBehaviour { }
		[HideInInspector]
		private static Runner runner;

#if UNITY_EDITOR
        [Header("EDITOR SETTINGS")]
		[Tooltip("If set to true, seasons will be driven by the game's time manager.")]
		public static bool DriveSeasonsByTime = false;
		public static void CallNextSeason()
		{
			NextSeason(0);
		}

		public static void UpdateCurrentSeason()
		{
			runner.StartCoroutine(TransitionSeason(_currentSeason, 0));
		}
#endif

		/// <summary>
		/// Called when a day has passed.
		/// </summary>
		private static void OnDayPassed()
		{
#if UNITY_EDITOR
			if (DriveSeasonsByTime)
#endif
				if ((TimeManager.DayCount) % _daysPerSeason == 0)
					NextSeason();
		}

		/// <summary>
		/// Starts transition to next season.
		/// </summary>
		private static void NextSeason(float _transitionTime = -1)
		{
			Debug.Log("Next Season Called");
			Season nextSeason = _currentSeason + 1;

			if (nextSeason == Season.Count)
				nextSeason = 0;

			if (!_seasonChanging)
				runner.StartCoroutine(TransitionSeason(nextSeason, _transitionTime == -1 ? _seasonTransitionTime : _transitionTime));
		}

		/// <summary>
		/// Transitions from current season to the next season.
		/// </summary>
		/// <param name="nextSeason"></param>
		/// <param name="transitionTime"></param>
		/// <param name="triggerEvent"></param>
		/// <returns></returns>
		private static IEnumerator TransitionSeason(Season nextSeason, float transitionTime, bool triggerEvent = true)
		{
			GameManager.Instance.WeatherManager.StopWeather();
			yield return new WaitForEndOfFrame();
			OnSeasonChanging?.Invoke(nextSeason);
			_seasonChanging = true;

			SeasonScriptable currentSeasonData = _allSeasonsData.GetSeasonData(_currentSeason);
			SeasonScriptable nextSeasonData = _allSeasonsData.GetSeasonData(nextSeason);

			float transition = 0;
			GameManager.Instance.WeatherManager.StartWeather(nextSeason);
			while (transition < 1)
			{
				if (transitionTime == 0)
					transition = 1;
				else
				{
					transition += Time.deltaTime / transitionTime;
					if (transition > 1)
						transition = 1;
				}
				// Lerp all season colors and set the materials.

				// Grass Values.
				if (_grassMaterial)
				{
					_grassMaterial.SetColor("_GridColor1", Color.Lerp(currentSeasonData.GrassGridColor1, nextSeasonData.GrassGridColor1, transition));
					_grassMaterial.SetColor("_GridColor2", Color.Lerp(currentSeasonData.GrassGridColor2, nextSeasonData.GrassGridColor2, transition));
					_grassMaterial.SetColor("_TopColor", Color.Lerp(currentSeasonData.GrassTopColor, nextSeasonData.GrassTopColor, transition));
					_grassMaterial.SetColor("_WindColor", Color.Lerp(currentSeasonData.GrassWindColor, nextSeasonData.GrassWindColor, transition));
				}

				// Terrain Values.
				if (_terrainMaterial)
				{
					_terrainMaterial.SetColor("_color1", Color.Lerp(currentSeasonData.TerrainColor1, nextSeasonData.TerrainColor1, transition));
					_terrainMaterial.SetColor("_color2", Color.Lerp(currentSeasonData.TerrainColor2, nextSeasonData.TerrainColor2, transition));
				}

				// Tree Values.
				// Uhh yeah this is gonna need some thinkin' aye.
				// Possibly need to store 2 gradients in at the same time, Current/Next, use transition to lerp in shader. pog.
				SetSeasonMaterial(nextSeason, transition);

				yield return new WaitForEndOfFrame();
			}

			_currentSeason = nextSeason;
			OnSeasonChanged?.Invoke(nextSeason);
			_seasonChanging = false;
		}

		public static void ForceSetNextSeason()
		{

			Season nextSeason = _currentSeason + 1;

			if (nextSeason == Season.Count)
				nextSeason = 0;
			SeasonScriptable currentSeasonData = _allSeasonsData.GetSeasonData(_currentSeason);
			SeasonScriptable nextSeasonData = _allSeasonsData.GetSeasonData(nextSeason);
			// Grass Values.
			if (_grassMaterial)
			{
				_grassMaterial.SetColor("_GridColor1", Color.Lerp(currentSeasonData.GrassGridColor1, nextSeasonData.GrassGridColor1, 1));
				_grassMaterial.SetColor("_GridColor2", Color.Lerp(currentSeasonData.GrassGridColor2, nextSeasonData.GrassGridColor2, 1));
				_grassMaterial.SetColor("_TopColor", Color.Lerp(currentSeasonData.GrassTopColor, nextSeasonData.GrassTopColor, 1));
				_grassMaterial.SetColor("_WindColor", Color.Lerp(currentSeasonData.GrassWindColor, nextSeasonData.GrassWindColor, 1));
			}

			// Terrain Values.
			if (_terrainMaterial)
			{
				_terrainMaterial.SetColor("_color1", Color.Lerp(currentSeasonData.TerrainColor1, nextSeasonData.TerrainColor1, 1));
				_terrainMaterial.SetColor("_color2", Color.Lerp(currentSeasonData.TerrainColor2, nextSeasonData.TerrainColor2, 1));
			}

			// Tree Values.
			// Uhh yeah this is gonna need some thinkin' aye.
			// Possibly need to store 2 gradients in at the same time, Current/Next, use transition to lerp in shader. pog.
			SetSeasonMaterial(nextSeason, 1);
			_currentSeason = nextSeason;
		}

		public static void SetSeason(Season selectedSeason)
		{
			SeasonScriptable selectedSeasonData = _allSeasonsData.GetSeasonData(selectedSeason);
			// Grass Values.
			if (_grassMaterial)
			{
				_grassMaterial.SetColor("_GridColor1", selectedSeasonData.GrassGridColor1);
				_grassMaterial.SetColor("_GridColor2", selectedSeasonData.GrassGridColor2);
				_grassMaterial.SetColor("_TopColor", selectedSeasonData.GrassTopColor);
				_grassMaterial.SetColor("_WindColor", selectedSeasonData.GrassWindColor);
			}

			// Terrain Values.
			if (_terrainMaterial)
			{
				_terrainMaterial.SetColor("_color1", selectedSeasonData.TerrainColor1);
				_terrainMaterial.SetColor("_color2", selectedSeasonData.TerrainColor2);
			}

			// Tree Values.
			// Uhh yeah this is gonna need some thinkin' aye.
			// Possibly need to store 2 gradients in at the same time, Current/Next, use transition to lerp in shader. pog.
			// Note: Much Pog
		}

		private static void SetSeasonMaterial(Season season, float transition)
		{
			if (season == Season.Autumn)
			{
				_treeMaterial.SetFloat("_AutumnPower", transition * 0.3f);
				_treeMaterial.SetFloat("_SnowPower", 0);
				_buildingMaterial.SetFloat("_SnowPower", 0);
				_buildingMaterial.SetFloat("_SnowNoiseLevels", 0);
			}
			else if (season == Season.Winter)
			{
				_treeMaterial.SetFloat("_AutumnPower", (1 - transition) * 0.5f);
				_treeMaterial.SetFloat("_SnowPower", transition * 0.5f);
				_buildingMaterial.SetFloat("_SnowPower", transition * 1f);
				_buildingMaterial.SetFloat("_SnowNoiseLevels", transition);
				_waterMaterial.SetFloat("_IceStrength", transition);
				_terrainMaterial.SetFloat("_Tint", transition * _winterTint);
				_grassMaterial.SetFloat("_Tint", transition * _winterTint);
			}
			else if (season == Season.Spring)
			{
				_treeMaterial.SetFloat("_SnowPower", (1 - transition) * 0.5f);
				_buildingMaterial.SetFloat("_SnowPower", (1 - transition) * 0.5f);
				_treeMaterial.SetFloat("_Spring", transition * 0.1f);
				_treeMaterial.SetFloat("_AutumnPower", 0);
				_buildingMaterial.SetFloat("_SnowNoiseLevels", (1 - transition));
				_waterMaterial.SetFloat("_IceStrength", 1 - transition);
				_terrainMaterial.SetFloat("_Tint", (1 - transition) * _restTint);
				_grassMaterial.SetFloat("_Tint", (1 - transition) * _restTint);
			}
			else
			{
				_treeMaterial.SetFloat("_Spring", (1 - transition) * 0.1f);
				_treeMaterial.SetFloat("_SnowPower", 0);
				_treeMaterial.SetFloat("_AutumnPower", 0);
				_buildingMaterial.SetFloat("_SnowPower", 0);
				_buildingMaterial.SetFloat("_SnowNoiseLevels", 0);
			}
		}

		public static void SetSeasonByTimePassed()
		{
			_currentSeason = (Season)(TimeManager.DayCount % _daysPerSeason);
			SetSeasonMaterial(_currentSeason, 1.0f);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InitializeRunner()
		{
			GameObject runnerObject = new GameObject("SeasonManagerRunner");
			Runner runner = runnerObject.AddComponent<Runner>();
			UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            _currentSeason = _startingSeason;
            runner.StartCoroutine(TransitionSeason(_currentSeason, 0));
            TimeManager.DayPassed += OnDayPassed;
        }
	}
}
/*#if UNITY_EDITOR
[CustomEditor(typeof(SeasonManager))]
public class SeasonManagerEditor : Editor
{
	private SeasonManager _t;

	private void OnEnable()
	{
		_t = (SeasonManager)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Next Season"))
			{
				_t.ForceSetNextSeason();
			}
			if (GUILayout.Button("Update Current"))
			{
				_t.UpdateCurrentSeason();
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();

			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		GUILayout.Label($"Current Season: {_t.CurrentSeason}");
	}
}
#endif*/