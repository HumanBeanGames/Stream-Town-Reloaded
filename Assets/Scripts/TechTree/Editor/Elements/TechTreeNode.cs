using TownGoal.Data.Save;
using System;
using System.Collections.Generic;
using TechTree.Data.Save;
using TechTree.Utilities;
using TechTree.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using UnityEditor;

namespace TechTree.Elements
{
	/// <summary>
	/// Holds all of the data for a tech tree node and handles all of the drawing on the editor.
	/// </summary>
	public class TechTreeNode : Node
	{
		public string ID { get; set; }
		public string TechName { get; set; }
		public string NodeTitle { get; set; }
		public List<NodeUnlockSaveData> Unlocks { get; set; }
		public bool Unlocked { get; set; }
		public List<ChildrenSaveData> ChildTech { get; set; }
		public List<ObjectiveSaveData> Objectives { get; set; }
		public Sprite Icon { get; set; }
		public string IconPath { get; set; }

		public string Description { get; set; }
		public Age Age { get; set; } = Age.Age1;
		public int Tier { get; set; } = 1;
		public bool Unavailable { get; set; }
		public bool UnlocksFoldoutCollapsed { get; set; } = true;
		public bool ObjectivesFoldoutCollapsed { get; set; } = true;
		public bool DescriptionFoldoutCollapsed { get; set; } = true;
		public TechnologyTreeGroup Group { get; set; }

		private TechTreeGraphView _graphview;
		private Color _defaultBackgroundColor;
		private VisualElement _unlocksElement;
		private Foldout _unlocksFoldout;
		private Foldout _objectivesFoldout;

		public VisualElement UnlocksElement => _unlocksElement;
		public Foldout UnlocksFoldout => _unlocksFoldout;
		public Foldout ObjectivesFoldout => _objectivesFoldout;

		/// <summary>
		/// Initializes the node and draws its data.
		/// </summary>
		/// <param name="nodeName"></param>
		/// <param name="graphview"></param>
		/// <param name="position"></param>
		public void Initialize(string nodeName, TechTreeGraphView graphview, Vector2 position)
		{
			ID = Guid.NewGuid().ToString();

			TechName = nodeName;
			Unlocks = new List<NodeUnlockSaveData>();
			ChildTech = new List<ChildrenSaveData>();
			Objectives = new List<ObjectiveSaveData>();
			Description = "Tech Text.";
			_graphview = graphview;
			_defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
			NodeTitle = nodeName;

			SetPosition(new Rect(position, Vector2.zero));

			titleContainer.AddClasses("techtree-node__title-container");
			mainContainer.AddClasses("techtree-node__main-container");
			extensionContainer.AddClasses("techtree-node__extension_container");
			ChildTech.Add(new ChildrenSaveData());
		}

		/// <summary>
		/// Builds contextual menus for right clicking a node in editor.
		/// </summary>
		/// <param name="evt"></param>
		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Disconnect Inputs", actionEvent => DisconnectInputPorts());
			evt.menu.AppendAction("Disconnect Outputs", actionEvent => DisconnectOutputPorts());
			base.BuildContextualMenu(evt);
		}

