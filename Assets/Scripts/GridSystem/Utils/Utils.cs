using System;
using UnityEngine;

namespace GridSystem.Utils
{
	/// <summary>
	/// Dictates the type of collision
	/// </summary>
	[Serializable, Flags]
	public enum CollisionType
	{
		Walkable = 0,
		Unwalkable = 1,
		Water = 2,
		Friendly = 3
	}

	/// <summary>
	/// Holds all color data for different types of collisions.
	/// </summary>
	public static class CollisionColours
	{
		public static Color Walkable = Color.green;
		public static Color Unwalkable = Color.red;
		public static Color Water = Color.blue;
		public static Color Friendly = Color.yellow;
	}
}