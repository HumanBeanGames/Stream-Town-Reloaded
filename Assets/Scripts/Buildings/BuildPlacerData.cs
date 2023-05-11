using UnityEngine;
using Utils;

namespace Buildings
{
	/// <summary>
	/// Stores data used by the building placer to determine certain attributes of a building.
	/// </summary>
	[System.Serializable]
	public class BuildPlacerData
	{
		public string BuildingName;
		public BuildingType BuildingType;
		public GameObject BuildingModel;
		public Vector2 BuildingSize;
		public GameObject Prefab; //TODO:: Use pooling to spawn
		public Renderer Renderer;
		public PlacementProbeHandler ProbeManager;
	}
}