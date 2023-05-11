using UnityEngine;

namespace Utils 
{
    public class Easings 
	{
		public static float EaseInOutCubic(float x)
		{
			return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
		}

		public static float EaseInCubic(float x)
		{
			return x * x * x;
		}

		public static float EaseOutCubic(float x)
		{
			return 1 - Mathf.Pow(1 - x, 3);
		}

		public static float EaseInOutSine(float x)
		{
			return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
		}

		public static float EaseInSine(float x)
		{
			return 1 - Mathf.Cos((x * Mathf.PI) / 2);
		}

		public static float EaseOutSine(float x)
		{
			return Mathf.Sin((x * Mathf.PI) / 2);
		}
	}
}