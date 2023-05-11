using UnityEngine;
using Utils;

namespace SavingAndLoading.Structs
{
	[System.Serializable]
	public struct EnemySaveData
	{
		public TransformSaveData Transform;
		public string EnemyType;
		public int Health;
		public uint GUID;
		public uint TargetGUID;
		public uint CampGUID;

		public EnemySaveData(Transform transform, string enemyType, int health, uint gUID)
		{
			Transform = new TransformSaveData(transform);
			EnemyType = enemyType;
			Health = health;
			GUID = gUID;
			TargetGUID = 0;
			CampGUID = 0;
		}
		public void SetTargetGUID(uint targetGUID)
		{
			TargetGUID = targetGUID;
		}

		public void SetCampGUID(uint campGUID)
		{
			CampGUID = campGUID;
		}
	}
}