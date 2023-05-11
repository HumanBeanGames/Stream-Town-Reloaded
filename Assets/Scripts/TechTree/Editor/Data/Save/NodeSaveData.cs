using TownGoal.Data.Save;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace TechTree.Data.Save
{
	/// <summary>
	/// Holds Save data for a tech tree node.
	/// </summary>
	[Serializable]
	public class NodeSaveData
	{
		[field: SerializeField]
		public string ID { get; set; }
		[field: SerializeField]
		public string Name { get; set; }
		[field: SerializeField]
		public string NodeTitle { get; set; }
		[field: SerializeField]
		public string Description { get; set; }
		[field: SerializeField]
		public List<ChildrenSaveData> ChildTech { get; set; }
		[field: SerializeField]
		public List<NodeUnlockSaveData> Unlocks { get; set; }
		[field: SerializeField]
		public List<ObjectiveSaveData> Objectives { get; set; }
		[field: SerializeField]
		public string GroupID { get; set; }
		[field: SerializeField]
		public Vector2 Position { get; set; }
		[field: SerializeField]
		public bool Unlocked { get; set; }
		[field: SerializeField]
		public Age Age { get; set; } = Age.Age1;
		[field: SerializeField]
		public int Tier { get; set; } = 1;
		[field: SerializeField]
		public bool UnlocksFoldoutCollapsed { get; set; }
		[field: SerializeField]
		public bool ObjectivesFoldoutCollapsed { get; set; }
		[field: SerializeField]
		public bool DescriptionFoldoutCollapsed { get; set; }
		[field: SerializeField]
		public string IconPath { get; set; }
		[field: SerializeField]
		public bool Unavailable { get; set; }
	}
}