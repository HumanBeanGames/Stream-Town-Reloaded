using GUIDSystem;
using Managers;
using SavingAndLoading.Structs;
using Target;
using Units;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects 
{
    public class SaveableEnemyCamp : SaveableObject
	{
		public HealthHandler HealthHandler;

		public override object SaveData()
		{
			return (object)new EnemyCampSaveData(HealthHandler.transform, HealthHandler.Health, GUIDComponent.GUID);
		}

		public override void LoadData(object data)
		{
			EnemyCampSaveData enemyCampData = (EnemyCampSaveData)data;
			HealthHandler.transform.position = Vector3SaveData.ToUnityVec3(enemyCampData.Transform.Position);
			HealthHandler.transform.eulerAngles = Vector3SaveData.ToUnityVec3(enemyCampData.Transform.Rotation);
			HealthHandler.transform.localScale = Vector3SaveData.ToUnityVec3(enemyCampData.Transform.LossyScale);
			HealthHandler.gameObject.SetActive(true);
			HealthHandler.SetHealth(enemyCampData.Health);

			GUIDComponent.SetGUID(enemyCampData.GUID);
			GUIDManager.AddToDictionary(PoolableObject);
		}

		public void SetVariables(Targetable target, GUIDComponent component, string poolName, PoolableObject poolableObject, HealthHandler healthHandler)
		{
			HealthHandler = healthHandler;
			base.SetVariables(target, component, poolName, poolableObject);
		}

	}
}