using System;
using UnityEditor;
using System.Linq;

namespace TechTree.Utilities
{
	using Data.Save;
	using ScriptableObjects;
	using Elements;
	using System.Collections.Generic;
	using UnityEngine;
	using Windows;
	using TechTree.Data;
	using DataStructures;
	using UnityEditor.Experimental.GraphView;
	using TownGoal.Data;
	using TownGoal.Data.Save;

	/// <summary>
	/// Handles all of the saving and loading for the Tech Tree Editor.
	/// </summary>
	public static class TechTreeIOUtility
	{
		public const string RUNTIME_FOLDER_LOCATION = "Assets/Resources/TechTree";
		public const string EDITOR_GRAPHS_FOLDER = EDITOR_FOLDER_LOCATION + "/Graphs";
		public const string EDITOR_FOLDER_LOCATION = "Assets/Scripts/TechTree/Editor";

		private static TechTreeGraphView _graphview;
		private static string _graphFileName;
		private static string _containerFolderPath;

		private static List<TechnologyTreeGroup> _groups;
		private static List<TechTreeNode> _nodes;
		private static Dictionary<string, NodeGroup_SO> _createdTechGroups;
		private static Dictionary<string, Node_SO> _createdTechs;

		private static Dictionary<string, TechnologyTreeGroup> _loadedGroups;
		private static Dictionary<string, TechTreeNode> _loadedNodes;

		/// <summary>
		/// Initializes the saving and loading.
		/// </summary>
		/// <param name="graphView"></param>
		/// <param name="graphName"></param>
		public static void Initialize(TechTreeGraphView graphView, string graphName)
		{
			_graphview = graphView;
			_graphFileName = graphName;
			_containerFolderPath = $"{RUNTIME_FOLDER_LOCATION}/Technologies/{_graphFileName}";

			_groups = new List<TechnologyTreeGroup>();
			_nodes = new List<TechTreeNode>();

			_createdTechGroups = new Dictionary<string, NodeGroup_SO>();
			_createdTechs = new Dictionary<string, Node_SO>();

			_loadedGroups = new Dictionary<string, TechnologyTreeGroup>();
			_loadedNodes = new Dictionary<string, TechTreeNode>();
		}

		/// <summary>
		/// Saves the currently open graph.
		/// </summary>
		public static void Save()
		{
			CreateStaticFolders();

			GetElementsFromGraphView();

			TechTreeSaveData_SO graphData = CreateAsset<TechTreeSaveData_SO>(EDITOR_GRAPHS_FOLDER, $"{_graphFileName}Graph");

			graphData.Initialize(_graphFileName);

			TechTree_SO technologyContainer = CreateAsset<TechTree_SO>(_containerFolderPath, _graphFileName);

			technologyContainer.Initialize(_graphFileName);

			SaveGroups(graphData, technologyContainer);
			SaveNodes(graphData, technologyContainer);
			SaveAsset(graphData);

			SaveAsset(technologyContainer);
		}

		/// <summary>
		/// Saves all of the groups.
		/// </summary>
		/// <param name="graphData"></param>
		/// <param name="technologyContainer"></param>
		private static void SaveGroups(TechTreeSaveData_SO graphData, TechTree_SO technologyContainer)
		{
			List<string> groupNames = new List<string>();

			foreach (TechnologyTreeGroup group in _groups)
			{
				SaveGroupToGraph(group, graphData);
				SaveGroupToScriptableObject(group, technologyContainer);

				groupNames.Add(group.title);
			}

			UpdateOldGroups(groupNames, graphData);
		}

		/// <summary>
		/// Removes all of the old groups that no longer exist.
		/// </summary>
		/// <param name="currentGroupNames"></param>
		/// <param name="graphData"></param>
		private static void UpdateOldGroups(List<string> currentGroupNames, TechTreeSaveData_SO graphData)
		{
			if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
			{
				List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

				foreach (string groupToRemove in groupsToRemove)
				{
					RemoveFolder($"{_containerFolderPath}/Groups/{groupToRemove}");
				}
			}

			graphData.OldGroupNames = new List<string>(currentGroupNames);
		}

