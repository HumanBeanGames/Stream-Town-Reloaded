
using SavingAndLoading.Structs;
using System;
using TownGoal.Enumerations;
using UnityEngine;
using Utils;

namespace TownGoal.Data
{
	public class Objective
	{
		public Action<Objective> ObjectiveComplete;
		public Action<Objective, int> AmountChanged;

		private ObjectiveType _objectiveType;

		private int _requiredAmount;
		private int _amount;
		private ObjectiveData _data;

		public int Amount => _amount;
		public int RequiredAmount => _requiredAmount;
		public ObjectiveType ObjectiveType => _objectiveType;
		public ObjectiveData Data => _data;

		public Objective(ObjectiveData data)
		{
			_data = data;
			_objectiveType = data.ObjectiveType;
			_requiredAmount = (int)(data.IntValue);
			ObjectiveComplete += ClearListeners;
			_amount = 0;

			BuildObjective();
		}

		public ObjectiveSaveData ToObjectiveSaveData()
		{
			return new ObjectiveSaveData(_requiredAmount, _amount);
		}

		public void SetValues(int amount, int requiredAmount)
		{
			_amount = amount;
			_requiredAmount = requiredAmount;
			OnAmountChanged();
		}
		
		private void BuildObjective()
		{
			switch (_objectiveType)
			{
				case ObjectiveType.Build:
					EventManager.BuildingBuilt += OnBuildingBuilt;
					break;
				case ObjectiveType.BuildAny:
					EventManager.BuildingBuilt += OnBuildingBuilt;
					break;
				case ObjectiveType.Collect:
					EventManager.ResourceGained += OnResourceGained;
					break;
				case ObjectiveType.Kill:
					EventManager.EnemyKilled += OnEnemyKilled;
					break;
				case ObjectiveType.KillAny:
					EventManager.EnemyKilled += OnEnemyKilled;
					break;
				case ObjectiveType.EarnPerHour:
					EventManager.ResourceGained += OnResourceGained;
					break;
				case ObjectiveType.Sell:
					EventManager.ResourceSold += OnResourceSold;
					break;
				case ObjectiveType.SellAny:
					EventManager.ResourceSold += OnResourceSold;
					break;
				case ObjectiveType.Buy:
					EventManager.ResourceBought += OnResourceBought;
					break;
				case ObjectiveType.BuyAny:
					EventManager.ResourceBought += OnResourceBought;
					break;
				default:
					break;
			}
		}

		private void ClearListeners(Objective obj)
		{
			switch (_objectiveType)
			{
				case ObjectiveType.Build:
					EventManager.BuildingBuilt -= OnBuildingBuilt;
					break;
				case ObjectiveType.BuildAny:
					EventManager.BuildingBuilt -= OnBuildingBuilt;
					break;
				case ObjectiveType.Collect:
					EventManager.ResourceGained -= OnResourceGained;
					break;
				case ObjectiveType.Kill:
					EventManager.EnemyKilled -= OnEnemyKilled;
					break;
				case ObjectiveType.KillAny:
					EventManager.EnemyKilled -= OnEnemyKilled;
					break;
				case ObjectiveType.EarnPerHour:
					EventManager.ResourceGained -= OnResourceGained;
					break;
				case ObjectiveType.Sell:
					EventManager.ResourceSold -= OnResourceSold;
					break;
				case ObjectiveType.SellAny:
					EventManager.ResourceSold -= OnResourceSold;
					break;
				case ObjectiveType.Buy:
					EventManager.ResourceBought -= OnResourceBought;
					break;
				case ObjectiveType.BuyAny:
					EventManager.ResourceBought -= OnResourceBought;
					break;
				default:
					break;
			}

			ObjectiveComplete -= ClearListeners;
		}

		private void OnEnemyKilled(EnemyType enemyType)
		{
			if (_objectiveType == ObjectiveType.KillAny)
			{
				_amount++;
				OnAmountChanged();
			}
			else if (_data.EnemyType == enemyType)
			{
				_amount++;
				OnAmountChanged();
			}
		}

		private void OnResourceGained(Resource resource, int amount)
		{
			if (_objectiveType == ObjectiveType.Collect && _data.ResourceType == resource)
			{
				_amount += amount;
				OnAmountChanged();
			}
			else if (_objectiveType == ObjectiveType.EarnPerHour && _data.ResourceType == resource)
			{
				_amount = _requiredAmount;
				OnAmountChanged();
			}
		}

		private void OnBuildingBuilt(BuildingType type)
		{
			if (_objectiveType == ObjectiveType.BuildAny)
			{
				_amount++;
				OnAmountChanged();
			}
			else if (type == _data.BuildingType)
			{
				_amount++;
				OnAmountChanged();
			}
		}

		public void CompleteObjective()
		{
			_amount = _requiredAmount;
			ObjectiveComplete?.Invoke(this);
			Debug.Log($"Objective Complete '{_objectiveType}'");
		}

		private void OnAmountChanged()
		{
			AmountChanged?.Invoke(this, _amount);
			if (_amount >= _requiredAmount)
			{
				CompleteObjective();
			}
		}

		private void OnResourceBought(Resource resourceType, int amount)
		{
			if (_objectiveType == ObjectiveType.BuyAny || _data.ResourceType == resourceType)
			{
				_amount += amount;
				OnAmountChanged();
			}
		}

		private void OnResourceSold(Resource resourceType, int amount)
		{
			if (_objectiveType == ObjectiveType.SellAny || _data.ResourceType == resourceType)
			{
				_amount += amount;
				OnAmountChanged();
			}
		}
	}
}