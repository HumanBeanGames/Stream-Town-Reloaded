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
		/// <summary>
		/// The number of seconds in a day.
		/// </summary>
		private static int _secondsPerDay = 3600;
		[HideInInspector]
		private static int _dayCount = 0;
		[HideInInspector]
		private static float _worldTimePassed = 0;

		/// <summary>
		/// The current day count.
		/// </summary>
		public static int DayCount => _dayCount;
		/// <summary>
		/// How much time has passed since the world started.
		/// </summary>
		public static float WorldTimePassed { get { return _worldTimePassed; } set { _worldTimePassed = value; } }
		/// <summary>
		/// The number of seconds in a day.
		/// </summary>
		public static int SecondsPerDay => _secondsPerDay;

		/// <summary>
		/// Called when a day has passed.
		/// </summary>
		public static event Action DayPassed;

		/// <summary>
		/// Calculates how many days have passed.
		/// </summary>
		public static void CalculateDayCount()
		{
			int prevDayCount = _dayCount;
			_dayCount = Mathf.FloorToInt((int)_worldTimePassed / _secondsPerDay);

			if (prevDayCount < _dayCount)
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
                _worldTimePassed += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }
	}
}