		/// <summary>
		/// Saves the data of each individual node.
		/// </summary>
		/// <param name="graphData"></param>
		/// <param name="technologyContainer"></param>
		private static void SaveNodes(TechTreeSaveData_SO graphData, TechTree_SO technologyContainer)
		{
			SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
			List<string> ungroupedNodeNames = new List<string>();
			
			foreach (TechTreeNode node in _nodes)
			{
				SaveNodeToGraph(node, graphData);
				SaveNodeToScriptableObject(node, technologyContainer);

				if (node.Group != null)
				{
					groupedNodeNames.AddItem(node.Group.title, node.TechName);
					continue;
				}

				ungroupedNodeNames.Add(node.TechName);
			}

			UpdateTechnologyChildrenConnections();
			UpdateOldGroupedNodes(groupedNodeNames, graphData);
			UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
		}

		/// <summary>
		/// Removes any Old nodes that no longer grouped.
		/// </summary>
		/// <param name="currentGroupedNodeNames"></param>
		/// <param name="graphData"></param>
		private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, TechTreeSaveData_SO graphData)
		{
			if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
			{
				foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
				{
					List<string> nodesToRemove = new List<string>();

					// If it exists, its folder wasn't removed.
					if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
					{
						nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
					}

					foreach (string nodeToRemove in nodesToRemove)
					{
						RemoveAsset($"{_containerFolderPath}/Groups/{oldGroupedNode.Key}/Technologies", nodeToRemove);
					}
				}
			}

			graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
		}

