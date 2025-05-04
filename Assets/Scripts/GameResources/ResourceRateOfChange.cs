using DataStructures;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace GameResources
{
	/// <summary>
	/// Used for calculating the rate of change of a resource.
	/// </summary>
	public class ResourceRateOfChange
	{
		private Queue<ChangeTimeStamp> _timestampData = new Queue<ChangeTimeStamp>();
		private int _averageOverTime;
		private float _timePeriod;
		private float _updateRate;
		private float _updateTimer;
		private Utils.Resource _resourceType;
		private int _changeDuringPeriod;

		public int AverageOverTime => _averageOverTime;

		// Constructor.
		public ResourceRateOfChange(Utils.Resource resourceType, float timePeriod, float updateRate, TownResourceManager resourceManager)
		{
			_timePeriod = timePeriod;
			_updateRate = updateRate;
			_resourceType = resourceType;
			// Subscribe to resource change event
			resourceManager.OnAnyResourceChangeEvent.AddListener(OnResourceChange);
		}

		/// <summary>
		/// Processes a queue and calculates rate of change for town resources.
		/// </summary>
		public void ProcessQueue()
		{
			_updateTimer += Time.deltaTime;

			// Return if not enough time has elapsed.
			if (_updateTimer < _updateRate)
				return;

			_updateTimer -= _updateRate;

			// Add change to queue and reset changeDuringPeriod
			// Get the current time.
			System.DateTime now = System.DateTime.UtcNow;
			_timestampData.Enqueue(new ChangeTimeStamp(now, _changeDuringPeriod));
			_changeDuringPeriod = 0;

			//int accumlative = 0;
			List<float> medianAmounts = new List<float>();

			// Loop through all timestamps and calculate the rate of change.
			for (int i = _timestampData.Count - 1; i >= 0; i--)
			{
				ChangeTimeStamp cts = _timestampData.Dequeue();
				double timeDifference = (now - cts.TimeStamp).TotalSeconds;

				if (timeDifference < _timePeriod)
				{
					medianAmounts.Add(cts.Change);
					_timestampData.Enqueue(cts);
				}
			}

			if (medianAmounts.Count < 10)
				return;

			int half = medianAmounts.Count / 2;
			List<float> plottedPoints = new List<float>();
			for (int i = 2; i < medianAmounts.Count - 3; i++)
			{
				plottedPoints.Add((medianAmounts[i - 2] + medianAmounts[i - 1] + medianAmounts[i] + medianAmounts[i + 1] + medianAmounts[i + 2]) / 5.0f);
			}

			float movingMean = 0;

			for (int i = 0; i < plottedPoints.Count; i++)
			{
				movingMean += plottedPoints[i];
			}

			movingMean /= plottedPoints.Count;

			_averageOverTime = (int)(movingMean * 60 * 60);
		}

		/// <summary>
		/// Called when a town resource value changes and enqueues the rate of change to be calculated.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		private void OnResourceChange(Utils.Resource resource, int amount, bool purchase)
		{
			// If it was a purchase, we don't want to calculate the rate of change as it messes up the data.
			if (resource != _resourceType || purchase)
				return;

			_changeDuringPeriod += amount;
			//System.DateTime dateTime = System.DateTime.UtcNow;
			//_timestampData.Enqueue(new ChangeTimeStamp(dateTime, amount));
		}
	}
}