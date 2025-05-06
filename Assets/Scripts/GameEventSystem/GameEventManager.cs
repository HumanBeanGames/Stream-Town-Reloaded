using Buildings;
using Character;
using GameEventSystem.Events.Voting;
using Managers;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TechTree;
using TownGoal.Data;
using Twitch;
using UnityEngine;
using World;

namespace GameEventSystem
{
	/// <summary>
	/// Handles all Game Events, queueing up the events to be dispatched in order.
	/// </summary>
	[GameManager]
	public static class GameEventManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static GameEventConfig Config = GameEventConfig.Instance;

		private static float _rulerVoteMinTime => Config.rulerVoteMinTime;

		[HideInInspector]
		private static float _timeUntilRulerVote = 30;

        [HideInInspector]
        private static Transform _fishGodSpawn;
		public static Transform FishGodSpawn
		{
			get
			{
				if (_fishGodSpawn == null)
				{
					GameObject foundObject = GameObject.FindWithTag("FishGodSpawn");
					if (foundObject != null)
					{
						_fishGodSpawn = foundObject.transform;
					}
				}
				return _fishGodSpawn;
			}
			set { _fishGodSpawn = value; }
		}

        [HideInInspector]
        private static ParticleSystem _fallingFishVFX;
		public static ParticleSystem FallingFishVFX
		{
			get
			{
				if (_fallingFishVFX == null)
				{
					GameObject foundObject = GameObject.FindWithTag("RainingFish");
					if (foundObject != null)
					{
						_fallingFishVFX = foundObject.GetComponent<ParticleSystem>();
					}
				}
				return _fallingFishVFX;
			}
			set { _fallingFishVFX = value; }
		}

        [HideInInspector]
        private static bool _logEvents = true;

        [HideInInspector]
        private static SortedSet<GameEvent> _eventQueue = new SortedSet<GameEvent>(new SortGameEventStartTime());

        [HideInInspector]
        private static GameEvent _currentEvent = null;
        [HideInInspector]
        private static bool _eventActive = false;

        [HideInInspector]
        private static bool _canStartNewRulerVote;

