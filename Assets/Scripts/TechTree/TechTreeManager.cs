using GameEventSystem;
using GameEventSystem.Events.Voting;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTree.Data;
using TechTree.ScriptableObjects;
using TownGoal.Data;
using Twitch;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using UserInterface;
using System.Collections;
using Sirenix.OdinInspector;
using TownGoal;

namespace Managers
{
	[GameManager]
	public static class TechTreeManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static TechTreeConfig Config = TechTreeConfig.Instance;

		private static TechTree_SO _techTreeSO => Config.techTreeSO;

		[HideInInspector]
        private static int TECH_COUNT_REQ_AGE_2 = 50;

        /// <summary>
        /// Minimum time after a technology was unlocked can the auto-vote start.
        /// </summary>
		/// 
		[HideInInspector]
        private static int _minTimeBetweenVotes;

        [HideInInspector]
        private static int _timeSinceLastUnlock;


        [HideInInspector]
        private static TechnologyTree _techTree;

        [HideInInspector]
        private static Dictionary<Goal, Node_SO> _goalsFollowed;

        [HideInInspector]
        private static int _techsUnlocked = 0;

        [HideInInspector]
        private static UserInterface_TownGoal _townGoalInterface;

        public static int MinTimeBetweenVotes => _minTimeBetweenVotes;
		public static TechnologyTree TechTree => _techTree;

		[HideInInspector]
		public static Node_SO CurrentTech = null;

        [HideInInspector]
        public static Action<Resource> OnStorageBoostUnlocked;

        [HideInInspector]
        public static Action<PlayerRole, StatType> OnStatBoostUnlocked;

        [HideInInspector]
        public static Action<BuildingType> OnBuildingUnlocked;

        [HideInInspector]
        public static Action<BuildingType> OnBuildingLevelIncreased;

        [HideInInspector]
        public static Action<BuildingType> OnBuildingCostReduction;

        [HideInInspector]
        public static Action<BuildingType> OnBuildingAgedUp;

