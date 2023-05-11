using System;
using UnityEngine;

namespace TechTree.Data.Save 
{
	/// <summary>
	/// Holds Save Data for node's children.
	/// </summary>
	[Serializable]
    public class ChildrenSaveData
	{
		[field: SerializeField]
        public string NodeID { get; set; }
    }
}