using Buildings;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UserInterface;
using Utils;

namespace Managers
{
	/// <summary>
	/// Manages all stations in the game.
	/// </summary>
	[GameManager]
	public static class StationManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static StationConfig Config = StationConfig.Instance;

		private static Dictionary<StationMask, List<Station>> _stationsDictionary => Config.stationsDictionary;

		/// <summary>
		/// Queue of stations that need to have their targets updated.
		/// </summary>
		private static Queue<Station> _stationUpdateQueue => Config.stationUpdateQueue;

		/// <summary>
		/// Queue of stations that need to check if their targets are disabled.
		/// </summary>
		private static Queue<Station> _clearDisabledQueue => Config.clearDisabledQueue;

		/// <summary>
		/// Adds the station to the update queue so that it's targets will be updated.
		/// </summary>
		/// <param name="station"></param>
		public static void UpdateStation(Station station)
		{
			if (_stationUpdateQueue.Contains(station))
				return;

			_stationUpdateQueue.Enqueue(station);
		}

        /// <summary>
        /// Adds a station to the station dictionary.
        /// </summary>
        /// <param name="station"></param>
        public static void AddStation(Station station)
		{
			foreach (int i in Enum.GetValues(typeof(StationMask)))
			{
				StationMask t = (StationMask)i;

				if (t == StationMask.Nothing)
					continue;

				if (station.Flags.HasFlag(t))
				{
					AddStation(t, station);
				}
			}
		}

        /// <summary>
        /// Removes station from station dictionary.
        /// </summary>
        /// <param name="station"></param>
        public static void RemoveStation(Station station)
		{
			foreach (int i in Enum.GetValues(typeof(StationMask)))
			{
				StationMask t = (StationMask)i;

				if (t == StationMask.Nothing)
					continue;

				if (station.Flags.HasFlag(t))
				{
					RemoveStation(t, station);
				}
			}
		}

        /// <summary>
        /// Uses StationFlags flag to get list of available stations.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static List<Station> GetStationsByFlag(StationMask flag)
		{
			List<Station> stations = new List<Station>();

			foreach (int i in Enum.GetValues(typeof(StationMask)))
			{
				StationMask t = (StationMask)i;

				if (t == StationMask.Nothing)
					continue;

				if (!flag.HasFlag(t) || !_stationsDictionary.ContainsKey(t))
					continue;

				stations.AddRange(_stationsDictionary[t]);
			}

			return stations;
		}

        /// <summary>
        /// Displays the stations ID for a given amount of time.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool DisplayStationIdByType(StationMask flags)
		{
			List<Station> _validStations = GetStationsByFlag(flags);

			if (_validStations.Count == 0)
			{
				return false;
			}

			for (int i = 0; i < _validStations.Count; i++)
			{
				UtilDisplayManager.AddTextDisplay(_validStations[i].Targetable, $"{i + 1}");
			}

			return true;
		}

        /// <summary>
        /// Attempts to get a Station based on flags and given index.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Station GetStationByFlaggedIndex(StationMask flags, int index)
		{
			List<Station> _validStations = GetStationsByFlag(flags);

			if (_validStations.Count <= index)
				return null;

			return _validStations[index];
		}

        /// <summary>
        /// Adds a new station to the dictionary based on it's mask.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="station"></param>
        private static void AddStation(StationMask mask, Station station)
		{
			// Check that the dictionary contains the key, otherwise create it.
			if (!_stationsDictionary.ContainsKey(mask))
				_stationsDictionary[mask] = new List<Station>();

			// Check that the station isn't already in the dictionary.
			if (_stationsDictionary[mask].Contains(station))
				return;

			// Add it to the dictionary and queue.
			_stationsDictionary[mask].Add(station);
			_clearDisabledQueue.Enqueue(station);
		}

        /// <summary>
        /// Removes an existing station from the dictionary based on it's flag.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="station"></param>
        private static void RemoveStation(StationMask mask, Station station)
		{
			// Check that the key existed in the dictionary.
			if (!_stationsDictionary.ContainsKey(mask))
				return;

			// Check that the station is actually in the dictionary.
			if (!_stationsDictionary[mask].Contains(station))
				return;

			// Remove it from the dictionary.
			_stationsDictionary[mask].Remove(station);
		}

        // Unity Functions.
        private static void Update()
		{
			if (_stationUpdateQueue.Count > 0)
			{
				_stationUpdateQueue.Dequeue().PopulateDictionary();
			}

			if (_clearDisabledQueue.Count > 0)
			{
				var station = _clearDisabledQueue.Dequeue();

				if (station != null && station.gameObject.activeInHierarchy)
				{
					_clearDisabledQueue.Enqueue(station);
					station.CheckDisabledTargets();
				}
			}
		}
	}
}