		/// <summary>
		/// Draws all of the Node's information to the graph view.
		/// </summary>
		public void Draw()
		{
			// TITLE CONTAINER
			TextField techTreeNameTextField = TechTreeUtilities.CreateTextField(TechName, null, callback =>
			 {
				 TextField target = (TextField)callback.target;

				 target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

				 if (string.IsNullOrEmpty(target.value))
				 {
					 if (!string.IsNullOrEmpty(TechName))
					 {
						 ++_graphview.NameErrorCount;
					 }
				 }
				 else
				 {
					 if (string.IsNullOrEmpty(TechName))
						 --_graphview.NameErrorCount;
				 }

				 if (Group == null)
				 {
					 _graphview.RemoveUngroupedNode(this);

					 TechName = target.value;

					 _graphview.AddUngroupedNode(this);
					 return;
				 }
				 else
				 {
					 TechnologyTreeGroup currentGroup = Group;

					 _graphview.RemoveGroupedNode(this, Group);

					 TechName = target.value;

					 _graphview.AddGroupedNode(this, currentGroup);
				 }
			 });

			techTreeNameTextField.AddClasses(
				"techtree-node__text-field",
				"techtree-node__filename-text-field__hidden",
				"techtree-node__text-field__hidden"
				);

			titleContainer.Insert(0, techTreeNameTextField);

			// INPUT CONTAINER
			Port inputPort = this.CreatePort(direction: Direction.Input, capacity: Port.Capacity.Single);
			inputContainer.Add(inputPort);

			// MAIN CONTAINER
			Button addChildButton = TechTreeUtilities.CreateButton("Add Child", () =>
			{
				ChildrenSaveData saveData = new ChildrenSaveData();
				ChildTech.Add(saveData);

				Port childPort = CreateChildPort(saveData);

				outputContainer.Add(childPort);
			});

			addChildButton.AddClasses("techtree-node__button");
			mainContainer.Insert(1, addChildButton);

			// OUTPUT CONTAINER
			foreach (ChildrenSaveData data in ChildTech)
			{
				Port childPort = CreateChildPort(data);
				outputContainer.Add(childPort);
			}

			// EXTENSIONS CONTAINER

			// Change this to be a simple textfield, no need for foldout
			VisualElement customDataContainer = new VisualElement();
			customDataContainer.AddClasses("techtree-node__custom-data-container");

			TextField title = TechTreeUtilities.CreateTextField(NodeTitle, "Title: ", callback =>
			 {
				 NodeTitle = callback.newValue;
			 });
			customDataContainer.Add(title);
			ObjectField iconField = new ObjectField()
			{
				objectType = typeof(Sprite),
				label = "Icon",
				value = Icon
			};

			iconField.RegisterValueChangedCallback((evt) =>
			{
				Icon = (Sprite)evt.newValue;
				IconPath = AssetDatabase.GetAssetPath(Icon);
			});

			customDataContainer.Add(iconField);
			EnumField AgeEnum = TechTreeUtilities.CreateEnumField(Age, "Age: ", callback =>
			  {
				  Age = (Age)callback.newValue;
			  });
			customDataContainer.Add(AgeEnum);

			IntegerField tierIntField = TechTreeUtilities.CreateIntField(Tier, "Tier: ", callback =>
			 {
				 Tier = callback.newValue;
			 });
			customDataContainer.Add(tierIntField);

			Toggle unlockedToggle = TechTreeUtilities.CreateToggle(Unlocked, "Unlocked", callback =>
			{
				Unlocked = callback.newValue;
			});

			customDataContainer.Add(unlockedToggle);
			Toggle availableToggle = TechTreeUtilities.CreateToggle(Unavailable, "Unavailable", callback =>
			{
				Unavailable = callback.newValue;
			});
			customDataContainer.Add(availableToggle);

			Foldout techFoldout = TechTreeUtilities.CreateFoldout("Description", !DescriptionFoldoutCollapsed, callback =>
			{
				DescriptionFoldoutCollapsed = callback.newValue;
			});

			TextField textTextField = TechTreeUtilities.CreateTextArea(Description, null, callback =>
			{
				Description = callback.newValue;
			});

			textTextField.AddClasses("techtree-node__text-field", "techtree-node__quote-text-field");

			techFoldout.Add(textTextField);
			customDataContainer.Add(techFoldout);
			extensionContainer.Add(customDataContainer);

			_unlocksElement = new VisualElement();
			_unlocksFoldout = TechTreeUtilities.CreateFoldout("Unlocks", !UnlocksFoldoutCollapsed, callback =>
			{
				UnlocksFoldoutCollapsed = callback.newValue;
			});

			List<(NodeUnlockSaveData, VisualElement)> elements = new List<(NodeUnlockSaveData, VisualElement)>();
			for (int i = Unlocks.Count - 1; i >= 0; i--)
			{
				elements.Add(CreateNewUnlockContainer(Unlocks[i], addToElement: false));
			}

			for (int i = elements.Count - 1; i >= 0; i--)
			{
				_unlocksFoldout.Insert(Unlocks.IndexOf(elements[i].Item1), elements[i].Item2);
			}
			_unlocksElement.Add(_unlocksFoldout);

			extensionContainer.Add(_unlocksElement);


			Button addUnlockButton = TechTreeUtilities.CreateButton("Add Unlock", () =>
			{
				CreateNewUnlockContainer(new NodeUnlockSaveData());
			});
			extensionContainer.Add(_unlocksFoldout);
			extensionContainer.Add(addUnlockButton);

			_objectivesFoldout = TechTreeUtilities.CreateFoldout("Objectives", !ObjectivesFoldoutCollapsed, callback =>
			{
				ObjectivesFoldoutCollapsed = callback.newValue;
			});
			extensionContainer.Add(_objectivesFoldout);


			List<(ObjectiveSaveData, VisualElement)> objectiveElements = new List<(ObjectiveSaveData, VisualElement)>();
			for (int i = Objectives.Count - 1; i >= 0; i--)
			{
				objectiveElements.Add(CreateNewObjectiveContainer(Objectives[i], addToElement: false));
			}

			for (int i = objectiveElements.Count - 1; i >= 0; i--)
			{
				_objectivesFoldout.Insert(Objectives.IndexOf(objectiveElements[i].Item1), objectiveElements[i].Item2);
			}

			Button addObjectiveButton = TechTreeUtilities.CreateButton("Add Objective", () =>
			{
				CreateNewObjectiveContainer(new ObjectiveSaveData());
			});

			extensionContainer.Add(addObjectiveButton);

			RefreshExpandedState();
		}

