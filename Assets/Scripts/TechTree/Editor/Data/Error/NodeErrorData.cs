using System.Collections.Generic;
using TechTree.Elements;

namespace TechTree.Data.Error 
{
	/// <summary>
	/// Used for displaying an error color on nodes.
	/// </summary>
    public class NodeErrorData 
	{
        public ErrorData ErrorData { get; set; }
		public List<TechTreeNode> Nodes { get; set; }

		public NodeErrorData()
		{
			ErrorData = new ErrorData();
			Nodes = new List<TechTreeNode>();
		}
    }
}