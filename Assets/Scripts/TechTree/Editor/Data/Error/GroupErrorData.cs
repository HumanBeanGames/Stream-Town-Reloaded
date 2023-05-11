using System.Collections.Generic;
using TechTree.Elements;

namespace TechTree.Data.Error
{
	/// <summary>
	/// Used for displaying an error on groups.
	/// </summary>
	public class GroupErrorData
	{
		public ErrorData ErrorData { get; set; }

		public List<TechnologyTreeGroup> Groups { get; set; }

		public GroupErrorData()
		{
			ErrorData = new ErrorData();
			Groups = new List<TechnologyTreeGroup>();
		}
	}
}