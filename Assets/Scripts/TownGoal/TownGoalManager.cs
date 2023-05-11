using System.Collections.Generic;
using UnityEngine;

namespace TownGoal
{
	using Data;
	using System;

	public class TownGoalManager : MonoBehaviour
	{
		public static int MaxGoals = 2;

		private List<Goal> _currentGoals;

		public List<Goal> CurrentGoals => _currentGoals;

		// Objective UI.

		public void Initialize()
		{
			_currentGoals = new List<Goal>(1);
		}

		public bool StartNewGoal(Goal goal)
		{
			if (_currentGoals.Contains(goal))
			{
				Debug.LogWarning($"Attempted to start goal that was already started '{goal}'");
				return false;
			}

			goal.OnGoalCompleted += OnGoalCompleted;
			_currentGoals.Add(goal);

			return true;
		}

		public bool ForceStopGoal(Goal goal)
		{
			List<KeyValuePair<Objective, bool>> statuses = new List<KeyValuePair<Objective, bool>>();
			foreach (var v in goal.ObjectivesStatuses)
				statuses.Add(v);

			for(int i = 0; i < statuses.Count;i++)
			{
				goal.ObjectivesStatuses[statuses[i].Key] = true;
				statuses[i].Key.CompleteObjective();
			}
			return true;
		}

		private void OnGoalCompleted(Goal goal)
		{
			goal.OnGoalCompleted -= OnGoalCompleted;
			if (!_currentGoals.Contains(goal))
				return;

			Debug.Log("GOAL COMPLETED!");

			RemoveUIElement(goal);
			_currentGoals.Remove(goal);

		}

		private void BuildObjectiveUI(Objective objective)
		{

		}

		private void RemoveUIElement(Goal goal)
		{

		}
	}
}