using System;
using System.Collections.Generic;
using Target;
using UnityEngine;
using Utils;

namespace Managers
{
	//TODO:: Check if this is still required after BSP implementation
	public class TargetManager : MonoBehaviour
	{
		[SerializeField]
		private Dictionary<TargetMask, List<Targetable>> _targetDictionary = new Dictionary<TargetMask, List<Targetable>>();
		[SerializeField]
		private TargetableData[] _targetableData;

		public StationUpdate GetUpdateType(TargetMask type)
		{
			return _targetableData[TargetFlagHelper.GetIndexByFlag(type)].UpdateType;
		}

        public List<Targetable> GetSingleTargetList(TargetMask type)
        {
            if (_targetDictionary.TryGetValue(type, out var list))
            {
                return list;
            }

            Debug.LogWarning($"[TargetManager] No list found for TargetMask: {type}");
            return new List<Targetable>();
        }


        /// <summary>
        /// Gets all targets defined by the flag into one list.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<Targetable> GetTargetsByFlag(TargetMask flag)
		{
			List<Targetable> targets = new List<Targetable>();

			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{

				TargetMask t = (TargetMask)i;

				if (t == TargetMask.Nothing)
					continue;

				if (!flag.HasFlag(t) || !_targetDictionary.ContainsKey(t))
					continue;

				targets.AddRange(_targetDictionary[t]);

			}

			return targets;
		}

		/// <summary>
		/// Adds a target to the target dictionary
		/// </summary>
		/// <param name="target"></param>
		public void AddTarget(Targetable target)
		{
			// Add to each flag type
			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{

				TargetMask t = (TargetMask)i;

				if (t == TargetMask.Nothing)
					continue;

				if (target.TargetType.HasFlag(t))
				{
					AddTarget(t, target);
				}
			}
		}

		/// <summary>
		/// Removes a target from the target dictionary
		/// </summary>
		/// <param name="target"></param>
		public void RemoveTarget(Targetable target)
		{
			foreach (int i in Enum.GetValues(typeof(TargetMask)))
			{
				TargetMask t = (TargetMask)i;

				if (t == TargetMask.Nothing)
					continue;

				if (target.TargetType.HasFlag(t))
				{
					RemoveTarget(t, target);
				}
			}
		}

		/// <summary>
		/// Adds a Targetable object to the target dictionary.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target"></param>
		private void AddTarget(TargetMask type, Targetable target)
		{
			if (!_targetDictionary.ContainsKey(type))
				_targetDictionary[type] = new List<Targetable>();

			if (_targetDictionary[type].Contains(target))
				return;

			_targetDictionary[type].Add(target);
		}

		/// <summary>
		/// Removes a Targetable object from the target dictionary.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target"></param>
		private void RemoveTarget(TargetMask type, Targetable target)
		{
			if (!_targetDictionary.ContainsKey(type))
				return;

			if (!_targetDictionary[type].Contains(target))
				return;

			_targetDictionary[type].Remove(target);
		}
	}
}