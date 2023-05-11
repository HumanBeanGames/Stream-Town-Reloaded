using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TechTree.Elements
{
	/// <summary>
	/// Holds group data for a tree in the technology tree.
	/// </summary>
	public class TechnologyTreeGroup : Group
	{
		public string ID { get; set; }
		public string OldTitle;
		private Color _defaultBorderColor;
		private float _defaultBorderWidth;

		public TechnologyTreeGroup(string groupTitle, Vector2 position)
		{
			ID = Guid.NewGuid().ToString();
			title = groupTitle;
			OldTitle = groupTitle;
			SetPosition(new Rect(position, Vector2.zero));

			_defaultBorderColor = contentContainer.style.borderBottomColor.value;
			_defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
		}

		public void SetErrorStyle(Color color)
		{
			contentContainer.style.borderBottomColor = color;
			contentContainer.style.borderBottomWidth = 2f;
		}

		public void ResetStyle()
		{
			contentContainer.style.borderBottomColor = _defaultBorderColor;
			contentContainer.style.borderBottomWidth = _defaultBorderWidth;
		}
	}
}