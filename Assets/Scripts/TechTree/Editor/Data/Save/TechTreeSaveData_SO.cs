using UnityEngine;
using System.Collections.Generic;
using DataStructures;

namespace TechTree.Data.Save
{
	/// <summary>
	/// Holds the save data for technology tree
	/// </summary>
	public class TechTreeSaveData_SO : ScriptableObject
	{
		[field: SerializeField]
		public string FileName { get; set; }
		[field: SerializeField]
		public List<GroupSaveData> Groups { get; set; }
		[field: SerializeField]
		public List<NodeSaveData> Nodes { get; set; }
		[field: SerializeField]
		public List<string> OldGroupNames { get; set; }
		[field: SerializeField]
		public List<string> OldUngroupedNodeNames { get; set; }
		[field: SerializeField]
		public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

		public void Initialize(string fileName)
		{
			FileName = fileName;

			Groups = new List<GroupSaveData>();
			Nodes = new List<NodeSaveData>();
		}
	}
}