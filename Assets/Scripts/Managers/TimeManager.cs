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
		/// The number of seconds in a day  
		/// </summary>
		public static int SecondsPerDay => Config.secondsPerDay;
        [HideInInspector]
        public static int dayCount = 0;

        [HideInInspector]
        public static float worldTimePassed = 0;

        /// <summary>
        /// Called when a day has passed.
        /// </summary>
        public static event Action DayPassed;

		/// <summary>
		/// Calculates how many days have passed.
		/// </summary>
		public static void CalculateDayCount()
		{
			int prevDayCount = dayCount;
			dayCount = Mathf.FloorToInt((int)worldTimePassed / SecondsPerDay);

			if (prevDayCount < dayCount)
			{
				Debug.Log("Day Passed");
				DayPassed?.Invoke();
			}
		}

        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

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
                worldTimePassed += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }
	}
}