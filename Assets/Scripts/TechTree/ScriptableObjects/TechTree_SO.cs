using DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace TechTree.ScriptableObjects
{
	public class TechTree_SO : ScriptableObject
	{
		[field: SerializeField]
		public string FileName { get; set; }
		[field: SerializeField]
		public SerializableDictionary<NodeGroup_SO, List<Node_SO>> NodeGroups { get; set; }
		[field: SerializeField]
		public List<Node_SO> UngroupedNodes { get; set; }

		public void Initialize(string fileName)
		{
			FileName = fileName;

			NodeGroups = new SerializableDictionary<NodeGroup_SO, List<Node_SO>>();
			UngroupedNodes = new List<Node_SO>();
		}
	}
}