using System.Collections.Generic;
using UnityEngine;
using TownGoal.Data;


namespace TechTree.ScriptableObjects
{
	using Data;
	using Utils;

	public class Node_SO : ScriptableObject
	{
		[field: SerializeField]
		public string TechName { get; set; }
		[field: SerializeField]
		public string NodeTitle { get; set; }
		[field: SerializeField, TextArea]
		public string Description { get; set; }
		[field: SerializeField]
		public List<NodeChildrenTechData> Children { get; set; }
		[field: SerializeField]
		public List<NodeUnlockData> Unlocks { get; set; }
		[field: SerializeField]
		public List<ObjectiveData> Objectives {get;set;}
		[field: SerializeField]
		public bool IsUnlocked { get; set; }
		[field: SerializeField]
		public Age Age { get; set; }
		[field: SerializeField]
		public int Tier { get; set; }
		[field: SerializeField]
		public Node_SO Parent { get; set; }
		[field: SerializeField]
		public string IconPath { get; set; }

		[field: SerializeField]
		public bool Unavailable { get; set; }
		public void Initialize(string techName,string nodeTitle, string text, List<NodeChildrenTechData> children, List<NodeUnlockData> unlocks, List<ObjectiveData> objectives,	bool isUnlocked, Age age, int tier, string iconPath, bool unavailable)
		{
			NodeTitle = nodeTitle;
			TechName = techName;
			Description = text;
			Children = children;
			Unlocks = unlocks;
			IsUnlocked = isUnlocked;
			Age = age;
			Tier = tier;
			Objectives = objectives;
			IconPath = iconPath;
			Unavailable = unavailable;
		}
	}
}