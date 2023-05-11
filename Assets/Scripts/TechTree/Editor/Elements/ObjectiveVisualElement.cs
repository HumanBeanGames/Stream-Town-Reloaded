using TownGoal.Data.Save;
using TownGoal.Enumerations;
using System;
using TechTree.Utilities;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils;

namespace TechTree.Elements
{
	/// <summary>
	/// Holds the data for a node's objectives.
	/// </summary>
	public class ObjectiveVisualElement : VisualElement
	{
		public ObjectiveSaveData ObjectiveSaveData { get; set; }

		public Button RemoveButton { get; set; }

		public EnumField ObjectiveTypeEnumField { get; set; }

		public int ListIndex { get; set; }

		private TechTreeNode _parentNode;

		private VisualElement _objectiveExtraData;

		public void Init(ObjectiveSaveData objectiveSaveData, TechTreeNode node)
		{
			this.AddClasses("techtree-node__unlock-container");
			_parentNode = node;
			ObjectiveSaveData = objectiveSaveData;
			ListIndex = node.Objectives.IndexOf(objectiveSaveData);

			// Remove Button
			RemoveButton = TechTreeUtilities.CreateButton("X", () =>
			{
				node.Objectives.Remove(objectiveSaveData);
				node.ObjectivesFoldout.Remove(this);
			});

			// Main Objective Type -- Changing this should update the drawn elements
			ObjectiveTypeEnumField = TechTreeUtilities.CreateEnumField(objectiveSaveData.ObjectiveType, onValueChanged: callback =>
			{
				objectiveSaveData.ObjectiveType = (ObjectiveType)callback.newValue;
				OnObjectiveTypeChanged((ObjectiveType)callback.newValue);
			});

			Label label = new Label("Objective Type: ");
			label.AddClasses("techtree-node__unlock-label");

			ObjectiveTypeEnumField.Insert(0, RemoveButton);
			ObjectiveTypeEnumField.Insert(1, label);
			Add(ObjectiveTypeEnumField);

			// Objective Type Based Context
			OnObjectiveTypeChanged(objectiveSaveData.ObjectiveType);
		}

		private void OnObjectiveTypeChanged(ObjectiveType newValue)
		{
			if (_objectiveExtraData != null)
				Remove(_objectiveExtraData);

			_objectiveExtraData = new VisualElement();

			switch (newValue)
			{
				case ObjectiveType.Build:
					AddBuildingTypeField();
					AddIntField();
					break;
				case ObjectiveType.BuildAny:
					AddIntField();
					break;
				case ObjectiveType.Collect:
					AddResourceTypeField();
					AddIntField();
					break;
				case ObjectiveType.Kill:
					AddEnemyTypeField();
					AddIntField();
					break;
				case ObjectiveType.KillAny:
					AddIntField();
					break;
				case ObjectiveType.EarnPerHour:
					AddResourceTypeField();
					AddIntField();
					break;
				case ObjectiveType.Sell:
					AddResourceTypeField();
					AddIntField();
					break;
				case ObjectiveType.SellAny:
					AddIntField();
					break;
				case ObjectiveType.Buy:
					AddResourceTypeField();
					AddIntField();
					break;
				case ObjectiveType.BuyAny:
					AddIntField();
					break;

			}

			Add(_objectiveExtraData);
		}

		private void AddIntField(string label = "Amount: ")
		{
			IntegerField intField = TechTreeUtilities.CreateIntField(ObjectiveSaveData.IntValue, label, callback =>
			{
				ObjectiveSaveData.IntValue = callback.newValue;
			});

			_objectiveExtraData.Add(intField);
		}

		private void AddBuildingTypeField()
		{
			EnumField buildingTypeField = TechTreeUtilities.CreateEnumField(ObjectiveSaveData.BuildingType, "Building: ", callback =>
			{
				ObjectiveSaveData.BuildingType = (BuildingType)callback.newValue;
			});

			_objectiveExtraData.Add(buildingTypeField);
		}

		private void AddEnemyTypeField()
		{
			EnumField enemyTypeField = TechTreeUtilities.CreateEnumField(ObjectiveSaveData.EnemyType, "Enemy: ", callback =>
			{
				ObjectiveSaveData.EnemyType = (EnemyType)callback.newValue;
			});

			_objectiveExtraData.Add(enemyTypeField);
		}

		private void AddResourceTypeField()
		{
			EnumField resourceTypeField = TechTreeUtilities.CreateEnumField(ObjectiveSaveData.ResourceType, "Resource: ", callback =>
			{
				ObjectiveSaveData.ResourceType = (Resource)callback.newValue;
			});

			_objectiveExtraData.Add(resourceTypeField);
		}
	}
}