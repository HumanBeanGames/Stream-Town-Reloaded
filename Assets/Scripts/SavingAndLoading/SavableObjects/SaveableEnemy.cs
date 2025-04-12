using Enemies;
using GUIDSystem;

using SavingAndLoading.Structs;
using Target;
using Utils;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects 
{
    public class SaveableEnemy : SaveableObject
	{
        public Enemy Enemy { get; set; }

		public override object SaveData()
		{
			return (object)new EnemySaveData(Enemy.transform, Enemy.EnemyType.ToString(), Enemy.HealthHandler.Health, GameManager.Instance.GUIDManager.CreateGUIDandAddToDictionary(PoolableObject));
		}

		public override void LoadData(object data)
		{
			EnemySaveData enemyData = (EnemySaveData)data;
			Enemy.transform.position = Vector3SaveData.ToUnityVec3(enemyData.Transform.Position);
			Enemy.transform.eulerAngles = Vector3SaveData.ToUnityVec3(enemyData.Transform.Rotation);
			Enemy.transform.localScale = Vector3SaveData.ToUnityVec3(enemyData.Transform.LossyScale);
			Enemy.gameObject.SetActive(true);
			Enemy.HealthHandler.SetHealth(enemyData.Health);

			GUIDComponent.SetGUID(enemyData.GUID);
			GameManager.Instance.GUIDManager.AddToDictionary(PoolableObject);
		}

		public void SetVariables(Targetable target, GUIDComponent component, string poolName, PoolableObject poolableObject, Enemy enemy)
		{
			Enemy = enemy;
			base.SetVariables(target, component, poolName, poolableObject);
		}
	}
}