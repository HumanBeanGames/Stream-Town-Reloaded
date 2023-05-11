using GameResources;
using UnityEngine;
using Utils;

namespace Scriptables
{
	/// <summary>
	/// Scriptable Object to hold Building Data
	/// </summary>
	[CreateAssetMenu(fileName = "Building Data", menuName = "ScriptableObjects/BuildingDataObject", order = 2)]
	public class BuildingDataScriptable : ScriptableObject
	{
		public string BuildingName;
		public Sprite BuildingSprite;
		public BuildingType BuildingType;
		public bool CanLevel = true;
		public bool Placeable = true;
		public bool Unlocked = false;
		public Age StartingAge;

		[Header("Base Cost To Build")]
		public ResourceCostData BuildResourceCost;
        public float CostIncreasePerBuildingMultiplier = 2;

		[Header("Base Cost To Level")]
		public ResourceCostData LevelResourceCost;
		public float CostIncreasePerLevelMultiplier = 2;
	}
}