		public static void InitializeTree()
		{
			_techTree = new TechnologyTree(_techTreeSO);
			_goalsFollowed = new Dictionary<Goal, Node_SO>();

			//PrintAvailableNodes();

		}
        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            GameObject runnerObject = new GameObject("TimeManagerRunner");
            runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
        }

        public static void StartCoroutine(IEnumerator routine) => runner.StartCoroutine(routine);

		public static void PrintAvailableNodes()
		{
			var availableNodes = _techTree.AvailableNodes;

			for (int i = 0; i < availableNodes.Count; i++)
				Debug.Log(availableNodes[i].TechName);
		}

		public static Node_SO[] GetRandomAvailableTechs(int count = 3)
		{
			List<Node_SO> nodes = new List<Node_SO>(_techTree.AvailableNodes.Count);

			for (int i = 0; i < _techTree.AvailableNodes.Count; i++)
			{
				if (_techTree.AvailableNodes[i].Unavailable)
					continue;

				bool canAdd = false;
				if (BuildingManager.BuildingAges[BuildingType.Townhall] != Age.Age2)
					if (_techTree.AvailableNodes[i].Age == Age.Age2)
						for (int j = 0; j < _techTree.AvailableNodes[i].Unlocks.Count; j++)
						{
							if (_techTree.AvailableNodes[i].Unlocks[j].BuildingType == BuildingType.Townhall && _techTree.AvailableNodes[i].Unlocks[j].TechType == TechType.AgeUpBuilding && _techsUnlocked >= TECH_COUNT_REQ_AGE_2)
								canAdd = true;
						}
					else
						canAdd = true;
				else
					canAdd = true;
				if (canAdd)
					nodes.Add(_techTree.AvailableNodes[i]);
			}

			Node_SO[] randomNodes = new Node_SO[count];

			for (int i = 0; i < count; i++)
			{
				randomNodes[i] = GetRandomTechFromList(nodes);
			}

			return randomNodes;
		}

		private static void GoalCompleted(Goal goal)
		{

			if (_goalsFollowed.ContainsKey(goal))
			{
				_techTree.UnlockNode(_goalsFollowed[goal]);
				_goalsFollowed.Remove(goal);
				goal.OnGoalCompleted -= GoalCompleted;
			}

			CurrentTech = null;
			StartNewTechVote();
		}

		public static void StartNewRandomTech()
		{
			var nodes = GetRandomAvailableTechs();

			if (nodes[0] == null)
				return;

			string text = "Options: ";

			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodes[i] != null)
					text += $"{nodes[i].TechName} |";
			}

			StartGoalFromNode(nodes[0]);
		}

		public static void StartNewTechVote(float delay = 0)
		{
			var nodes = GetRandomAvailableTechs();

			if (nodes.Length == 0)
				return;

			if (nodes[0] == null)
				return;

			TechVote voteEvent = new TechVote(delay, 60, nodes);
			voteEvent.EventEnded += OnTechVoteEnded;
			GameEventManager.AddEvent(voteEvent);
		}

		private static void OnTechVoteEnded(bool success, GameEvent.EventType type, object data)
		{
			if (data == null)
				return;

			StartGoalFromNode(((VoteOption)data).OptionData as Node_SO);
		}

		private static Node_SO GetRandomTechFromList(List<Node_SO> values)
		{
			if (values.Count == 0)
				return null;

			int rand = UnityEngine.Random.Range(0, values.Count);
			Node_SO node = values[rand];

			values.Remove(node);

			return node;
		}

		public static Goal StartGoalFromNode(Node_SO node)
		{
			Goal goal = new Goal(node.Objectives);
			TownGoalManager.StartNewGoal(goal);
			goal.OnGoalCompleted += GoalCompleted;
			_goalsFollowed.Add(goal, node);
			 if(_townGoalInterface == null)
				_townGoalInterface = GameManager.Instance.UIManager.TownGoalInterface;

			_townGoalInterface.AddGoal(goal, node);

			CurrentTech = node;
			return goal;
		}

		public static void SetCurrentTech(Node_SO node)
		{
			CurrentTech = node;
			// do stuff to make sure it works here
		}

		public static void UnlockAllTech()
		{
			var availableTechs = GetRandomAvailableTechs();
			do
			{
				_techTree.UnlockNode(availableTechs[0]);
				availableTechs = GetRandomAvailableTechs();
			} while (availableTechs.Length > 0 && availableTechs[0] != null);
		}

		public static void UnlockToAge2Tech()
		{
			var availableTechs = GetRandomAvailableTechs();
			do
			{
				_techTree.UnlockNode(availableTechs[0]);
				availableTechs = GetRandomAvailableTechs();
			} while (availableTechs.Length > 0 && availableTechs[0] != null && availableTechs[0].Age == Age.Age1);
		}

		public static void UnlockTech(Node_SO techNode)
		{
			OnTechUnlocked(techNode);
		}

		public static void OnTechUnlocked(Node_SO techNode)
		{
			_techsUnlocked++;
			for (int i = 0; i < techNode.Unlocks.Count; i++)
			{
				var data = techNode.Unlocks[i];
				switch (data.TechType)
				{
					case TechType.StatBoost:
						StatBoostUnlocked(data);
						break;
					case TechType.UnlockBuilding:
						UnlockBuilding(data);
						break;
					case TechType.BuildingCostReduction:
						BuildingCostReduction(data);
						break;
					case TechType.StorageBoost:
						StorageBoostUpgrade(data);
						break;
					case TechType.UpgradeBuilding:
						BuildingMaxLevelIncreased(data);
						break;
					case TechType.AgeUpBuilding:
						AgeBuilding(data);
						break;
				}
			}
		}

		/// <summary>
		/// Called when the node unlock data boosts a stat.
		/// </summary>
		/// <param name="data"></param>
		private static void StatBoostUnlocked(NodeUnlockData data)
		{
			StatType statType = data.StatType;

			PlayerRole playerRole = data.PlayerRole;

			if (playerRole == PlayerRole.Count)
			{
				PlayerManager.GlobalStatModifiers.AddToModifier(statType, data.IntValue);
			}
			else
				PlayerManager.GetStatModifiers(playerRole).AddToModifier(statType, data.IntValue);

			OnStatBoostUnlocked?.Invoke(playerRole, statType);
		}

		/// <summary>
		/// Called when a node unlock unlocks a building.
		/// </summary>
		/// <param name="data"></param>
		private static void UnlockBuilding(NodeUnlockData data)
		{
			BuildingManager.UnlockBuilding(data.BuildingType);

			OnBuildingUnlocked?.Invoke(data.BuildingType);
		}

		private static void BuildingCostReduction(NodeUnlockData data)
		{
			BuildingType buildingType = data.BuildingType;

			if (buildingType == BuildingType.Count)
				BuildingManager.GlobalBuildCostModifier += data.IntValue;
			else
				BuildingManager.BuildingCostModifiers[buildingType] += data.IntValue;

			OnBuildingCostReduction?.Invoke(buildingType);
		}

		/// <summary>
		/// Triggered when a node Unlock boosts the storage.
		/// </summary>
		/// <param name="data"></param>
		private static void StorageBoostUpgrade(NodeUnlockData data)
		{
			Resource resourceType = data.ResourceType;

			if (TownResourceManager.ResourceBoostValues.ContainsKey(resourceType))
				TownResourceManager.ResourceBoostValues[resourceType] += data.IntValue;

			OnStorageBoostUnlocked?.Invoke(resourceType);
		}

		/// <summary>
		/// Called when a node unlock upgrades a building.
		/// </summary>
		/// <param name="data"></param>
		private static void BuildingMaxLevelIncreased(NodeUnlockData data)
		{
			BuildingManager.BuildingsMaxLevel[data.BuildingType] = data.IntValue;

			OnBuildingLevelIncreased?.Invoke(data.BuildingType);
		}

		/// <summary>
		/// Called when a node unlock Ages up a building.
		/// </summary>
		/// <param name="data"></param>
		private static void AgeBuilding(NodeUnlockData data)
		{
			if ((int)BuildingManager.BuildingAges[data.BuildingType] < 1)
				BuildingManager.BuildingAges[data.BuildingType]++;

			Age age = (Age)data.IntValue;
			OnBuildingAgedUp?.Invoke(data.BuildingType);
		}

		private static void Start()
		{
			//StartNewRandomTech();
		}

		public static IEnumerator DelayedSetup()
		{
			yield return new WaitForSeconds(0.01f);
			_townGoalInterface = GameManager.Instance.UIManager.TownGoalInterface;
			if (GameManager.Instance.MetaDatas.LoadType == MetaData.LoadType.Generate)
				StartNewTechVote(20);
		}
	}
}