		/// <summary>
		/// Updates nodes that were ungrouped.
		/// </summary>
		/// <param name="currentUngroupedNodeNames"></param>
		/// <param name="graphData"></param>
		private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, TechTreeSaveData_SO graphData)
		{
			if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
			{
				List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

				foreach (string nodeToRemove in nodesToRemove)
				{
					RemoveAsset($"{_containerFolderPath}/Global/Techologies", nodeToRemove);
				}
			}

			graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
		}

		/// <summary>
		/// Updates the connections to children.
		/// </summary>
		private static void UpdateTechnologyChildrenConnections()
		{
			foreach (TechTreeNode node in _nodes)
			{
				Node_SO technology = _createdTechs[node.ID];

				for (int childIndex = 0; childIndex < node.ChildTech.Count; childIndex++)
				{
					ChildrenSaveData techChild = node.ChildTech[childIndex];

					if (string.IsNullOrEmpty(techChild.NodeID))
						continue;

					technology.Children[childIndex].NextTech = _createdTechs[techChild.NodeID];

					SaveAsset(technology);
				}
			}
		}

		/// <summary>
		/// Saves the group to the graph.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="graphData"></param>
		private static void SaveGroupToGraph(TechnologyTreeGroup group, TechTreeSaveData_SO graphData)
		{
			GroupSaveData groupData = new GroupSaveData()
			{
				ID = group.ID,
				Name = group.title,
				Position = group.GetPosition().position
			};

			graphData.Groups.Add(groupData);
		}

		/// <summary>
		/// Saves the node to the graph data.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="graphData"></param>
		private static void SaveNodeToGraph(TechTreeNode node, TechTreeSaveData_SO graphData)
		{
			List<ChildrenSaveData> children = CloneNodeChildren(node.ChildTech);
			List<NodeUnlockSaveData> unlocks = CloneUnlocks(node.Unlocks);

			NodeSaveData nodeData = new NodeSaveData()
			{
				ID = node.ID,
				Name = node.TechName,
				NodeTitle = node.NodeTitle,
				ChildTech = children,
				Description = node.Description,
				GroupID = node.Group?.ID,
				Position = node.GetPosition().position,
				Unlocks = unlocks,
				Age = node.Age,
				Tier = node.Tier,
				Unlocked = node.Unlocked,
				UnlocksFoldoutCollapsed = node.UnlocksFoldoutCollapsed,
				DescriptionFoldoutCollapsed = node.DescriptionFoldoutCollapsed,
				Objectives = node.Objectives,
				ObjectivesFoldoutCollapsed = node.ObjectivesFoldoutCollapsed,
				IconPath = node.IconPath,
				Unavailable = node.Unavailable
			};

			graphData.Nodes.Add(nodeData);
		}

		/// <summary>
		/// Saves a group to a scriptable object.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="technologyContainer"></param>
		private static void SaveGroupToScriptableObject(TechnologyTreeGroup group, TechTree_SO technologyContainer)
		{
			string groupName = group.title;

			CreateFolder($"{_containerFolderPath}/Groups", groupName);
			CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Technologies");

			NodeGroup_SO techGroup = CreateAsset<NodeGroup_SO>($"{_containerFolderPath}/Groups/{groupName}", groupName);

			techGroup.Initialize(groupName);

			_createdTechGroups.Add(group.ID, techGroup);

			technologyContainer.NodeGroups.Add(techGroup, new List<Node_SO>());

			SaveAsset(techGroup);
		}

		/// <summary>
		/// Saves a node out to a scriptable object.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="technologyContainer"></param>
		private static void SaveNodeToScriptableObject(TechTreeNode node, TechTree_SO technologyContainer)
		{
			Node_SO technology;

			if (node.Group != null)
			{
				technology = CreateAsset<Node_SO>($"{_containerFolderPath}/Groups/{node.Group.title}/Technologies", node.TechName);

				technologyContainer.NodeGroups.AddItem(_createdTechGroups[node.Group.ID], technology);
			}
			else
			{
				technology = CreateAsset<Node_SO>($"{_containerFolderPath}/Global/Technologies", node.TechName);

				technologyContainer.UngroupedNodes.Add(technology);
			}

			//TODO: UPDATE THE AUTO FALSE HERE.
			technology.Initialize(node.TechName,node.NodeTitle, node.Description, ConvertNodeChildrenToTechChildren(node.ChildTech), ConvertNodeUnlocksToTechUnlock(node.Unlocks), ConvertNodeObjectivesToTechObjectives(node.Objectives), node.Unlocked, node.Age, node.Tier, node.IconPath, node.Unavailable);

			_createdTechs.Add(node.ID, technology);

			SaveAsset(technology);
		}

		// LOADING
		/// <summary>
		/// Loads the selected Technology Tree Graph.
		/// </summary>
		public static void Load()
		{
			TechTreeSaveData_SO graphData = LoadAsset<TechTreeSaveData_SO>($"{EDITOR_FOLDER_LOCATION}/Graphs", _graphFileName);

			if (graphData == null)
			{
				EditorUtility.DisplayDialog(
					"Couldn't load file",
					"The file at the following path could not be found: \n\n"
					+ $"{EDITOR_FOLDER_LOCATION}/Graphs/{_graphFileName}\n\n"
					+ "Make sure you chose the right file and it's placed at the folder path mentioned above.",
					"Poggers");

				return;
			}

			TechTreeEditorWindow.UpdateFileName(graphData.FileName);

			LoadGroups(graphData.Groups);
			LoadNodes(graphData.Nodes);
			LoadNodesConnections();
		}

		/// <summary>
		/// Loads groups to the graph.
		/// </summary>
		/// <param name="groups"></param>
		private static void LoadGroups(List<GroupSaveData> groups)
		{
			foreach (GroupSaveData groupData in groups)
			{
				TechnologyTreeGroup group = _graphview.CreateGroup(groupData.Name, groupData.Position);

				group.ID = groupData.ID;

				_loadedGroups.Add(group.ID, group);
			}
		}

		/// <summary>
		/// Loads nodes to the graph.
		/// </summary>
		/// <param name="nodes"></param>
		private static void LoadNodes(List<NodeSaveData> nodes)
		{
			foreach (NodeSaveData nodeData in nodes)
			{
				List<ChildrenSaveData> children = CloneNodeChildren(nodeData.ChildTech);
				List<NodeUnlockSaveData> unlocks = CloneUnlocks(nodeData.Unlocks);
				List<ObjectiveSaveData> objectives = CloneObjectives(nodeData.Objectives);
				TechTreeNode node = _graphview.CreateNode(nodeData.Name, nodeData.Position, false);
				node.ID = nodeData.ID;
				node.ChildTech = children;
				node.TechName = nodeData.Name;
				node.NodeTitle = nodeData.NodeTitle;
				node.Unlocked = nodeData.Unlocked;
				node.Unlocks = unlocks;
				node.Description = nodeData.Description;
				node.Age = nodeData.Age;
				node.Tier = nodeData.Tier;
				node.Objectives = objectives;
				node.UnlocksFoldoutCollapsed = nodeData.UnlocksFoldoutCollapsed;
				node.ObjectivesFoldoutCollapsed = nodeData.ObjectivesFoldoutCollapsed;
				node.DescriptionFoldoutCollapsed = nodeData.DescriptionFoldoutCollapsed;
				node.IconPath = nodeData.IconPath;
				node.Icon = (Sprite)AssetDatabase.LoadAssetAtPath(node.IconPath, typeof(Sprite));
				node.Unavailable = nodeData.Unavailable;
				node.Draw();

				_graphview.AddElement(node);

				_loadedNodes.Add(node.ID, node);

				if (string.IsNullOrEmpty(nodeData.GroupID))
					continue;

				TechnologyTreeGroup group = _loadedGroups[nodeData.GroupID];

				node.Group = group;

				group.AddElement(node);
			}
		}

		private static void LoadNodesConnections()
		{
			foreach (KeyValuePair<string, TechTreeNode> loadedNode in _loadedNodes)
			{
				foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
				{
					ChildrenSaveData childData = (ChildrenSaveData)choicePort.userData;

					if (string.IsNullOrEmpty(childData.NodeID))
						continue;

					TechTreeNode nextNode = _loadedNodes[childData.NodeID];

					// CHANGE THIS TO NOT USE FIRST IF THERE ARE MULTIPLE INPUTS
					Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

					_graphview.AddElement(choicePort.ConnectTo(nextNodeInputPort));

					loadedNode.Value.RefreshPorts();
				}
			}
		}


		/// <summary>
		/// Converts children save data to tech tree data.
		/// </summary>
		/// <param name="childTech"></param>
		/// <returns></returns>
		private static List<NodeChildrenTechData> ConvertNodeChildrenToTechChildren(List<ChildrenSaveData> childTech)
		{
			List<NodeChildrenTechData> childTechData = new List<NodeChildrenTechData>();

			foreach (ChildrenSaveData child in childTech)
			{
				NodeChildrenTechData childData = new NodeChildrenTechData()
				{
					NodeID = child.NodeID
				};

				childTechData.Add(childData);
			}

			return childTechData;
		}

		/// <summary>
		/// Convers Node save data to tech tree data.
		/// </summary>
		/// <param name="saveUnlocks"></param>
		/// <returns></returns>
		private static List<NodeUnlockData> ConvertNodeUnlocksToTechUnlock(List<NodeUnlockSaveData> saveUnlocks)
		{
			List<NodeUnlockData> techUnlockData = new List<NodeUnlockData>();

			foreach (NodeUnlockSaveData unlock in saveUnlocks)
			{
				NodeUnlockData data = new NodeUnlockData()
				{
					BoolValue = unlock.BoolValue,
					BuildingType = unlock.BuildingType,
					CharValue = unlock.CharValue,
					FloatValue = unlock.FloatValue,
					IntValue = unlock.IntValue,
					ObjectValue = unlock.ObjectValue,
					PlayerRole = unlock.PlayerRole,
					ResourceType = unlock.ResourceType,
					StatType = unlock.StatType,
					StringValue = unlock.StringValue,
					TechType = unlock.TechType
				};

				techUnlockData.Add(data);
			}

			return techUnlockData;
		}

		/// <summary>
		/// Converts Objective Save data to Tech Tree data.
		/// </summary>
		/// <param name="saveData"></param>
		/// <returns></returns>
		private static List<ObjectiveData> ConvertNodeObjectivesToTechObjectives(List<ObjectiveSaveData> saveData)
		{
			List<ObjectiveData> objectives = new List<ObjectiveData>();

			foreach (ObjectiveSaveData save in saveData)
			{
				ObjectiveData data = new ObjectiveData()
				{
					BuildingType = save.BuildingType,
					FloatValue = save.FloatValue,
					IntValue = save.IntValue,
					ObjectiveType = save.ObjectiveType,
					ResourceType = save.ResourceType,
					EnemyType = save.EnemyType
				};

				objectives.Add(data);
			}
			return objectives;
		}

		// Creation Methods
		/// <summary>
		/// Creates the Static folders required for saving and loading.
		/// </summary>
		private static void CreateStaticFolders()
		{
			CreateFolder("Assets/Scripts", "TechTree");
			CreateFolder(EDITOR_FOLDER_LOCATION, "Graphs");
			CreateFolder(RUNTIME_FOLDER_LOCATION, "Technologies");

			CreateFolder($"{RUNTIME_FOLDER_LOCATION}/Technologies", _graphFileName);
			CreateFolder(_containerFolderPath, "Global");
			CreateFolder(_containerFolderPath, "Groups");
			CreateFolder($"{_containerFolderPath}/Global", "Technologies");
		}

		// Utility Methods'
		/// <summary>
		/// Takes a path and foldername to create a new folder.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="folderName"></param>
		private static void CreateFolder(string path, string folderName)
		{
			if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
				return;

			AssetDatabase.CreateFolder(path, folderName);
		}

		/// <summary>
		/// Creates a new scriptable object in the asset database.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
		{
			string fullPath = $"{path}/{assetName}.asset";
			T asset = LoadAsset<T>(path, assetName);

			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance<T>();

				AssetDatabase.CreateAsset(asset, fullPath);
			}
			return asset;
		}

		/// <summary>
		/// Loads a scriptable object from the asset database.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
		{
			string fullPath = $"{path}/{assetName}.asset";

			return AssetDatabase.LoadAssetAtPath<T>(fullPath);
		}

		/// <summary>
		/// Removes a scriptable object from the asset database.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="assetName"></param>
		private static void RemoveAsset(string path, string assetName)
		{
			AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
		}

		/// <summary>
		/// Saves an asset to the asset database and refreshes it.
		/// </summary>
		/// <param name="asset"></param>
		private static void SaveAsset(UnityEngine.Object asset)
		{
			EditorUtility.SetDirty(asset);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Removes a folder from the project.
		/// </summary>
		/// <param name="fullPath"></param>
		private static void RemoveFolder(string fullPath)
		{
			FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
			FileUtil.DeleteFileOrDirectory($"{fullPath}/");
		}

		/// <summary>
		/// Used to safely copy child data.
		/// </summary>
		/// <param name="nodeChildren"></param>
		/// <returns></returns>
		private static List<ChildrenSaveData> CloneNodeChildren(List<ChildrenSaveData> nodeChildren)
		{
			List<ChildrenSaveData> children = new List<ChildrenSaveData>();

			foreach (ChildrenSaveData child in nodeChildren)
			{
				ChildrenSaveData childData = new ChildrenSaveData()
				{
					NodeID = child.NodeID
				};

				children.Add(child);
			}

			return children;
		}

		/// <summary>
		/// Used to safely clone Unlock data.
		/// </summary>
		/// <param name="nodeUnlocks"></param>
		/// <returns></returns>
		private static List<NodeUnlockSaveData> CloneUnlocks(List<NodeUnlockSaveData> nodeUnlocks)
		{
			List<NodeUnlockSaveData> unlocks = new List<NodeUnlockSaveData>();

			foreach (NodeUnlockSaveData unlock in nodeUnlocks)
			{
				NodeUnlockSaveData unlockData = new NodeUnlockSaveData()
				{
					TechType = unlock.TechType,
					BoolValue = unlock.BoolValue,
					BuildingType = unlock.BuildingType,
					CharValue = unlock.CharValue,
					FloatValue = unlock.FloatValue,
					IntValue = unlock.IntValue,
					ObjectValue = unlock.ObjectValue,
					PlayerRole = unlock.PlayerRole,
					ResourceType = unlock.ResourceType,
					StatType = unlock.StatType,
					StringValue = unlock.StringValue
				};

				unlocks.Add(unlockData);
			}

			return unlocks;
		}

		/// <summary>
		/// Used to safely copy Objectives data.
		/// </summary>
		/// <param name="objectivesData"></param>
		/// <returns></returns>
		private static List<ObjectiveSaveData> CloneObjectives(List<ObjectiveSaveData> objectivesData)
		{
			List<ObjectiveSaveData> objectives = new List<ObjectiveSaveData>();

			foreach (ObjectiveSaveData objective in objectivesData)
			{
				ObjectiveSaveData objectiveData = new ObjectiveSaveData()
				{
					BuildingType = objective.BuildingType,
					FloatValue = objective.FloatValue,
					IntValue = objective.IntValue,
					ObjectiveType = objective.ObjectiveType,
					ResourceType = objective.ResourceType,
					EnemyType = objective.EnemyType
				};

				objectives.Add(objectiveData);
			}
			return objectives;
		}

		// Fetch Methods
		/// <summary>
		/// Gets all the different elements from the graph.
		/// </summary>
		private static void GetElementsFromGraphView()
		{
			Type groupType = typeof(TechnologyTreeGroup);

			_graphview.graphElements.ForEach(graphElement =>
			{
				if (graphElement is TechTreeNode node)
				{
					_nodes.Add(node);
					return;
				}

				if (graphElement.GetType() == groupType)
				{
					_groups.Add((TechnologyTreeGroup)graphElement);
					return;
				}
			});
		}
	}
}