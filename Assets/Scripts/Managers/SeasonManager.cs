using System;
using System.Collections;
using UnityEngine;
using Utils;
using Scriptables;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    [GameManager]
    public static class SeasonManager
    {
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static SeasonConfig Config = SeasonConfig.Instance;

        public static event Action<Season> OnSeasonChanged;
        public static event Action<Season> OnSeasonChanging;

        public static bool SeasonChanging
        {
            get => Config?.seasonChanging ?? false;
            set
            {
                if (Config != null)
                    Config.seasonChanging = value;
            }
        }

        public static int DaysPerSeason => Config?.daysPerSeason ?? 0;
        public static float SeasonTransitionTime => Config?.seasonTransitionTime ?? 0;
        public static Season StartingSeason => Config?.startingSeason ?? Season.Summer;

        public static Season CurrentSeason
        {
            get => Config?.currentSeason ?? Season.Summer;
            set
            {
                if (Config != null)
                    Config.currentSeason = value;
            }
        }

        public static AllSeasonsScriptable AllSeasonsData => Config?.allSeasonsData;
        public static Material GrassMaterial => Config?.grassMaterial;
        public static Material TerrainMaterial => Config?.terrainMaterial;
        public static Material TreeMaterial => Config?.treeMaterial;
        public static Material BuildingMaterial => Config?.buildingMaterial;
        public static Material WaterMaterial => Config?.waterMaterial;

#if UNITY_EDITOR
        public static bool DriveSeasonsByTime => Config?.driveSeasonsByTime ?? false;

        public static void CallNextSeason() => NextSeason(0);
        public static void UpdateCurrentSeason() => runner.StartCoroutine(TransitionSeason(CurrentSeason, 0));
#endif

        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

        private static void OnDayPassed()
        {
#if UNITY_EDITOR
            if (DriveSeasonsByTime)
#endif
                if (TimeManager.DayCount % DaysPerSeason == 0)
                    NextSeason();
        }

        private static void NextSeason(float transitionTime = -1)
        {
            Debug.Log("Next Season Called");
            Season nextSeason = CurrentSeason + 1;
            if (nextSeason == Season.Count) nextSeason = 0;

            if (!SeasonChanging)
                runner.StartCoroutine(TransitionSeason(nextSeason, transitionTime == -1 ? SeasonTransitionTime : transitionTime));
        }

        private static IEnumerator TransitionSeason(Season nextSeason, float transitionTime, bool triggerEvent = true)
        {
            yield return new WaitUntil(()=>GameManager.Instance != null);
            GameManager.Instance.WeatherManager.StopWeather();
            yield return new WaitForEndOfFrame();

            OnSeasonChanging?.Invoke(nextSeason);
            SeasonChanging = true;

            var currentSeasonData = AllSeasonsData?.GetSeasonData(CurrentSeason);
            var nextSeasonData = AllSeasonsData?.GetSeasonData(nextSeason);

            float transition = 0;
            GameManager.Instance.WeatherManager.StartWeather(nextSeason);
            while (transition < 1)
            {
                transition += (transitionTime == 0) ? 1 : Time.deltaTime / transitionTime;
                transition = Mathf.Min(transition, 1);

                if (GrassMaterial)
                {
                    GrassMaterial.SetColor("_GridColor1", Color.Lerp(currentSeasonData.GrassGridColor1, nextSeasonData.GrassGridColor1, transition));
                    GrassMaterial.SetColor("_GridColor2", Color.Lerp(currentSeasonData.GrassGridColor2, nextSeasonData.GrassGridColor2, transition));
                    GrassMaterial.SetColor("_TopColor", Color.Lerp(currentSeasonData.GrassTopColor, nextSeasonData.GrassTopColor, transition));
                    GrassMaterial.SetColor("_WindColor", Color.Lerp(currentSeasonData.GrassWindColor, nextSeasonData.GrassWindColor, transition));
                }

                if (TerrainMaterial)
                {
                    TerrainMaterial.SetColor("_color1", Color.Lerp(currentSeasonData.TerrainColor1, nextSeasonData.TerrainColor1, transition));
                    TerrainMaterial.SetColor("_color2", Color.Lerp(currentSeasonData.TerrainColor2, nextSeasonData.TerrainColor2, transition));
                }

                SetSeasonMaterial(nextSeason, transition);
                yield return new WaitForEndOfFrame();
            }

            CurrentSeason = nextSeason;
            OnSeasonChanged?.Invoke(nextSeason);
            SeasonChanging = false;
        }

        public static void ForceSetNextSeason()
        {
            Season nextSeason = CurrentSeason + 1;
            if (nextSeason == Season.Count) nextSeason = 0;

            var currentSeasonData = AllSeasonsData?.GetSeasonData(CurrentSeason);
            var nextSeasonData = AllSeasonsData?.GetSeasonData(nextSeason);

            if (GrassMaterial)
            {
                GrassMaterial.SetColor("_GridColor1", nextSeasonData.GrassGridColor1);
                GrassMaterial.SetColor("_GridColor2", nextSeasonData.GrassGridColor2);
                GrassMaterial.SetColor("_TopColor", nextSeasonData.GrassTopColor);
                GrassMaterial.SetColor("_WindColor", nextSeasonData.GrassWindColor);
            }

            if (TerrainMaterial)
            {
                TerrainMaterial.SetColor("_color1", nextSeasonData.TerrainColor1);
                TerrainMaterial.SetColor("_color2", nextSeasonData.TerrainColor2);
            }

            SetSeasonMaterial(nextSeason, 1);
            CurrentSeason = nextSeason;
        }

        public static void SetSeason(Season selectedSeason)
        {
            var data = AllSeasonsData?.GetSeasonData(selectedSeason);
            if (GrassMaterial)
            {
                GrassMaterial.SetColor("_GridColor1", data.GrassGridColor1);
                GrassMaterial.SetColor("_GridColor2", data.GrassGridColor2);
                GrassMaterial.SetColor("_TopColor", data.GrassTopColor);
                GrassMaterial.SetColor("_WindColor", data.GrassWindColor);
            }
            if (TerrainMaterial)
            {
                TerrainMaterial.SetColor("_color1", data.TerrainColor1);
                TerrainMaterial.SetColor("_color2", data.TerrainColor2);
            }
        }

        private static void SetSeasonMaterial(Season season, float transition)
        {
            float winterTint = Config?.winterTint ?? 0.42f;
            float restTint = Config?.restTint ?? -0.08f;

            if (season == Season.Autumn)
            {
                TreeMaterial?.SetFloat("_AutumnPower", transition * 0.3f);
                TreeMaterial?.SetFloat("_SnowPower", 0);
                BuildingMaterial?.SetFloat("_SnowPower", 0);
                BuildingMaterial?.SetFloat("_SnowNoiseLevels", 0);
            }
            else if (season == Season.Winter)
            {
                TreeMaterial?.SetFloat("_AutumnPower", (1 - transition) * 0.5f);
                TreeMaterial?.SetFloat("_SnowPower", transition * 0.5f);
                BuildingMaterial?.SetFloat("_SnowPower", transition);
                BuildingMaterial?.SetFloat("_SnowNoiseLevels", transition);
                WaterMaterial?.SetFloat("_IceStrength", transition);
                TerrainMaterial?.SetFloat("_Tint", transition * winterTint);
                GrassMaterial?.SetFloat("_Tint", transition * winterTint);
            }
            else if (season == Season.Spring)
            {
                TreeMaterial?.SetFloat("_SnowPower", (1 - transition) * 0.5f);
                BuildingMaterial?.SetFloat("_SnowPower", (1 - transition) * 0.5f);
                TreeMaterial?.SetFloat("_Spring", transition * 0.1f);
                TreeMaterial?.SetFloat("_AutumnPower", 0);
                BuildingMaterial?.SetFloat("_SnowNoiseLevels", (1 - transition));
                WaterMaterial?.SetFloat("_IceStrength", 1 - transition);
                TerrainMaterial?.SetFloat("_Tint", (1 - transition) * restTint);
                GrassMaterial?.SetFloat("_Tint", (1 - transition) * restTint);
            }
            else
            {
                TreeMaterial?.SetFloat("_Spring", (1 - transition) * 0.1f);
                TreeMaterial?.SetFloat("_SnowPower", 0);
                TreeMaterial?.SetFloat("_AutumnPower", 0);
                BuildingMaterial?.SetFloat("_SnowPower", 0);
                BuildingMaterial?.SetFloat("_SnowNoiseLevels", 0);
            }
        }

        public static void SetSeasonByTimePassed()
        {
            CurrentSeason = (Season)(TimeManager.DayCount % DaysPerSeason);
            SetSeasonMaterial(CurrentSeason, 1.0f);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            Debug.Log(Config);
            GameObject runnerObject = new GameObject("SeasonManagerRunner");
            runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);

            CurrentSeason = StartingSeason;
            runner.StartCoroutine(TransitionSeason(CurrentSeason, 0));
            TimeManager.DayPassed += OnDayPassed;
        }
    }
}
