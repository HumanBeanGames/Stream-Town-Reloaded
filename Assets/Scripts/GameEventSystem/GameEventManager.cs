using Buildings;
using Character;
using GameEventSystem.Events.Voting;
using Managers;
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
	public class GameEventManager : MonoBehaviour
	{
		private const float _RULER_VOTE_MIN_TIME = 3600;
		private float _timeUntilRulerVote;

		[SerializeField]
		private Transform _fishGodSpawn;
		[SerializeField]
		private ParticleSystem _fallingFishVFX;
		[SerializeField]
		private bool _logEvents = true;

		private SortedSet<GameEvent> _eventQueue = new SortedSet<GameEvent>(new SortGameEventStartTime());

		private GameEvent _currentEvent = null;
		private bool _eventActive = false;
		private GameManager _gameManager;
		private PlayerManager _playerManager;
		
		private bool _canStartNewRulerVote = false;

		public GameEvent CurrentEvent => _currentEvent;
		public Transform FishGodSpawn => _fishGodSpawn;
		public ParticleSystem FallingFishVFX => _fallingFishVFX;
		public float TimeTillRulerVote
		{
			get { return _timeUntilRulerVote; }	
			set { _timeUntilRulerVote = value;}
		}
		public bool CanStartNewRulerVote
		{
			set { _canStartNewRulerVote = value;}
		}

		/// <summary>
		/// Adds an event to the event queue.
		/// </summary>
		/// <param name="gameEvent"></param>
		public bool AddEvent(GameEvent gameEvent)
		{
			if (EventTypeExistsInQueue(gameEvent.Event))
				return false;

			_eventQueue.Add(gameEvent);

			if (_logEvents)
				Debug.Log($"Game Event Added: '{gameEvent.Event}'");

			return true;
		}

		/// <summary>
		/// Creates an event and stores it in the event queue.
		/// </summary>
		/// <param name="delay">How long from the current time should the event start</param>
		/// <param name="eventDuration"></param>
		/// <param name="eventType"></param>
		/// <param name="data"></param>
		/// <param name="overrideCurrentEvent"></param>
		public bool CreateEvent(double delay, double eventDuration, GameEvent.EventType eventType, object data = null, bool overrideCurrentEvent = false, double timeout = -1)
		{
			if (EventTypeExistsInQueue(eventType))
				return false;

			_eventQueue.Add(new GameEvent(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout));

			if (_logEvents)
				Debug.Log($"Game Event Added: '{eventType}'");

			return true;
		}

		/// <summary>
		/// Remove all events in the current queue.
		/// </summary>
		public void DisposeEventsQueue()
		{
			_eventQueue.Clear();
			if (_logEvents)
				Debug.Log($"Game Event Queue Disposed.");
		}

		/// <summary>
		/// Returns the next event in the queue without removing it.
		/// </summary>
		/// <returns></returns>
		public GameEvent PeekNextEvent()
		{
			if (_eventQueue.Count == 0)
				return null;

			return _eventQueue.Min;
		}

		public bool EventTypeExistsInQueue(GameEvent.EventType type)
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

		/// <summary>
		/// Processes the event queue.
		/// </summary>
		public void ProcessEvents()
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

		/// <summary>
		/// Ends the current event by calling its stop function.
		/// </summary>
		public void EndCurrentEvent()
		{
			_eventActive = false;

			if (_currentEvent == null)
				return;

			if (_logEvents)
				Debug.Log($"Event Stopped: '{_currentEvent.Event}'.");

			_currentEvent.Stop();

			_currentEvent = null;
		}

		public void RunCoroutine(IEnumerator func)
		{
			StartCoroutine(func);
		}

		/// <summary>
		/// Starts the next available event and removes it from the queue.
		/// </summary>
		private void StartNextEvent()
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

		private void OnCurrentEventEnded(bool success, GameEvent.EventType eventType, object finishedEvent)
		{
			_currentEvent = null;
		}

		private void HandleRulerVoting()
		{
			if (_canStartNewRulerVote)
			{
				_timeUntilRulerVote -= Time.deltaTime;

				if (_timeUntilRulerVote <= 0)
				{
					if (_playerManager.Ruler != null)
					{
						StartKeepRulerVote();
					}
					else
					{
						StartNewRulerVote();
					}

					_timeUntilRulerVote = _RULER_VOTE_MIN_TIME;
				}
			}
		}

		public void StartKeepRulerVote()
		{
			Debug.Log("Keep ruler vote");
			KeepKingVote keepKingVote = new KeepKingVote(1, 120, timeout: 3600);
			if (AddEvent(keepKingVote))
				keepKingVote.EventEnded += OnKeepRulerVoteEnded;

		}

		public void StartNewRulerVote()
		{
			Debug.Log("New ruler vote");
			NewKingVote newKingVote = new NewKingVote(1, 120, timeout: 3600);
			if (AddEvent(newKingVote))
				newKingVote.EventEnded += OnNewRulerVoteEnded;
		}

		private void OnKeepRulerVoteEnded(bool success, GameEvent.EventType eventType, object data)
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
				_playerManager.SetRuler(null);
				// Start new Ruler vote event
				StartNewRulerVote();
			}
		}

		private void OnNewRulerVoteEnded(bool success, GameEvent.EventType eventType, object data)
		{
			if (data == null)
			{
				Debug.Log("No Votes Were Cast");
				return;
			}

			VoteOption option = data as VoteOption;
			if (_playerManager.PlayerExistsByNameToLower(option.OptionName, out int index))
			{
				_playerManager.SetRuler(_playerManager.GetPlayer(index));
				Debug.Log($"Winner Was {option.OptionName}");
			}
			else
				Debug.Log("No Player Found");
		}

		private void Start()
		{
			_gameManager = GameManager.Instance;
			_playerManager = _gameManager.PlayerManager;
			_timeUntilRulerVote = 30;
		}

	}
}