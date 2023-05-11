using GUIDSystem;
using SavingAndLoading.Structs;
using Target;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects 
{
    public class SaveablFoliage : SaveableObject
	{
		public override object SaveData()
		{
			return (object)new FoliageSaveData(PoolableObject.transform, PoolName);
		}

		public override void LoadData(object data)
		{
			FoliageSaveData foliageData = (FoliageSaveData)data;
			PoolableObject.transform.position = Vector3SaveData.ToUnityVec3(foliageData.FoliageTransform.Position);
			PoolableObject.transform.eulerAngles = Vector3SaveData.ToUnityVec3(foliageData.FoliageTransform.Rotation);
			PoolableObject.transform.localScale = Vector3SaveData.ToUnityVec3(foliageData.FoliageTransform.LossyScale);
			PoolableObject.gameObject.SetActive(true);
		}

		public void SetVariables(string poolName, PoolableObject poolableObject)
		{
			PoolName = poolName;
			PoolableObject = poolableObject;
		}
	}
}