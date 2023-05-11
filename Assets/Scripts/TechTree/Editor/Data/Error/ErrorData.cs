using UnityEngine;

namespace TechTree.Data.Error
{
	/// <summary>
	/// Used for displaying Error Colors.
	/// </summary>
	public class ErrorData
	{
		public Color Color { get; set; }

		public ErrorData()
		{
			GenerateRandomColor();
		}

		private void GenerateRandomColor()
		{
			Color = new Color32(
				(byte)Random.Range(180, 256),
				(byte)Random.Range(50, 176),
				(byte)Random.Range(50, 176),
				255
				);
		}
	}
}