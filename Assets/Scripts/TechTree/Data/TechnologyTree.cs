
using System;
using System.Collections.Generic;
using TechTree.ScriptableObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace TechTree.Data
{
	public class TechnologyTree
	{
		private TechTree_SO _tree;
		private Node_SO _rootNode;

		private List<Node_SO> _availableNodes;

		public Node_SO RootNode => _rootNode;
		public List<Node_SO> AvailableNodes => _availableNodes;
		public Action<Node_SO> TechUnlocked;

		public Dictionary<Node_SO, bool> _unlockedNodes;

		public TechnologyTree(TechTree_SO tree, TechTreeManager manager)
		{
			Profiler.BeginSample("Initialize Tech Tree");
			_tree = tree;
			_availableNodes = new List<Node_SO>();
			_unlockedNodes = new Dictionary<Node_SO, bool>();
			Profiler.EndSample();
			TechUnlocked += manager.OnTechUnlocked;
			InitializeData();
			Debug.Log($"Root Node Result: {_rootNode.TechName}");
		}

		public void UnlockNode(Node_SO node)
		{
			if (AvailableNodes.Contains(node))
			{
				_unlockedNodes[node] = true;
				AvailableNodes.Remove(node);

				RecursivelyAddAvailableNodes(node);
				TechUnlocked?.Invoke(node);
			}
		}

		public void ForceUnlockNode(Node_SO node)
		{
			if (AvailableNodes.Contains(node))
				AvailableNodes.Remove(node);

			//Debug.Log($"{node} unlocked.");
			_unlockedNodes[node] = true;

			RecursivelyAddAvailableNodes(node);
			TechUnlocked?.Invoke(node);
		}

		public bool IsUnlocked(Node_SO node)
		{
			return _unlockedNodes[node];
		}

		public List<bool> GetUnlockedNodes()
		{
			List<bool> result = new List<bool>();

			foreach (Node_SO node in _unlockedNodes.Keys)
			{
				result.Add(_unlockedNodes[node]);
			}

			return result;
		}

		public int GetCurrentNodesIndex()
		{
			int i = 0;

			foreach (Node_SO node in _unlockedNodes.Keys)
			{
				if (node == GameManager.Instance.TechTreeManager.CurrentTech)
					break;
				i++;
			}

			return i;
		}

		public Node_SO GetNodeFromName(string techName)
		{
			foreach (Node_SO node in _unlockedNodes.Keys)
			{
				if (techName == node.TechName)
					return node;
			}

			return null;
		}

		public void SetUnlockedNodes(List<bool> unlockedNodes)
		{
			List<Node_SO> nodesToBeProcessed = new List<Node_SO>();
			int i = 0;
			foreach (Node_SO node in _unlockedNodes.Keys)
			{
				node.IsUnlocked = unlockedNodes[i];
				if (node.IsUnlocked && !nodesToBeProcessed.Contains(node))
					nodesToBeProcessed.Add(node);
				i++;
			}

			for (int j = nodesToBeProcessed.Count - 1; j >= 0; j--)
			{
				ForceUnlockNode(nodesToBeProcessed[j]);
			}
		}

		private void InitializeData()
		{
			List<Node_SO> allNodes = new List<Node_SO>(_tree.UngroupedNodes);

			foreach (List<Node_SO> group in _tree.NodeGroups.Values)
				allNodes.AddRange(group);

			foreach (Node_SO node in allNodes)
				_unlockedNodes.Add(node, node.IsUnlocked);

			//TODO: This should probably be done in the editor tool...
			ConnectParents(ref allNodes);
			SetRootNode(ref allNodes);
			if (GameManager.Instance.MetaDatas.LoadType == MetaData.LoadType.Generate)
			{
				ForceUnlockNode(_rootNode);
				RecursivelyAddAvailableNodes(_rootNode);
			}
		}

		private void ConnectParents(ref List<Node_SO> allNodes)
		{
			for (int i = 0; i < allNodes.Count; i++)
			{
				if (allNodes[i].Children != null)
					for (int j = 0; j < allNodes[i].Children.Count; j++)
					{
						if (allNodes[i].Children[j].NextTech != null)
							allNodes[i].Children[j].NextTech.Parent = allNodes[i];
					}
			}
		}

		/// <summary>
		/// Attempts to find the root node.
		/// </summary>
		private void SetRootNode(ref List<Node_SO> allNodes)
		{
			for (int i = 0; i < allNodes.Count; i++)
			{
				if (allNodes[i].Parent == null && allNodes[i].Children.Count > 0 && !allNodes[i].Unavailable)
				{
					_rootNode = allNodes[i];
					return;
				}
			}
		}

		private void RecursivelyAddAvailableNodes(Node_SO node)
		{
			if (!IsUnlocked(node) && !_availableNodes.Contains(node) && !node.Unavailable)
			{
				Debug.Log($"{node} added to available list.");
				_availableNodes.Add(node);
			}

			for (int i = 0; i < node.Children.Count; i++)
			{
				if (node.Children[i].NextTech != null)
				{
					var child = node.Children[i].NextTech;

					if (IsUnlocked(child))
					{
						ForceUnlockNode(child);
					}
					else if (!_availableNodes.Contains(child))
						_availableNodes.Add(child);
				}
			}
		}
	}
}