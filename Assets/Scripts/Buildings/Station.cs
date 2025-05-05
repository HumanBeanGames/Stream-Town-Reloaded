using System.Collections.Generic;
using Target;
using UnityEngine;
using Utils;
using System;
using Managers;
using UnityEngine.Profiling;
using Pathfinding;
using UserInterface;
using GUIDSystem;

namespace Buildings
{
	/// <summary>
	/// Used for Units to station at and recieve targets from.
	/// </summary>
	public class Station : MonoBehaviour
	{
		/// <summary>
		/// How often the Update Target Queue should be processed.
		/// </summary>
		private const float TARGET_CHECK_RATE = 2;

		/// <summary>
		/// Used to determine what type of Units can station here.
		/// </summary>
		[SerializeField, Tooltip("Used to determine what type of Units can station here.")]
		private StationMask _flags;

		/// <summary>
		/// Used to determine what type of targets are stored.
		/// </summary>
		[SerializeField, Tooltip("Used to determine what type of targets are stored.")]
		private TargetMask _targetMask;

		/// <summary>
		/// The max number of targets listed per flag in the Target Mask.
		/// </summary>
		[SerializeField, Tooltip("The max number of targets listed per flag in the Target Mask.")]
		private int _maxListSize = 5;

		/// <summary>
		/// Determines how often the target lists should be updated.
		/// </summary>
		[SerializeField, Tooltip("Determines how often the target lists should be updated.")]
		private float _updateRate = 25;
		private float _updateTimer = 0;

		/// <summary>
		/// The max distance the station will search for Targets.
		/// </summary>
		[SerializeField, Tooltip("The max distance the station will search for Targets")]
		private float _targetSearchRange = 50;

		/// <summary>
		/// A dictionary holding all targets by their type.
		/// </summary>
		private Dictionary<TargetMask, List<Targetable>> _targetDictionary;

		/// <summary>
		/// A queue of targets that updates one target a time.
		/// </summary>
		private Queue<Targetable> _updateTargets = new Queue<Targetable>();

		private float _targetCheckTimer = 0;

		/// <summary>
		/// Cached list of targets that is used to reduce garbage collection calls
		/// </summary>
		private List<Targetable> _cachedTargetsList = new List<Targetable>(10000);

		// Required Components.
		private Transform _transform;
		private StationManager _manager;
		private Targetable _targetable;
		private GUIDComponent _gUIDComponent;
		//private UnitTextDisplay _displayText;

		// Properties.
		public Targetable Targetable => _targetable;
		public StationMask Flags => _flags;
		public GUIDComponent GUIDComponent => _gUIDComponent;
		//public UnitTextDisplay DisplayText => _displayText;

		/// <summary>
		/// Marks a target as invalid and removes it from the dictionary
		/// </summary>
		/// <param name="t"></param>
		public void MarkTargetInvald(Targetable t)
		{
			// If the target only has one flag, simply remove it from the dictionary
			if (TargetFlagHelper.GetFlagCount((int)t.TargetType) == 1)
			{
				if (_targetDictionary[t.TargetType].Contains(t))
				{
					_targetDictionary[t.TargetType].Remove(t);
				}
			}
			// If the target has multiple flags, we need to loop through each dictionary key and remove it manually.
			else
			{
				foreach (KeyValuePair<TargetMask, List<Targetable>> kv in _targetDictionary)
				{
					if (kv.Value.Contains(t))
					{
						kv.Value.Remove(t);
					}
				}
			}
		}

