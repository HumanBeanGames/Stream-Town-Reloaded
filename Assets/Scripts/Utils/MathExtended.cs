using System;
using UnityEngine;

namespace Utils
{
	/// <summary>
	/// Extends normal Math Functionality.
	/// </summary>
	public class MathExtended
	{
		/// <summary>
		/// Remaps a value from one range to another range and returns the value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="in1"></param>
		/// <param name="in2"></param>
		/// <param name="out1"></param>
		/// <param name="out2"></param>
		/// <returns></returns>
		public static float RemapValue(float value, float in1, float in2, float out1, float out2)
		{
			return out1 + (value - in1) * (out2 - out1) / (in2 - in1);
		}

		/// <summary>
		/// Snaps an object to a grid size
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="cellSize"></param>
		/// <returns></returns>
		public static Vector3 SnapPosition(Vector3 pos, float cellSize)
		{
			return new Vector3(Mathf.Floor(pos.x / cellSize) * cellSize, 0, Mathf.Floor(pos.z / cellSize) * cellSize);
		}

		/// <summary>
		/// Pass in a function to set the values of a 2D array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <param name="arrayValueSetter"></param>
		public static void Set2DArrayValues<T>(ref T[,] values, Func<int, int, T> arrayValueSetter)
		{
			if (values == null) return;

			int dim0 = values.GetLength(0);
			if (dim0 == 0) return;

			int dim1 = values.GetLength(1);
			if (dim1 == 0) return;

			for (int i = 0; i < dim0; i++)
			{
				for (int j = 0; j < dim1; j++)
				{
					values[i, j] = arrayValueSetter(i, j);
				}
			}
		}
	}
}