		/// <summary>
		/// Returns true if the node is unlocked.
		/// </summary>
		/// <returns></returns>
		public bool IsUnlocked()
		{
			return Unlocked;
		}

		/// <summary>
		/// Sets the error color of the node.
		/// </summary>
		/// <param name="color"></param>
		public void SetErrorStyle(Color color)
		{
			mainContainer.style.backgroundColor = color;
		}

		/// <summary>
		/// Resets the node style back to default.
		/// </summary>
		public void ResetStyle()
		{
			mainContainer.style.backgroundColor = _defaultBackgroundColor;
		}

		public void DisconnectAllPorts()
		{
			DisconnectInputPorts();
			DisconnectOutputPorts();
		}

		private void DisconnectInputPorts()
		{
			DisconnectPorts(inputContainer);
		}

		private void DisconnectOutputPorts()
		{
			DisconnectPorts(outputContainer);
		}

		private void DisconnectPorts(VisualElement container)
		{
			foreach (Port port in container.Children())
			{
				if (!port.connected)
					continue;

				_graphview.DeleteElements(port.connections);
			}
		}

		private Port CreateChildPort(object userData)
		{
			Port childPort = this.CreatePort();

			childPort.userData = userData;

			ChildrenSaveData childSaveData = (ChildrenSaveData)userData;

			Button deleteChildButton = TechTreeUtilities.CreateButton("X", () =>
			{
				if (ChildTech.Count == 1)
					return;

				if (childPort.connected)
					_graphview.DeleteElements(childPort.connections);

				ChildTech.Remove(childSaveData);

				_graphview.RemoveElement(childPort);
			});

			deleteChildButton.AddClasses("techtree-node__button");

			childPort.Add(deleteChildButton);
			return childPort;
		}

		private (NodeUnlockSaveData, VisualElement) CreateNewUnlockContainer(NodeUnlockSaveData unlockSaveData, VisualElement existingElement = null, bool addToElement = true)
		{
			if (existingElement != null)
				_unlocksFoldout.Remove(existingElement);
			else if (!Unlocks.Contains(unlockSaveData))
				Unlocks.Add(unlockSaveData);

			UnlockVisualElement unlockElement = new UnlockVisualElement();
			unlockElement.Init(unlockSaveData, this);

			if (addToElement)
				_unlocksFoldout.Insert(Unlocks.IndexOf(unlockSaveData), unlockElement);

			return (unlockSaveData, unlockElement);
		}

		private (ObjectiveSaveData, VisualElement) CreateNewObjectiveContainer(ObjectiveSaveData objectiveSaveData, VisualElement existingElement = null, bool addToElement = true)
		{
			if (existingElement != null)
				_objectivesFoldout.Remove(existingElement);
			else if (!Objectives.Contains(objectiveSaveData))
				Objectives.Add(objectiveSaveData);

			ObjectiveVisualElement objectiveElement = new ObjectiveVisualElement();
			objectiveElement.Init(objectiveSaveData, this);

			if (addToElement)
				_objectivesFoldout.Insert(Objectives.IndexOf(objectiveSaveData), objectiveElement);

			return (objectiveSaveData, objectiveElement);
		}
	}
}