		public static GameEvent CurrentEvent => _currentEvent;
		public static float TimeTillRulerVote
		{
			get { return _timeUntilRulerVote; }	
			set { _timeUntilRulerVote = value;}
		}
		public static bool CanStartNewRulerVote
		{
			get { return _canStartNewRulerVote; }
			set { _canStartNewRulerVote = value;}
		}

        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner _runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            GameObject runnerObject = new GameObject("GameEventRunner");
            _runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
        }

        public static void StartCoroutine(IEnumerator routine) => _runner.StartCoroutine(routine);
		public static void StopCoroutine(IEnumerator routine) => _runner.StopCoroutine(routine);

		public static bool AddEvent(GameEvent gameEvent)
		{
			if (EventTypeExistsInQueue(gameEvent.Event))
				return false;

			_eventQueue.Add(gameEvent);

			if (_logEvents)
				Debug.Log($"Game Event Added: '{gameEvent.Event}'");

			return true;
		}

		public static bool CreateEvent(double delay, double eventDuration, GameEvent.EventType eventType, object data = null, bool overrideCurrentEvent = false, double timeout = -1)
		{
			if (EventTypeExistsInQueue(eventType))
				return false;

			_eventQueue.Add(new GameEvent(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout));

			if (_logEvents)
				Debug.Log($"Game Event Added: '{eventType}'");

			return true;
		}

		public static void DisposeEventsQueue()
		{
			_eventQueue.Clear();
			if (_logEvents)
				Debug.Log($"Game Event Queue Disposed.");
		}

		public static GameEvent PeekNextEvent()
		{
			if (_eventQueue.Count == 0)
				return null;

			return _eventQueue.Min;
		}

		public static bool EventTypeExistsInQueue(GameEvent.EventType type)
		{
			if (_currentEvent != null && _currentEvent.Event == type)
				return true;

			if (_eventQueue.Count == 0)
				return false;

			IEnumerator<GameEvent> enumerator = _eventQueue.GetEnumerator();

			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Event == type)
					return true;
			}

			return false;
		}

		public static void ProcessEvents()
		{
			if (_currentEvent != null)
				_currentEvent.Update();
			HandleRulerVoting();

			double currentTime = WorldUtils.CurrentTime;

			// Check if we should end the current event 
			if (_currentEvent != null && _currentEvent.EventDuration + _currentEvent.StartTime <= currentTime)
				EndCurrentEvent();

			if (_eventQueue.Count == 0)
				return;

			GameEvent nextEvent = PeekNextEvent();

			if (nextEvent == null)
				return;

			if ((_currentEvent != null && nextEvent.OverrideCurrentEvent) || _currentEvent == null)
			{
				// Check if the event should start
				if (nextEvent.StartTime <= currentTime)
				{
					// Check if the event has timed out
					if (nextEvent.Timeout == -1)
						StartNextEvent();
					else if (nextEvent.Timeout + nextEvent.StartTime <= currentTime)
					{
						_eventQueue.Remove(nextEvent);
						Debug.Log($"Event Timed Out: '{nextEvent.Event}', removed from queue.");
					}
					else if (nextEvent.Timeout + nextEvent.StartTime >= currentTime)
					{
						StartNextEvent();
					}
				}
			}
		}

		public static void EndCurrentEvent()
		{
			_eventActive = false;

			if (_currentEvent == null)
				return;

			if (_logEvents)
				Debug.Log($"Event Stopped: '{_currentEvent.Event}'.");

			_currentEvent.Stop();

			_currentEvent = null;
		}

		private static void StartNextEvent()
		{
			if (_eventActive)
				EndCurrentEvent();

			_currentEvent = _eventQueue.Min;
			_eventQueue.Remove(_eventQueue.Min);
			_currentEvent.Start();
			_eventActive = true;

			if (_logEvents)
				Debug.Log($"Event Started: '{_currentEvent.Event}'.");

			_currentEvent.EventEnded += OnCurrentEventEnded;
		}

		private static void OnCurrentEventEnded(bool success, GameEvent.EventType eventType, object finishedEvent)
		{
			_currentEvent = null;
		}

		private static void HandleRulerVoting()
		{
			if (_canStartNewRulerVote)
			{
				_timeUntilRulerVote -= Time.deltaTime;

				if (_timeUntilRulerVote <= 0)
				{
					if (PlayerManager.Ruler != null)
					{
						StartKeepRulerVote();
					}
					else
					{
						StartNewRulerVote();
					}

					_timeUntilRulerVote = _rulerVoteMinTime;
				}
			}
		}

		public static void StartKeepRulerVote()
		{
			Debug.Log("Keep ruler vote");
			KeepKingVote keepKingVote = new KeepKingVote(1, 120, timeout: 3600);
			if (AddEvent(keepKingVote))
				keepKingVote.EventEnded += OnKeepRulerVoteEnded;
		}

		public static void StartNewRulerVote()
		{
			Debug.Log("New ruler vote");
			NewKingVote newKingVote = new NewKingVote(1, 120, timeout: 3600);
			if (AddEvent(newKingVote))
				newKingVote.EventEnded += OnNewRulerVoteEnded;
		}

		private static void OnKeepRulerVoteEnded(bool success, GameEvent.EventType eventType, object data)
		{
			if (data == null)
			{
				Debug.Log("No Votes Were Cast");
				return;
			}

			VoteOption option = data as VoteOption;

			if (option.OptionName == "yes")
			{
				// Keep Ruler.
			}
			else
			{
				// Remove Ruler.
				PlayerManager.SetRuler(null);
				// Start new Ruler vote event
				StartNewRulerVote();
			}
		}

		private static void OnNewRulerVoteEnded(bool success, GameEvent.EventType eventType, object data)
		{
			if (data == null)
			{
				Debug.Log("No Votes Were Cast");
				return;
			}

			VoteOption option = data as VoteOption;
			if (PlayerManager.PlayerExistsByNameToLower(option.OptionName, out int index))
			{
				PlayerManager.SetRuler(PlayerManager.GetPlayer(index));
				Debug.Log($"Winner Was {option.OptionName}");
			}
			else
				Debug.Log("No Player Found");
		}
	}
}