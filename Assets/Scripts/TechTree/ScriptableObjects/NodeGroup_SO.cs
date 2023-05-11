using UnityEngine;

namespace TechTree.ScriptableObjects
{
	public class NodeGroup_SO : ScriptableObject
	{
		[field: SerializeField]
		public string GroupName { get; set; }

		public void Initialize(string groupName)
		{
			GroupName = groupName;
		}
	}
}