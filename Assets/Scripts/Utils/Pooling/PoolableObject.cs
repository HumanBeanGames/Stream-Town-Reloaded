using Buildings;
using Character;
using Enemies;
using GameResources;
using GUIDSystem;
using Managers;
using SavingAndLoading.SavableObjects;
using Target;
using Units;
using UnityEngine;

namespace Utils.Pooling
{
	public enum PoolType
	{
		Other,
		Resource,
		Enemy,
		Player,
		Building,
		Foliage,
		Count
	}

	public class PoolableObject : MonoBehaviour
	{
		[SerializeField]
		private string _poolName;
		private object _saveableObject;
		[SerializeField]
		private PoolType _poolType;

		public PoolType PoolType => _poolType;
		public object SaveableObject
		{
			get { return _saveableObject; }
			set { _saveableObject = value; }
		}
		public string PoolName
		{
			get { return _poolName; }
			set { _poolName = value; }
		}

		public void Initialize(string name)
		{
			_poolName = name;
			SetupSaveableObject();
		}

		public void SetupSaveableObject()
		{
			switch (_poolType)
			{
				case PoolType.Other:
					break;
				case PoolType.Resource:
					SaveableObject = (object)new SaveableResource();
					((SaveableResource)SaveableObject).SetVariables(gameObject.GetComponent<Targetable>(), gameObject.GetComponent<GUIDComponent>(), _poolName, this, GetComponent<ResourceHolder>());
					break;
				case PoolType.Enemy:
					SaveableObject = (object)new SaveableEnemy();
					((SaveableEnemy)SaveableObject).SetVariables(gameObject.GetComponent<Targetable>(), gameObject.GetComponent<GUIDComponent>(), _poolName, this, GetComponent<Enemy>());
					break;
				case PoolType.Player:
					SaveableObject = (object)new SaveablePlayer();
					((SaveablePlayer)SaveableObject).SetVariables(gameObject.GetComponent<Targetable>(), gameObject.GetComponent<GUIDComponent>(), _poolName, this, GetComponent<RoleHandler>());
					break;
				case PoolType.Building:
					SaveableObject = (object)new SaveableBuilding();
					((SaveableBuilding)SaveableObject).SetVariables(gameObject.GetComponent<Targetable>(), gameObject.GetComponent<GUIDComponent>(), _poolName, this, GetComponent<BuildingBase>());
					break;				
				case PoolType.Foliage:
					SaveableObject = (object)new SaveablFoliage();
					((SaveablFoliage)SaveableObject).SetVariables(_poolName, this);
					break;
			}
		}

		private void OnEnable()
		{
			if (GameManager.Instance != null && _saveableObject != null && ((SaveableObject)_saveableObject).GUIDComponent != null)
				GUIDManager.CreateGUIDandAddToDictionary(this);
		}

		private void OnDisable()
		{
			if (GameManager.Instance != null && _saveableObject != null && ((SaveableObject)_saveableObject).GUIDComponent !=null)
				GUIDManager.RemoveFromGUID(PoolType, ((SaveableObject)_saveableObject).GUIDComponent.GUID);

			ObjectPoolingManager.AddToPool(_poolName, this);
		}
	}
}