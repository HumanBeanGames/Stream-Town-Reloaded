using DataStructures;
using System;
using System.Collections.Generic;
using TechTree.Elements;
using TechTree.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using TechTree.Data.Error;
using Utils;
using TechTree.Data.Save;

namespace TechTree.Windows
{
	public class TechTreeGraphView : GraphView
	{
		private TechTreeEditorWindow _editorWindow;
		private TechTreeSearchWindow _searchWindow;
		private SerializableDictionary<string, NodeErrorData> _ungroupedNodes;
		private SerializableDictionary<string, GroupErrorData> _groups;
		private SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>> _groupedNodes;
		private int _nameErrorCount;
		private MiniMap _miniMap;

		public int NameErrorCount
		{
			get => _nameErrorCount;
			set
			{
				_nameErrorCount = value;

				if (_nameErrorCount == 0)
				{
					_editorWindow.EnableSaving();
				}
				else if (_nameErrorCount != 0)
				{
					_editorWindow.DisableSaving();
				}
			}
		}

		public TechTreeGraphView(TechTreeEditorWindow editorwindow)
		{
			_editorWindow = editorwindow;

			_ungroupedNodes = new SerializableDictionary<string, NodeErrorData>();
			_groups = new SerializableDictionary<string, GroupErrorData>();
			_groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>>();

			AddManipulators();
			AddSearchWindow();
			AddMiniMap();
			AddGridBackground();

			// Callback overrides
			OnElementsDeleted();
			OnGroupElementsAdded();
			OnGroupElementsRemoved();
			OnGroupRenamed();
			OnGraphViewChanged();

			AddStyles();
			AddMiniMapStyles();
		}

		public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
		{
			if (isSearchWindow)
				mousePosition -= _editorWindow.position.position;

			return contentViewContainer.WorldToLocal(mousePosition);
		}

		public TechTreeNode CreateNode(string nodeName, Vector2 position, bool shouldDraw = true)
		{
			TechTreeNode node = new TechTreeNode();
			node.Initialize(nodeName, this, position);

			if (shouldDraw)
				node.Draw();

			AddUngroupedNode(node);

			return node;
		}

		public void AddUngroupedNode(TechTreeNode node)
		{
			string nodeName = node.TechName.ToLower();

			if (!_ungroupedNodes.ContainsKey(nodeName))
			{
				NodeErrorData nodeErrorData = new NodeErrorData();
				nodeErrorData.Nodes.Add(node);

				_ungroupedNodes.Add(nodeName, nodeErrorData);

				return;
			}

			List<TechTreeNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;
			ungroupedNodesList.Add(node);

			Color errorColor = _ungroupedNodes[nodeName].ErrorData.Color;

			node.SetErrorStyle(errorColor);

			if (ungroupedNodesList.Count == 2)
			{
				++NameErrorCount;

				ungroupedNodesList[0].SetErrorStyle(errorColor);
			}
		}

		public void RemoveUngroupedNode(TechTreeNode node)
		{
			string nodeName = node.TechName.ToLower();

			List<TechTreeNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

			ungroupedNodesList.Remove(node);

			node.ResetStyle();

			if (ungroupedNodesList.Count == 1)
			{
				--NameErrorCount;

				ungroupedNodesList[0].ResetStyle();

				return;
			}

			if (ungroupedNodesList.Count == 0)
				_ungroupedNodes.Remove(nodeName);
		}

		public void AddGroupedNode(TechTreeNode node, TechnologyTreeGroup group)
		{
			string nodeName = node.TechName.ToLower();

			node.Group = group;
			// If group doesn't already exist
			if (!_groupedNodes.ContainsKey(group))
				_groupedNodes.Add(group, new SerializableDictionary<string, NodeErrorData>());

			// If name doesnt already exist
			if (!_groupedNodes[group].ContainsKey(nodeName))
			{
				NodeErrorData nodeErrorData = new NodeErrorData();

				nodeErrorData.Nodes.Add(node);

				_groupedNodes[group].Add(nodeName, nodeErrorData);

				return;
			}

			List<TechTreeNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
			// If name already exists
			groupedNodesList.Add(node);

			Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;

			node.SetErrorStyle(errorColor);

			if (groupedNodesList.Count == 2)
			{
				++NameErrorCount;
				groupedNodesList[0].SetErrorStyle(errorColor);
			}
		}

