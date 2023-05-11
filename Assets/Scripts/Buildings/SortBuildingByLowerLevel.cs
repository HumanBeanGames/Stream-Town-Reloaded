using System.Collections.Generic;

namespace Buildings
{
	public class SortBuildingByLowerLevel : IComparer<BuildingBase>
	{
		public int Compare(BuildingBase x, BuildingBase y)
		{
			if (x.LevelHandler.Level > y.LevelHandler.Level)
				return 1;
			if (x.LevelHandler.Level < y.LevelHandler.Level)
				return -1;
			return 0;
		}
	}
}