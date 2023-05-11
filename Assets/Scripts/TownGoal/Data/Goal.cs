using TownGoal.Data;
using System.Collections.Generic;
using UnityEngine;
using System;
using SavingAndLoading.Structs;

namespace TownGoal.Data
{
	public class Goal
	{
		public Action<Goal> OnGoalCompleted;

		private Dictionary<Objective, bool> _objectiveStatuses;

		public Dictionary<Objective, bool> ObjectivesStatuses => _objectiveStatuses;

		public Goal(List<ObjectiveData> objectiveData)
		{
			_objectiveStatuses = new Dictionary<Objective, bool>();
			for (int i = 0; i < objectiveData.Count; i++)
			{
				Objective newObjective = new Objective(objectiveData[i]);
				newObjective.ObjectiveComplete += OnObjectiveComplete;
				_objectiveStatuses.Add(newObjective, false);
			}

			if (_objectiveStatuses.Count == 0)
				CheckAllObjectivesComplete();
		}

		public void SetobjectivesFromSave(List<ObjectiveSaveData> data)
		{
			int index = 0;
			foreach (Objective objective in _objectiveStatuses.Keys)
			{
				objective.SetValues(data[index].Amount, data[index].RequiredAmount);
			}
		}

		public void ForceComplete()
		{
			if (_objectiveStatuses.Count <= 0)
			{
				Debug.LogError("No Objectives Set!");
			}
			else
			{
				var keys = new List<Objective>(_objectiveStatuses.Keys);

				for(int i = 0; i < keys.Count;i++)
				{
					_objectiveStatuses[keys[i]] = true;
					keys[i].CompleteObjective();
				}	
			}
			CheckAllObjectivesComplete();
		}

		private void OnObjectiveComplete(Objective objective)
		{
			_objectiveStatuses[objective] = true;
			CheckAllObjectivesComplete();
		}

		private void CheckAllObjectivesComplete()
		{
			bool allPassed = true;

			foreach (KeyValuePair<Objective, bool> status in _objectiveStatuses)
			{
				if (!status.Value)
				{
					allPassed = false;
					break;
				}

				allPassed = true;
			}

			if (allPassed)
				OnGoalCompleted?.Invoke(this);
		}
	}
}