		public void RemoveGroupedNode(TechTreeNode node, Group group)
		{
			string nodeName = node.TechName.ToLower();

			node.Group = null;

			List<TechTreeNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

			groupedNodesList.Remove(node);

			node.ResetStyle();

			if (groupedNodesList.Count == 1)
			{
				--NameErrorCount;
				groupedNodesList[0].ResetStyle();
				return;
			}

			if (groupedNodesList.Count == 0)
			{
				_groupedNodes[group].Remove(nodeName);

				// If the group has 0 elements, remove 
				if (_groupedNodes[group].Count == 0)
					_groupedNodes.Remove(group);
			}
		}

		public TechnologyTreeGroup CreateGroup(string title, Vector2 localMousePosition)
		{
			TechnologyTreeGroup group = new TechnologyTreeGroup(title, localMousePosition);

			AddGroup(group);

			AddElement(group);

			foreach (GraphElement selectedElement in selection)
			{
				if (!(selectedElement is TechTreeNode))
					continue;

				TechTreeNode node = (TechTreeNode)selectedElement;

				group.AddElement(node);
			}

			return group;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> compatiblePorts = new List<Port>();

			ports.ForEach(port =>
			{
				if (startPort == port)
					return;
				if (startPort.node == port.node)
					return;
				if (startPort.direction == port.direction)
					return;

				compatiblePorts.Add(port);
			});

			return compatiblePorts;
		}

		private void AddManipulators()
		{
			SetupZoom(ContentZoomer.DefaultMinScale * 0.5f, ContentZoomer.DefaultMaxScale);
			this.AddManipulator(CreateNodeContextualMenu());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new RectangleSelector());

			this.AddManipulator(CreateGroupContextualMenu());
		}

		private IManipulator CreateGroupContextualMenu()
		{
			ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
				menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("TechGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
				);
			return contextualMenuManipulator;
		}

		private IManipulator CreateNodeContextualMenu()
		{
			ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
				menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode("NewTechNode", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
				);
			return contextualMenuManipulator;
		}

		private void AddGridBackground()
		{
			GridBackground gridBackground = new GridBackground();

			gridBackground.StretchToParentSize();

			Insert(0, gridBackground);
		}

		private void AddStyles()
		{
			this.AddStyleSheets(
				"TechnologyTreeSystem/TechnologyTreeGraphViewStyles.uss",
				"TechnologyTreeSystem/TechnologyTreeNodeStyles.uss"
				);
		}

		private void AddMiniMapStyles()
		{
			StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
			StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

			_miniMap.style.backgroundColor = backgroundColor;
			_miniMap.style.borderBottomColor = borderColor;
			_miniMap.style.borderTopColor = borderColor;
			_miniMap.style.borderLeftColor = borderColor;
			_miniMap.style.borderRightColor = borderColor;
			_miniMap.style.color = new StyleColor(new Color32(99, 82, 34, 255));
		}

		private void AddSearchWindow()
		{
			if (_searchWindow == null)
			{
				_searchWindow = ScriptableObject.CreateInstance<TechTreeSearchWindow>();
				_searchWindow.Initialize(this);
			}

			nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
		}

		private void AddMiniMap()
		{
			_miniMap = new MiniMap();

			_miniMap.SetPosition(new Rect(15, 50, 200, 180));

			Add(_miniMap);

			_miniMap.visible = false;
		}

		public void ToggleMiniMap()
		{
			_miniMap.visible = !_miniMap.visible;
		}

		private void AddGroup(TechnologyTreeGroup group)
		{
			string groupName = group.title.ToLower();

			if (!_groups.ContainsKey(groupName))
			{
				GroupErrorData groupErrorData = new GroupErrorData();

				groupErrorData.Groups.Add(group);

				_groups.Add(groupName, groupErrorData);

				return;
			}

			List<TechnologyTreeGroup> groupsList = _groups[groupName].Groups;

			groupsList.Add(group);

			Color errorColor = _groups[groupName].ErrorData.Color;

			group.SetErrorStyle(errorColor);

			if (groupsList.Count == 2)
			{
				++NameErrorCount;
				groupsList[0].SetErrorStyle(errorColor);
			}
		}