		/// <summary>
		/// Populates the dictionaries with targets based on Target Mask
		/// </summary>
		public void PopulateDictionary()
		{
			Profiler.BeginSample("Populate Dictionary");

			// Get List of keys in dictionary
			List<TargetMask> keys = new List<TargetMask>(_targetDictionary.Keys);

			//Clear the list of cached targets and get a new list of targets.
			_cachedTargetsList.Clear();
			GameManager.Instance.CellPartitionGrid.GetTargetablesInRange(_targetMask, _transform.position, _targetSearchRange, ref _cachedTargetsList);

			// Set up for use in the sort.
			float closestDistSqr = float.MaxValue;
			Targetable closestTarget = null;
			float distance = 0;
			float loopCount = 0;

			// Clear the update target queue.
			_updateTargets.Clear();

			// Loop through each key type in the key list.
			foreach (TargetMask key in keys)
			{
				if (key == TargetMask.Nothing)
					continue;

				// If Targets update type is Clear, reset the list
				if (TargetManager.GetUpdateType(key) == StationUpdate.Clear || _targetDictionary[key] == null)
				{
					_targetDictionary[key] = new List<Targetable>(_maxListSize);
				}
				// Else we can simply update the target list
				else
				{
					// Loop through all targets currently in the list
					for (int i = _targetDictionary[key].Count - 1; i > 0; i--)
					{
						// If the target is inactive or it's target type no longer matches the key, remove it.
						if (!_targetDictionary[key][i].gameObject.activeInHierarchy || _targetDictionary[key][i].TargetType != key)
							_targetDictionary[key].RemoveAt(i);
					}

					// If we already have enough targets, skip this step.
					if (_targetDictionary[key].Count == _maxListSize)
						continue;
				}

				// Add all targets with the flag to a list.
				List<Targetable> keyTargets = _cachedTargetsList.FindAll(t => (t.TargetType & key) != 0);

				// Set max loop count so that we only get enough targets to fill the list.
				loopCount = Math.Max(Math.Min(keyTargets.Count, _maxListSize - _targetDictionary[key].Count), 0);

				// Loops until we have enough targets.
				for (int i = 0; i < loopCount; i++)
				{
					// Reset data after each closest target is acquired.
					closestDistSqr = float.MaxValue;
					closestTarget = null;

					// Loop through all targets in the list.
					for (int j = 0; j < keyTargets.Count; j++)
					{
						distance = Vector3.SqrMagnitude(keyTargets[j].CachedTransform.position - _transform.position);

						// Find the closest target.
						if (distance < closestDistSqr)
						{
							closestDistSqr = distance;
							closestTarget = keyTargets[j];
						}
					}

					// If we have a closest target add it to the dictionary and update queue.
					if (closestTarget != null)
					{
						if (!_targetDictionary[key].Contains(closestTarget))
							_targetDictionary[key].Add(closestTarget);

						if (!_updateTargets.Contains(closestTarget))
							_updateTargets.Enqueue(closestTarget);

						keyTargets.Remove(closestTarget);
					}
				}
			}
			Profiler.EndSample();
		}

		/// <summary>
		/// Loop through all targets in dictionary and check that they are not disabled, otherwise remove them.
		/// </summary>
		public void CheckDisabledTargets()
		{
			//TODO: Optimize by caching List, will need to update when a new object is added maybe
			List<TargetMask> keys = new List<TargetMask>(_targetDictionary.Keys);

			foreach (TargetMask key in keys)
			{
				for (int i = (_targetDictionary[key].Count - 1); i >= 0; i--)
				{
					if (!_targetDictionary[key][i].gameObject.activeInHierarchy)
						_targetDictionary[key].RemoveAt(i);
				}
			}

		}

		/// <summary>
		/// Remove target from dictionary
		/// </summary>
		/// <param name="target"></param>
		public void OnTargetNull(Targetable target)
		{
			// Get Flags of target and attempt to remove it from dictionary
			TargetMask typeFlag = target.TargetType;

			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{
				TargetMask t = (TargetMask)i;

				if (t == TargetMask.Nothing)
					continue;

				if (typeFlag.HasFlag(t) && _targetDictionary[t].Contains(target))
					_targetDictionary[t].Remove(target);

			}

		}

		/// <summary>
		/// Returns a random target from the Target Dictionary if one exists.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool GetRandomTarget(TargetMask type, ref Targetable target)
		{
			List<Targetable> validTargets = GetValidTargets(type);

			if (validTargets.Count == 0)
			{
				target = null;
				return false;
			}

			// Get a random target target from list of valid targets.
			target = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
			return true;
		}

		/// <summary>
		/// Returns the target with the best score. <br/>
		/// Scored based on Distance and Units already on the target.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="type"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool GetBestScoredTarget(Vector3 position, TargetMask type, ref Targetable target)
		{
			List<Targetable> validTargets = GetValidTargets(type);

			// If there are no valid targets, early out.
			if (validTargets.Count == 0)
			{
				target = null;
				return false;
			}

			float lowestScore = float.MaxValue;
			float score = 0;
			Targetable bestTarget = null;

			// Loop through all targets and find target with lowest (best) score.
			for (int i = 0; i < validTargets.Count; i++)
			{
				score = validTargets[i].CalculateScore(position);

				if (score < lowestScore && (type.HasFlag(validTargets[i].TargetType) || validTargets[i].TargetType.HasFlag(type)))
				{
					lowestScore = score;
					bestTarget = validTargets[i];
				}
			}

			target = bestTarget;

			return true;
		}

