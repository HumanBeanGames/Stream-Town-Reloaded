using GridSystem.Utils;
using System.Collections.Generic;
using UnityEngine;
// System may be obsolete.
namespace GridSystem
{
	//TODO:: Remove this
	[System.Serializable]
	public struct GridNode
	{
		public CollisionType CollisionType;
		public List<GridNode> Connections;
		public Vector2 Position;
		public int CellIndex;

		public GridNode(CollisionType collision, Vector2 position, int cellIndex)
		{
			CollisionType = collision;
			Connections = new List<GridNode>();
			Position = position;
			CellIndex = cellIndex;
		}

		public void AddConnection(GridNode node)
		{
			Connections.Add(node);
		}

		public void RemoveConnection(GridNode node)
		{
			Connections.Remove(node);
		}
	}
}