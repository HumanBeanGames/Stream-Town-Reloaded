using UnityEngine;
using Utils;

namespace Scriptables
{
	[CreateAssetMenu(fileName = "AllBuildingDataScriptableData", menuName = "ScriptableObjects/AllBuildingDataScriptable", order = 1)]
	public class AllBuildingDataScriptable : ScriptableObject
	{
		public BuildingDataScriptable[] BuildingData;

		public BuildingDataScriptable GetDataByBuildingType(BuildingType building)
		{
			for (int i = 0; i < BuildingData.Length; i++)
			{
				if (BuildingData[i].BuildingType == building)
					return BuildingData[i];
			}

			Debug.LogError($"Attempted to get building data that didnt exist in All Buildings Data: {building}");
			return null;
		}
	}
}