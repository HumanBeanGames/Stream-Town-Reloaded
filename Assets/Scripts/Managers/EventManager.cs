using System;
using Utils;
using Character;


namespace Managers
{
	public static class EventManager
	{
		public static Action<EnemyType> EnemyKilled;
		public static Action<BuildingType> BuildingBuilt;
		public static Action<Resource, int> ResourceGained;
		public static Action<Resource, int> ResourceSold;
		public static Action<Resource, int> ResourceBought;
		public static Action<Player> PlayerDied;
	}
}