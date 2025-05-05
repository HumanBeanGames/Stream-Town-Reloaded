using Managers;
using System;
using UnityEngine;
using World;

namespace GameEventSystem
{
	/// <summary>
	/// Holds data relating to an In Game Event.
	/// </summary>
	public class GameEvent
	{
		[Serializable]
		public enum EventType
		{
			None,
			FishGod,
			NightRaid,
			BloodMoonRaid,
			AdventureLandNecro,
			AdventureLandFishGod,
			DragonFire,
			DragonForest,
			DragonIce,
			DragonTwoHeaded,
			DragonUndead,
			Subscription,
			BitsDonated,
			Vote,
			MonsterRaid,
			NewKingVote,
			KeepKingVote,
			TechVote,
			Count
		}
		protected bool _alwaysReturnSuccess = false;
		protected object _returnData;
		/// <summary>
		/// When the event will start.
		/// </summary>
		protected double _eventStartTime;
		/// <summary>
		/// How long the event will last for, leave as -1 for undetermined.
		/// </summary>
		protected double _eventDuration;
		/// <summary>
		/// The Type of event.
		/// </summary>
		private EventType _eventType;
		/// <summary>
		/// Any extra data that needs to be passed for this event.
		/// </summary>
		private object _data;
		/// <summary>
		/// If this event can override the currently running event.
		/// </summary>
		private bool _overrideCurrentEvent;
		/// <summary>
		/// How long after the start time can this event still start? -1 for no timeout.
		/// </summary>
		private double _timeout;

		private Action EventStarted;
		public Action<bool, EventType, object> EventEnded;

		private bool _success;
		private bool _active;

		public double StartTime => _eventStartTime;
		public double EventDuration => _eventDuration;
		public EventType Event => _eventType;
		public object Data => _data;
		public bool OverrideCurrentEvent => _overrideCurrentEvent;
		public double Timeout => _timeout;

		public double RemainingDuration => _active ? (StartTime + EventDuration) - WorldUtils.CurrentTime : -1;
		public bool Success => _success;

		public GameEvent(double delay, double eventDuration, EventType eventType = EventType.None, object data = null, bool overrideCurrentEvent = false, double timeout = -1)
		{
			_eventStartTime = WorldUtils.CurrentTime + delay;
			_eventDuration = eventDuration;
			_eventType = eventType;
			_data = data;
			_overrideCurrentEvent = overrideCurrentEvent;
			_timeout = timeout;
		}

		/// <summary>
		/// Starts the Event.
		/// </summary>
		internal void Start(bool force = false)
		{
			_eventStartTime = WorldUtils.CurrentTime;
			_active = true;
			OnStarted();
			EventStarted?.Invoke();
		}

		/// <summary>
		/// Stops the Event.
		/// </summary>
		internal void Stop(bool completedSuccessfully = false)
		{
			_success = _alwaysReturnSuccess ? true : completedSuccessfully;
			Debug.Log($"Event Finished: " + (_success ? "successful" : "failed"));
			_active = false;
			OnStopped();
			EventEnded?.Invoke(_success, Event, _returnData);
		}

		public virtual void Update() { }
		/// <summary>
		/// Processes any action logic.
		/// </summary>
		/// <param name="data"></param>
		public void Action(object data = null)
		{
			OnActioned(data);
		}

		protected void OnCompleteEvent()
		{
			Stop(true);
		}

		/// <summary>
		/// Called when the event starts
		/// </summary>
		protected virtual void OnStarted() { }

		/// <summary>
		/// Called when th event ends
		/// </summary>
		protected virtual void OnStopped() { }

		/// <summary>
		/// Called when the event is actioned.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void OnActioned(object data = null) { }

		/// <summary>
		/// Called every frame.
		/// </summary>
		protected void OnUpdate()
		{
			if (WorldUtils.CurrentTime >= EventDuration + StartTime)
				Stop(false);
		}

	}
}