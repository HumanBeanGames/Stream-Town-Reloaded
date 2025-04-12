using System;
using UnityEngine;

/// <summary>
/// Handles the time passage in game.
/// </summary>
public class TimeManager : MonoBehaviour
{
	[SerializeField]
	private int _secondsPerDay = 3600;
	private int _dayCount = 0;
	private float _worldTimePassed = 0;

	public int DayCount => _dayCount;
	/// <summary>
	/// How much time has passed since the world started.
	/// </summary>
	public float WorldTimePassed { get { return _worldTimePassed; } set { _worldTimePassed = value; } }
	public int SecondsPerDay => _secondsPerDay;

	/// <summary>
	/// Called when a day has passed.
	/// </summary>
	public event Action DayPassed;

	/// <summary>
	/// Calculates how many days have passed.
	/// </summary>
	public void CalculateDayCount()
	{
		int prevDayCount = _dayCount;
		_dayCount = Mathf.FloorToInt((int)_worldTimePassed / _secondsPerDay);

		if (prevDayCount < _dayCount)
		{
			Debug.Log("Day Passed");
			DayPassed?.Invoke();
		}
	}

	private void Update()
	{
		_worldTimePassed += Time.deltaTime;
		CalculateDayCount();
	}
}