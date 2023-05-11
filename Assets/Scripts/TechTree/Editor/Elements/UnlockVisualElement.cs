using System.Collections.Generic;
using TechTree.Data.Save;
using TechTree.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace TechTree.Elements
{
	/// <summary>
	/// Holds visual elements for node Unlocks.
	/// </summary>
	public class UnlockVisualElement : VisualElement
	{
		public NodeUnlockSaveData TechNodeUnlockSaveData { get; private set; }

		public Button RemoveButton { get; private set; }

		public EnumField TechTypeEnumField { get; private set; }

		// Need to insert other data

		public int ListIndex { get; set; }

		private VisualElement _unlockExtraData;

		public void Init(NodeUnlockSaveData unlockSaveData, TechTreeNode node)
		{
			this.AddClasses("techtree-node__unlock-container");
			TechNodeUnlockSaveData = unlockSaveData;
			ListIndex = node.Unlocks.IndexOf(unlockSaveData);

			//Remove Button
			RemoveButton = TechTreeUtilities.CreateButton("X", () =>
			{
				node.Unlocks.Remove(unlockSaveData);
				node.UnlocksFoldout.Remove(this);
			});

			// Main Tech Type Enum - Changing this should update the drawn elements
			TechTypeEnumField = TechTreeUtilities.CreateEnumField(unlockSaveData.TechType, onValueChanged: callback =>
			 {
				 unlockSaveData.TechType = (TechType)callback.newValue;
				 OnTechTypeChanged((TechType)callback.newValue);
			 });

			Label label = new Label("Tech Type: ");

			label.AddClasses("techtree-node__unlock-label");

			TechTypeEnumField.Insert(0, RemoveButton);
			TechTypeEnumField.Insert(1, label);
			Add(TechTypeEnumField);

			// Tech Type Based Context
			OnTechTypeChanged(unlockSaveData.TechType);

		}

		private void OnTechTypeChanged(TechType newValue)
		{
			//Recreate the unlock data element
			if (_unlockExtraData != null)
				Remove(_unlockExtraData);

			_unlockExtraData = new VisualElement();

			switch (newValue)
			{
				case TechType.StatBoost:
					AddStatTypeField();
					AddIntField("Additive %: ");
					AddRoleField();
					break;
				case TechType.UnlockBuilding:
					AddBuildingField();
					break;
				case TechType.BuildingCostReduction:
					AddBuildingField();
					AddIntField("Additive %: ");
					break;
				case TechType.StorageBoost:
					AddResourceField();
					AddIntField("Additive %: ");
					break;
				case TechType.UpgradeBuilding:
					AddBuildingField();
					AddIntField("Max Level: ");
					break;
				case TechType.AgeUpBuilding:
					AddBuildingField();
					break;
			}
			Add(_unlockExtraData);
		}

		private void AddFloatField(string label)
		{
			FloatField floatValue = TechTreeUtilities.CreateFloatField(TechNodeUnlockSaveData.FloatValue, label, callback =>
			{
				TechNodeUnlockSaveData.FloatValue = callback.newValue;
			});
			_unlockExtraData.Add(floatValue);
		}

		private void AddStatTypeField()
		{
			EnumField statType = TechTreeUtilities.CreateEnumField(TechNodeUnlockSaveData.StatType, "Stat Type: ", callback =>
			{
				TechNodeUnlockSaveData.StatType = (StatType)callback.newValue;
			});

			_unlockExtraData.Add(statType);
		}

		private void AddRoleField()
		{
			EnumField roleType = TechTreeUtilities.CreateEnumField(TechNodeUnlockSaveData.PlayerRole, "Role: ", callback =>
			{
				TechNodeUnlockSaveData.PlayerRole = (PlayerRole)callback.newValue;
			});

			_unlockExtraData.Add(roleType);
		}

		private void AddBuildingField()
		{
			EnumField buildingType = TechTreeUtilities.CreateEnumField(TechNodeUnlockSaveData.BuildingType, "Building: ", callback =>
			{
				TechNodeUnlockSaveData.BuildingType = (BuildingType)callback.newValue;
			});

			_unlockExtraData.Add(buildingType);
		}

		private void AddResourceField()
		{
			EnumField resourceType = TechTreeUtilities.CreateEnumField(TechNodeUnlockSaveData.ResourceType, "Resource: ", callback =>
			{
				TechNodeUnlockSaveData.ResourceType = (Resource)callback.newValue;
			});

			_unlockExtraData.Add(resourceType);
		}

		private void AddIntField(string label)
		{
			IntegerField intField = TechTreeUtilities.CreateIntField(TechNodeUnlockSaveData.IntValue, label, callback =>
			{
				TechNodeUnlockSaveData.IntValue = callback.newValue;
			});

			_unlockExtraData.Add(intField);
		}
	}
}