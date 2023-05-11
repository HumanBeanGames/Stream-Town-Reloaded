using UnityEngine;

namespace TechTree.Data
{
	using ScriptableObjects;
	using System;

	/// <summary>
	/// Holds a node's child data.
	/// </summary>
	[Serializable]
	public class NodeChildrenTechData
	{
		[field: SerializeField]
		public string NodeID { get; set; }
		[field: SerializeField]
		public Node_SO NextTech { get; set; }
	}
}