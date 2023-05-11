using System.Collections.Generic;

namespace GameEventSystem
{
	public class SortGameEventStartTime : IComparer<GameEvent>
	{
		public int Compare(GameEvent x, GameEvent y)
		{
			if (x.StartTime > y.StartTime)
				return 1;
			if (x.StartTime < y.StartTime)
				return -1;
			return 0;
		}
	}
}