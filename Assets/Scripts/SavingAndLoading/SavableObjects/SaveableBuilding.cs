using Buildings;
using GUIDSystem;
using Managers;
using SavingAndLoading.Structs;
using Target;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects
{
	public class SaveableBuilding : SaveableObject
	{
		public BuildingBase BuildingBase;

		public override object SaveData()
		{
			return (object)new BuildingSaveData(BuildingBase.transform, PoolName, BuildingBase.HealthHandler.Health, GUIDComponent.GUID, BuildingBase.BuildingState, BuildingBase.GetRemovedFoliageData());
		}

		public override void LoadData(object data)
		{
			BuildingSaveData buildingData = (BuildingSaveData)data;
			BuildingBase.HealthHandler.transform.position = Vector3SaveData.ToUnityVec3(buildingData.BuildingTranform.Position);
			BuildingBase.HealthHandler.transform.eulerAngles = Vector3SaveData.ToUnityVec3(buildingData.BuildingTranform.Rotation);
			BuildingBase.HealthHandler.transform.localScale = Vector3SaveData.ToUnityVec3(buildingData.BuildingTranform.LossyScale);
			BuildingBase.HealthHandler.gameObject.SetActive(true);
			BuildingBase.HealthHandler.SetHealth(buildingData.BuildingHealth);
			BuildingBase.BuildingState = buildingData.BuildingState;
			BuildingBase.SetRemovedFoliage(buildingData.DestroyedFoliage);
			GUIDComponent.SetGUID(buildingData.GUID);
			GameManager.Instance.GUIDManager.AddToDictionary(PoolableObject);
			GameManager.Instance.BuildingManager.AddLoadedBuilding(BuildingBase);

			if (BuildingBase.BuildingState == Utils.BuildingState.Building)
				BuildingBase.OnLoadedBuiltBuilding();

			if (BuildingBase.DamageHandler != null)
				BuildingBase.DamageHandler.OnHealthChanged(BuildingBase.HealthHandler);

		}

		public void SetVariables(Targetable target, GUIDComponent component, string poolName, PoolableObject poolableObject, BuildingBase buildingBase)
		{
			BuildingBase = buildingBase;
			base.SetVariables(target, component, poolName, poolableObject);
		}
	}
}