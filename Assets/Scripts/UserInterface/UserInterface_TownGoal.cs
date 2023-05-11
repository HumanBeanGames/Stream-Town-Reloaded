using System.Collections.Generic;
using TechTree.ScriptableObjects;
using TMPro;
using TownGoal.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
	public class UserInterface_TownGoal : MonoBehaviour
	{
		public Transform TownGoalContainer;
		public Image Icon;
		public TextMeshProUGUI TechTitle;
		public RectTransform ObjectivesContainer;
		public GameObject ObjectivePrefab;

		private Dictionary<Objective, UI_Objective> _followedObjectives;

		public void AddGoal(Goal goal, Node_SO node)
		{
			List<Objective> objectives = new List<Objective>();

			foreach (var kvp in goal.ObjectivesStatuses)
			{
				objectives.Add(kvp.Key);
			}

			for (int i = 0; i < objectives.Count; i++)
			{
				CreateNewObjective(objectives[i]);
			}

			TechTitle.text = node.NodeTitle;
			ActivateTownGoalContainer();
			goal.OnGoalCompleted += OnGoalFinished;
			string modPath = node.IconPath.Remove(0, 17);
			modPath = modPath.Remove(modPath.Length - 4, 4);
			Icon.sprite = Resources.Load<Sprite>(modPath) as Sprite;
			Debug.Log(modPath);
			//Icon = node Icon;
		}

		private void CreateNewObjective(Objective objective)
		{
			GameObject go = Instantiate(ObjectivePrefab, ObjectivesContainer);

			UI_Objective uiObj = go.GetComponent<UI_Objective>();

			uiObj.AmountTMP.text = $"{objective.Amount} / {objective.RequiredAmount}";
			switch (objective.ObjectiveType)
			{
				case TownGoal.Enumerations.ObjectiveType.Build:
					uiObj.ObjectiveText.text = $"Build {objective.RequiredAmount} {objective.Data.BuildingType}s";
					break;
				case TownGoal.Enumerations.ObjectiveType.BuildAny:
					uiObj.ObjectiveText.text = $"Build {objective.RequiredAmount} Buildings";
					break;
				case TownGoal.Enumerations.ObjectiveType.Collect:
					uiObj.ObjectiveText.text = $"Collect {objective.RequiredAmount} {objective.Data.ResourceType}";
					break;
				case TownGoal.Enumerations.ObjectiveType.Kill:
					uiObj.ObjectiveText.text = $"Kill {objective.RequiredAmount} {objective.Data.EnemyType}s";
					break;
				case TownGoal.Enumerations.ObjectiveType.KillAny:
					uiObj.ObjectiveText.text = $"Kill {objective.RequiredAmount} Enemies";
					break;
				case TownGoal.Enumerations.ObjectiveType.EarnPerHour:
					uiObj.ObjectiveText.text = $"Earn {objective.RequiredAmount} {objective.Data.ResourceType} per hour";
					break;
				case TownGoal.Enumerations.ObjectiveType.Buy:
					uiObj.ObjectiveText.text = $"Buy {objective.RequiredAmount} {objective.Data.ResourceType}";
					break;
				case TownGoal.Enumerations.ObjectiveType.BuyAny:
					uiObj.ObjectiveText.text = $"Buy {objective.RequiredAmount} Resources";
					break;
				case TownGoal.Enumerations.ObjectiveType.Sell:
					uiObj.ObjectiveText.text = $"Sell {objective.RequiredAmount} {objective.Data.ResourceType}";
					break;
				case TownGoal.Enumerations.ObjectiveType.SellAny:
					uiObj.ObjectiveText.text = $"Sell {objective.RequiredAmount} Resources";
					break;
				default:
					break;
			}

			uiObj.ObjectiveSlider.value = 0;

			objective.AmountChanged += AmountChanged;
			_followedObjectives.Add(objective, uiObj);
		}

		private void AmountChanged(Objective objective, int amount)
		{
			_followedObjectives[objective].AmountTMP.text = $"{amount} / {objective.RequiredAmount}";
			_followedObjectives[objective].ObjectiveSlider.value = (amount / (float)objective.RequiredAmount);
		}

		public void ActivateTownGoalContainer()
		{
			TownGoalContainer.gameObject.SetActive(true);
		}

		private void OnGoalFinished(Goal goal)
		{
			goal.OnGoalCompleted -= OnGoalFinished;
			DisableTownGoalContainer();
		}

		public void DisableTownGoalContainer()
		{
			if (_followedObjectives != null)
			{
				List<Objective> objectivesToRemove = new List<Objective>();

				foreach (var v in _followedObjectives)
					objectivesToRemove.Add(v.Key);

				for (int i = 0; i < objectivesToRemove.Count; i++)
				{
					Destroy(_followedObjectives[objectivesToRemove[i]].gameObject);
					objectivesToRemove[i].AmountChanged -= AmountChanged;
					_followedObjectives.Remove(objectivesToRemove[i]);
				}
			}

			_followedObjectives = new Dictionary<Objective, UI_Objective>();
			TownGoalContainer.gameObject.SetActive(false);
		}

		private void Start()
		{
			DisableTownGoalContainer();
		}
	}
}