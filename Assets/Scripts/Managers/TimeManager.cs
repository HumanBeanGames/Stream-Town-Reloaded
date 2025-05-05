using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
	/// <summary>
	/// Handles the time passage in game.
	/// </summary>
	[GameManager]
	public static class TimeManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static TimeConfig Config = TimeConfig.Instance;

		/// <summary>
		/// The number of seconds in a day.
		/// </summary>
		public static int SecondsPerDay => Config.secondsPerDay;
		[HideInInspector]
        public static int DayCount
        {
            get => Config?.dayCount ?? 0;
            set
            {
                if (Config != null)
                    Config.dayCount = value;
            }
        }
        [HideInInspector]
        public static float WorldTimePassed
        {
            get => Config?.worldTimePassed ?? 0;
            set
            {
                if (Config != null)
                    Config.worldTimePassed = value;
            }
        }

        /// <summary>
        /// Called when a day has passed.
        /// </summary>
        public static event Action DayPassed;

		/// <summary>
		/// Calculates how many days have passed.
		/// </summary>
		public static void CalculateDayCount()
		{
			int prevDayCount = DayCount;
			DayCount = Mathf.FloorToInt((int)WorldTimePassed / SecondsPerDay);

			if (prevDayCount < DayCount)
			{
				Debug.Log("Day Passed");
				DayPassed?.Invoke();
			}
		}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            GameObject runnerObject = new GameObject("TimeManagerRunner");
            Runner runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
			runner.StartCoroutine(UpdateWorldTimeCoroutine());
        }

        private static IEnumerator UpdateWorldTimeCoroutine()
        {
            while (true)
            {
                WorldTimePassed += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }
	}
}