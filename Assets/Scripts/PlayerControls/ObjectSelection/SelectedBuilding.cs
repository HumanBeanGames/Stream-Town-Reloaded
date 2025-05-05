using UnityEngine;
using TMPro;
using Buildings;
using Units;
using Level;
using Managers;
using GameResources;

namespace PlayerControls.ObjectSelection
{
	public class SelectedBuilding : SelectedObject
	{
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			EnableDisplay();
			UpdateDisplay();
			SetBuildingName();
			AttachEvents();
			BuildingBase building = (BuildingBase)_selectedObject;
			if (building.BuildingType != Utils.BuildingType.Townhall)
			{
				_selectedObjectTypeUI.ButtonHolder.gameObject.SetActive(true);
				_selectedObjectTypeUI.SelectionButtonText.text = "Remove";
			}

			if (building.LevelHandler != null)
			{
				if (building.LevelHandler.Level == building.LevelHandler.MaxLevel)
				{
					_selectedObjectTypeUI.SelectionButtonTextTwo.text = "Max Level";
					_selectedObjectTypeUI.SelectionButtonTwo.interactable = false;
				}

				else
					_selectedObjectTypeUI.SelectionButtonTextTwo.text = "Level Up";

				if (BuildingManager.GetBuildingData(building.BuildingType).CanLevel && building.LevelHandler.CanLevel())
					_selectedObjectTypeUI.SelectionButtonTwo.interactable = true;
				else
					_selectedObjectTypeUI.SelectionButtonTwo.interactable = false;
			}
			else
				_selectedObjectTypeUI.ButtonHolderTwo.SetActive(false);
			//_selectedObjectTypeUI.Description.gameObject.SetActive(false);
		}

		protected override void AttachEvents()
		{
			BuildingBase building = (BuildingBase)_selectedObject;
			if (building.HealthHandler != null)
				building.HealthHandler.OnHealthChange += UpdateBuildingHealth;
			if (building.LevelHandler != null)
				building.LevelHandler.OnLeveledUp += UpdateBuildingLevel;

			if (building.BuildingType != Utils.BuildingType.Townhall)
			{
				OnButtonClick += OnRemoveButtonClick;
				_selectedObjectTypeUI.SelectionButton.onClick.AddListener(OnButtonClick);
				OnCheckConfirm += DisableCheck;
				OnCheckConfirm += RemoveBuilding;
				OnCheckDeny += DisableCheck;
			}
			if (building.BuildingType == Utils.BuildingType.Marketplace)
			{
				_selectedObjectTypeUI.RoleContainer.SetActive(true);
				PassiveResourceIncrementer incrementer = building.GetComponent<PassiveResourceIncrementer>();
				UpdateMarketPlaceRate(incrementer);
				incrementer.OnRateChange += UpdateMarketPlaceRate;
				_selectedObjectTypeUI.RoleImage.gameObject.SetActive(false);
				// Currently does not update to represent changing market values
			}

			OnButtonTwoClick += OnBuildingLevelUp;
			_selectedObjectTypeUI.SelectionButtonTwo.onClick.AddListener(OnButtonTwoClick);
			TownResourceManager.OnAnyResourceChangeEvent.AddListener(OnResourcesAdded);
		}

		private void OnResourcesAdded(Utils.Resource resource, int amount, bool yes)
		{
			BuildingBase building = (BuildingBase)_selectedObject;
			if (building.LevelHandler != null)
				if (building.LevelHandler.CanLevel())
					_selectedObjectTypeUI.SelectionButtonTwo.interactable = true;
		}

		protected override void DetachEvents()
		{
			BuildingBase building = (BuildingBase)_selectedObject;
			if (building.HealthHandler != null)
				building.HealthHandler.OnHealthChange -= UpdateBuildingHealth;
			if (building.LevelHandler != null)
				building.LevelHandler.OnLeveledUp -= UpdateBuildingLevel;

			if (building.BuildingType != Utils.BuildingType.Townhall)
			{
				OnButtonClick -= OnRemoveButtonClick;
				_selectedObjectTypeUI.SelectionButton.onClick.RemoveAllListeners();

			}
			else
			{
				OnCheckConfirm -= DisableCheck;
				OnCheckConfirm -= RemoveBuilding;
				OnCheckDeny -= DisableCheck;
			}
				_selectedObjectTypeUI.ConfirmCheck.RemoveListeners();
			if(building.BuildingType == Utils.BuildingType.Marketplace)
				building.GetComponent<PassiveResourceIncrementer>().OnRateChange -= UpdateMarketPlaceRate;

			_selectedObjectTypeUI.RoleImage.gameObject.SetActive(true);
			OnButtonTwoClick -= OnBuildingLevelUp;
			_selectedObjectTypeUI.SelectionButtonTwo.onClick.RemoveAllListeners();
		}

		public void OnRemoveButtonClick()
		{
			_selectedObjectTypeUI.ConfirmCheck.SetConfirmCheck(OnCheckConfirm, OnCheckDeny, "Do you wish to remove this building?", "This action is irreversable and will delete this building!");
		}

		public void RemoveBuilding()
		{
			BuildingBase building = (BuildingBase)_selectedObject;
			if (BuildingManager.TryRemoveBuilding(building))
				_selectedObjectTypeUI.HideContext();
		}

		public void OnBuildingLevelUp()
		{
			BuildingBase building = (BuildingBase)_selectedObject;
			if (!BuildingManager.CanLevelBuilding(building))
				_selectedObjectTypeUI.SelectionButtonTwo.interactable = false;
			else if (building.LevelHandler.Level == building.LevelHandler.MaxLevel)
			{
				_selectedObjectTypeUI.SelectionButtonTextTwo.text = "Max Level";
				_selectedObjectTypeUI.SelectionButtonTwo.interactable = false;
			}
		}

		public void DetachCurrentEvents() { DetachEvents(); }
		public override void UpdateDisplay()
		{
			BuildingBase buildingBase = (BuildingBase)_selectedObject;
			if (buildingBase.HealthHandler != null)
				UpdateBuildingHealth(buildingBase.HealthHandler);
			UpdateBuildingLevel(buildingBase.LevelHandler);
		}

		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.Title.gameObject.SetActive(true);
			_selectedObjectTypeUI.Description.gameObject.SetActive(true);
			_selectedObjectTypeUI.RedSliderContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.ID.gameObject.SetActive(true);
			_selectedObjectTypeUI.ButtonHolderTwo.gameObject.SetActive(true);
		}

		private void SetBuildingName()
		{
			_selectedObjectTypeUI.Title.text = (((BuildingBase)_selectedObject).BuildingType.ToString()).ToUpper();
		}

		public void UpdateBuildingHealth(HealthHandler health)
		{
			_selectedObjectTypeUI.RedSliderValue.text = health.Health + " / " + health.MaxHealth;
			if (health.Health != 0)
				_selectedObjectTypeUI.RedSlider.value = health.HealthPercentage;
			else
				_selectedObjectTypeUI.RedSlider.value = 0;
		}

		public void UpdateBuildingLevel(LevelHandler level)
		{
			if (level != null)
				_selectedObjectTypeUI.Description.text = "Lvl. " + level.Level + " / Lvl. " + level.MaxLevel;
			else
				_selectedObjectTypeUI.Description.text = "";

		}

		public void UpdateMarketPlaceRate(PassiveResourceIncrementer incrementor)
		{
			_selectedObjectTypeUI.Role.text = incrementor.GetInformation();
		}
	}
}