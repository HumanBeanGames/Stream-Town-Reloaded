using GameEventSystem;
using System;
using UnityEngine;
using Utils;

namespace Managers
{
	/// <summary>
	/// Handles all current Game State Logic.
	/// </summary>
	public static class GameStateManager
	{
		private static bool _playerReady;
		private static bool _worldGenerated;
		private static bool _objectsPooled;
		private static bool _eventLogging = true;

		/// <summary>
		/// Player control is active and ready.
		/// </summary>
		public static bool PlayerReady
		{
			get => _playerReady;
			private set
			{
				if (_playerReady == value) return;
				_playerReady = value;
				ReadiedPlayer?.Invoke();
			}
		}
		public static event Action ReadiedPlayer;

		/// <summary>
		/// The world has been loaded.
		/// </summary>
		public static bool WorldGenerated
		{
			get => _worldGenerated;
			private set
			{
				if (_worldGenerated == value) return;
				_worldGenerated = value;
				GeneratedWorld?.Invoke();
			}

		}
		public static event Action GeneratedWorld;

		/// <summary>
		/// All objects have been pooled.
		/// </summary>
		public static bool ObjectsPooled
		{
			get => _objectsPooled;
			private set
			{
				if (_objectsPooled == value) return;
				_objectsPooled = value;
				PooledObjects?.Invoke();
			}
		}
		public static event Action PooledObjects;

		public static bool EventLogging
		{
			get => _eventLogging;
			set => _eventLogging = value;
		}

		/// <summary>
		/// Notifies the State Manager that the Player is ready.
		/// </summary>
		public static void NotifyPlayerReady()
		{
			PlayerReady = true;

			if (_eventLogging)
				Debug.Log("Player Ready");
		}

		/// <summary>
		/// Notifies the State Manager that the world has finished loading.
		/// </summary>
		public static void NotifyWorldLoaded()
		{
			WorldGenerated = true;

			if (_eventLogging)
				Debug.Log("World Loaded");
		}

		/// <summary>
		/// Notifies the State Manager that all objects have been pooled.
		/// </summary>
		public static void NotifyObjectsPooled()
		{
			ObjectsPooled = true;

			if (_eventLogging)
				Debug.Log("Pooling Finished");
		}

		/// <summary>
		/// Notifies the State Manager that a new world is being Loaded.
		/// </summary>
		public static void NotifyLoadingWorld()
		{
			ResetStateFlags();

			if (_eventLogging)
				Debug.Log("Loading New World");
		}

		/// <summary>
		/// Resets all State Manager data to default.
		/// </summary>
		public static void ResetStateFlags()
		{
			_playerReady = false;
			_worldGenerated = false;
			_objectsPooled = false;
		}
	}
}