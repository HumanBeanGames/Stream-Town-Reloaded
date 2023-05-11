using System.Collections.Generic;
using TechTree.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TechTree.Windows
{
	public class TechTreeSearchWindow : ScriptableObject, ISearchWindowProvider
	{
		private TechTreeGraphView graphView;
		private Texture2D indentationIcon;

		public void Initialize(TechTreeGraphView techTreeGraphView)
		{
			graphView = techTreeGraphView;

			indentationIcon = new Texture2D(1, 1);
			indentationIcon.SetPixel(0, 0, Color.clear);
			indentationIcon.Apply();
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			List<SearchTreeEntry> searchTreeEntires = new List<SearchTreeEntry>()
			{
				new SearchTreeGroupEntry(new GUIContent("Create Element")),
				new SearchTreeEntry(new GUIContent("Create Node",indentationIcon))
				{
					level = 1,
					userData = new TechTreeNode()
				},
				new SearchTreeEntry(new GUIContent("Create Group",indentationIcon))
				{
					level = 1,
					userData = new Group()
				}
			};

			return searchTreeEntires;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
			switch (SearchTreeEntry.userData)
			{
				case TechTreeNode _:
					{
						TechTreeNode node = graphView.CreateNode("NewTechNode",localMousePosition);
						graphView.AddElement(node);
						return true;
					}

				case TechnologyTreeGroup _:
					{
						graphView.CreateGroup("Tech Group", localMousePosition);
						return true;
					}
				default:
					return false;
			}
		}
	}
}