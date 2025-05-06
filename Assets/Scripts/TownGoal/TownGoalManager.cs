using System.Collections.Generic;
using UnityEngine;

namespace TownGoal
{
    using Data;
    using Managers;
    using Sirenix.OdinInspector;


    [GameManager]
    public static class TownGoalManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static TownGoalConfig Config = TownGoalConfig.Instance;

		[HideInInspector]
        public static int MaxGoals = 2;

        [HideInInspector]
        private static List<Goal> _currentGoals;

		public static List<Goal> CurrentGoals => _currentGoals;

		// Objective UI.

		public static void Initialize()
		{
			_currentGoals = new List<Goal>(1);
		}

		public static bool StartNewGoal(Goal goal)
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

		public static bool ForceStopGoal(Goal goal)
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

		private static void OnGoalCompleted(Goal goal)
		{
			goal.OnGoalCompleted -= OnGoalCompleted;
			if (!_currentGoals.Contains(goal))
				return;

			Debug.Log("GOAL COMPLETED!");

			RemoveUIElement(goal);
			_currentGoals.Remove(goal);

		}

		private static void BuildObjectiveUI(Objective objective)
		{

		}

		private static void RemoveUIElement(Goal goal)
		{

		}
	}
}