		public Targetable GetTargetByFlaggedIndex(TargetMask flag, int index)
		{
			List<Targetable> validTargets = GetValidTargets(flag);

			if (validTargets.Count <= index)
				return null;

			return validTargets[index];
		}

		/// <summary>
		/// Display's the valid targets ID's for a set time.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool DisplayTargetIDsByMask(TargetMask type)
		{
			
			List<Targetable> validTargets = GetValidTargets(type);

			for (int i = 0; i < validTargets.Count; i++)
			{
				UtilDisplayManager.AddTextDisplay(validTargets[i], $"{i + 1}");
			}

			return true;
		}

		/// <summary>
		/// Returns a list of all valid Targets based on flag.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private List<Targetable> GetValidTargets(TargetMask type)
		{
			List<Targetable> validTargets = new List<Targetable>();

			// Loop through all enum values
			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{
				TargetMask key = (TargetMask)i;

				// If key isnt part of the targets flag, continue.
				if (!type.HasFlag(key))
					continue;

				// If they dictionary doesn't contain the target type or any targets, continue.
				if (!_targetDictionary.ContainsKey(key) || _targetDictionary[key].Count == 0)
				{
					continue;
				}

				// Add Target to list of valid targets.
				validTargets.AddRange(_targetDictionary[key]);
			}

			return validTargets;

		}

		/// <summary>
		/// Updates a single target from the queue given that enough time has passed.
		/// </summary>
		private void UpdateQueueTarget()
		{
			_targetCheckTimer += Time.deltaTime;

			// If the queue contains targets and the target check rate has been reached...
			if (_updateTargets.Count > 0 && _targetCheckTimer >= TARGET_CHECK_RATE)
			{
				// ...Reset timer and get next target from the queue.
				_targetCheckTimer -= TARGET_CHECK_RATE;

				Targetable t = _updateTargets.Dequeue();

				// Check that the target type is not a fish, due to them having a special case of water.
				// TODO: May need to change this to not be hard coded.
				if (t.TargetType != TargetMask.Fish)
				{
					// Get the closest walkable graph nodes nearest the station and nearest the target.
					GraphNode node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
					GraphNode node2 = AstarPath.active.GetNearest(t.transform.position, NNConstraint.Default).node;

					// If the nodes are not valid of there is no path between them...
					if (node1 == null || node2 == null || !PathUtilities.IsPathPossible(node1, node2))
					{
						// Mark Target as invalid to remove it from Dictioary
						MarkTargetInvald(t);
					}
					// If target wasn't invalid, add it back to the Queue
					else
						_updateTargets.Enqueue(t);
				}
			}
		}

		// Unity Functions
		private void Awake()
		{
			_transform = transform;
			_targetDictionary = new Dictionary<TargetMask, List<Targetable>>();

			// Loop over all target types and create a list in the dictionary
			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{
				TargetMask t = (TargetMask)i;

				if (t == TargetMask.Nothing)
					continue;

				// If the flag is present in the TargetMask, add a new list.
				if (_targetMask.HasFlag(t))
					_targetDictionary.Add(t, new List<Targetable>());
			}

			_targetable = GetComponent<Targetable>();
			_gUIDComponent = GetComponent<GUIDComponent>();
		}

		private void Start()
		{
			_manager = GameManager.Instance.StationManager;

			// Add this station to the Station Manager.
			_manager.AddStation(this);
			PopulateDictionary();

			_updateTimer = UnityEngine.Random.Range(0, _updateRate);
		}

		private void Update()
		{
			_updateTimer += Time.deltaTime;

			// Checks if it is time to update and informated the Station Manager to Update the station.
			if (_updateTimer >= _updateRate)
			{
				_updateTimer -= _updateRate;
				_manager.UpdateStation(this);
			}

			UpdateQueueTarget();

		}

		private void OnEnable()
		{
			if (_manager != null)
				_manager.AddStation(this);
		}

		private void OnDisable()
		{
			if (_manager != null)
				_manager.RemoveStation(this);
		}

		private void OnDrawGizmosSelected()
		{
			if (_targetDictionary == null)
				return;

			Gizmos.DrawLine(transform.position, transform.position + transform.up * 5);

			// Draw a line to each target in dictionary.
			foreach (TargetMask key in _targetDictionary.Keys)
			{
				for (int i = 0; i < _targetDictionary[key].Count; i++)
				{
					Gizmos.DrawLine(transform.position + transform.up * 5, _targetDictionary[key][i].transform.position);
				}
			}
		}

	}
}