		private void RemoveGroup(TechnologyTreeGroup group)
		{
			string oldGroupName = group.OldTitle.ToLower();

			List<TechnologyTreeGroup> groupsList = _groups[oldGroupName].Groups;

			groupsList.Remove(group);

			group.ResetStyle();

			if (groupsList.Count == 1)
			{
				--NameErrorCount;
				groupsList[0].ResetStyle();
				return;
			}

			if (groupsList.Count == 0)
				_groups.Remove(oldGroupName);
		}

		private void OnElementsDeleted()
		{
			deleteSelection = (operationName, askUser) =>
			{
				Type groupType = typeof(TechnologyTreeGroup);
				Type edgeType = typeof(Edge);
				List<Edge> edgesToDelete = new List<Edge>();
				for (int i = selection.Count - 1; i >= 0; i--)
				{
					if (i > selection.Count - 1)
						continue;

					if (selection[i] is TechTreeNode)
					{
						TechTreeNode node = selection[i] as TechTreeNode;

						if (node.Group != null)
							node.Group.RemoveElement(node);

						RemoveUngroupedNode(node);
						node.DisconnectAllPorts();
						RemoveElement(node);

					}
					else if (selection[i].GetType() == groupType)
					{
						TechnologyTreeGroup group = selection[i] as TechnologyTreeGroup;
						List<TechTreeNode> groupNodes = new List<TechTreeNode>();

						foreach (GraphElement groupElement in group.containedElements)
						{
							if (!(groupElement is TechTreeNode))
								continue;

							groupNodes.Add((TechTreeNode)groupElement);
						}

						group.RemoveElements(groupNodes);

						RemoveGroup(group);
						RemoveElement(group);
					}
					else if (selection[i].GetType() == edgeType)
					{
						edgesToDelete.Add((Edge)selection[i]);
					}
				}

				DeleteElements(edgesToDelete);
			};
		}

		private void OnGroupElementsAdded()
		{
			elementsAddedToGroup = (group, elements) =>
			{
				foreach (GraphElement element in elements)
				{
					if (!(element is TechTreeNode))
						continue;

					TechnologyTreeGroup nodeGroup = (TechnologyTreeGroup)group;
					TechTreeNode node = (TechTreeNode)element;

					RemoveUngroupedNode(node);
					AddGroupedNode(node, nodeGroup);
				}
			};
		}

		private void OnGroupElementsRemoved()
		{
			elementsRemovedFromGroup = (group, elements) =>
			{
				foreach (GraphElement element in elements)
				{
					if (!(element is TechTreeNode))
						continue;

					TechTreeNode node = (TechTreeNode)element;

					RemoveGroupedNode(node, group);
					AddUngroupedNode(node);
				}
			};
		}

		private void OnGroupRenamed()
		{
			groupTitleChanged = (group, newTitle) =>
			{
				TechnologyTreeGroup technologyTreeGroup = (TechnologyTreeGroup)group;

				technologyTreeGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

				if (string.IsNullOrEmpty(technologyTreeGroup.title))
				{
					if (!string.IsNullOrEmpty(technologyTreeGroup.OldTitle))
					{
						++NameErrorCount;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(technologyTreeGroup.OldTitle))
						--NameErrorCount;
				}


				RemoveGroup(technologyTreeGroup);

				technologyTreeGroup.OldTitle = technologyTreeGroup.title;

				AddGroup(technologyTreeGroup);
			};
		}

		private void OnGraphViewChanged()
		{
			graphViewChanged = (changes) =>
			 {
				 if (changes.edgesToCreate != null)
				 {
					 foreach (Edge edge in changes.edgesToCreate)
					 {
						 TechTreeNode nextNode = (TechTreeNode)edge.input.node;
						 ChildrenSaveData saveData = (ChildrenSaveData)edge.output.userData;
						 saveData.NodeID = nextNode.ID;
					 }
				 }
				 if (changes.elementsToRemove != null)
				 {
					 Type edgeType = typeof(Edge);

					 foreach (GraphElement element in changes.elementsToRemove)
					 {
						 if (element.GetType() != edgeType)
							 continue;

						 Edge edge = (Edge)element;
						 ChildrenSaveData saveData = (ChildrenSaveData)edge.output.userData;

						 saveData.NodeID = "";
					 }
				 }

				 return changes;
			 };
		}

		public void ClearGraph()
		{
			graphElements.ForEach(graphElement => RemoveElement(graphElement));

			_groups.Clear();
			_groupedNodes.Clear();
			_ungroupedNodes.Clear();

			NameErrorCount = 0;
		}
	}
}