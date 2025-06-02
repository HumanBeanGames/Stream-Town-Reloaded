using Buildings;
using Character;
using GameEventSystem;
using GridSystem.Partitioning;
using TownGoal;
using System.Collections.Generic;
using TechTree;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using World;
using World.Generation;
using SavingAndLoading;
using GUIDSystem;
using Enemies;
using PlayerControls;
using UnityEngine.EventSystems;
using UserInterface.MainMenu;
using System.Collections;
using Environment;
using Scriptables;
using Utils.Pooling;
using Twitch;
using Audio;

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }
		public static string[] GM_IDS = new[] { "43134305", "47817756", "51998688", "652607201", "159586407", "489520238", "56878491", "406879525" };

		public BuildingPlacer _buildingPlacer;

		[SerializeField]
		private Transform _playerSpawnPosition = null;
		[SerializeField]
		private EnemySpawner _enemySpawner;
		[SerializeField]
		private SettingsScriptable _settingsData;
		[SerializeField]
		private GameObject _connectPanel;

		private MetaData.MetaData _metaData = null;
		private Player _debugPlayer;
		public TwitchUser _broadcaster;

		public UIManager UIManager { get; set; }

		private Player _userPlayer;
		public Player UserPlayer => _userPlayer;
		public TwitchUser Broadcaster
		{ get; set; }

		private List<PathProbe> _pathProbes = new List<PathProbe>();
		public CameraController CameraController { get; set; }

		// Debug options
		[SerializeField]
		private bool _debugBuildingControls = true;
		public BuildingType LastBuildingType { get; set; } = BuildingType.Barracks;

		[SerializeField]
		private float _minButtonDelay = 0.2f;
		private float _buttonDelay = 0.02f;

		public TMPro.TMP_Text CodeDisplay;

		private string _code;
		public string Code => _code;
		// Debug Properties
		[field: SerializeField, Header("DEBUG OPTIONS")]
		public bool BuildingsCostResources { get; set; }
		[field: SerializeField]
		public bool PlayerRoleLimits { get; set; }
		[field: SerializeField]
		public bool IgnoreTechUnlocks { get; set; }
		public bool DebugBuildingControls => _debugBuildingControls;

		// Properties
		public Vector3 PlayerSpawnPosition => _playerSpawnPosition.position;
		public EnemySpawner EnemySpawner => _enemySpawner;

		public MetaData.MetaData MetaDatas => _metaData;

		public SettingsScriptable SettingsData => _settingsData;

		//Temp
		private Player _fakePlayer;
		private BuildingType _lastBuilding = BuildingType.Barracks;
		//END TEMP
		public List<PathProbe> PathProbes => _pathProbes;

		public GameObject ConnectPanel
		{
			set { _connectPanel = value; }
			get { return _connectPanel; }
		}


        private bool _idleMode = false;
        public bool IdleMode
        {
            get => _idleMode;
            set => _idleMode = value;
        }

        public void AddPathProbe(PathProbe probe) => _pathProbes.Add(probe);

		public void SetUserPlayer(Player player)
		{
			_userPlayer = player;
		}

		private void ProcessManagers()
		{
			//UpdateManager.Update();
			TileHelper.ProcessQueue();
			GameEventManager.ProcessEvents();
			AudioSourcesManager.ProcessSources();
		}

		private IEnumerator StartupSequence()
		{
			_code = Random.Range(100000, 999999).ToString();
            TwitchChatManager.SetExpectedConnectCode(_code);

            _connectPanel.SetActive(true);
			CodeDisplay.text = $"!CONNECT {_code}";
			BuildingManager.Initialize();
			PlayerManager.Initialize();
			TownGoalManager.Initialize();
			RoleManager.Initialize();
			yield return new WaitForEndOfFrame();
			TechTreeManager.InitializeTree();
			GUIDManager.Initialize();   // Must happen before pooling manager
			yield return StartCoroutine(ObjectPoolingManager.InitializePooling());
			if (_metaData != null)
			{
				if (_metaData.LoadType == MetaData.LoadType.Generate)
				{
					Debug.Log("Generating new world!");
					yield return StartCoroutine(ProceduralWorldGenerator.TryGenerateWorld());
				}

				else if (_metaData.LoadType == MetaData.LoadType.Load)
				{
					Debug.Log("Loading World!");
					SaveManager.LoadGame();
				}
			}
			else
				yield return StartCoroutine(ProceduralWorldGenerator.TryGenerateWorld());

			yield return new WaitForSeconds(3);
			AstarPath.active.Scan();
			GameStateManager.NotifyPlayerReady();
		}

		private void GetAllRequiredComponents()
		{
			GameObject obj = GameObject.Find("MetaData");

			if (obj != null)
				_metaData = obj.GetComponent<MetaData.MetaData>();
		}

		private void UpdateDebugBuildingControls()
		{
			if (!_debugBuildingControls)
				return;

			_buttonDelay -= Time.deltaTime;
			if (Keyboard.current.escapeKey.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
				BuildingManager.TryCancelBuilding(_userPlayer);


			if (Keyboard.current.eKey.wasReleasedThisFrame)
			{
				BuildingManager.TryRotateBuilding(_userPlayer, 1);
			}

			if (Keyboard.current.qKey.wasReleasedThisFrame)
			{
				BuildingManager.TryRotateBuilding(_userPlayer, -1);
			}

			if (Mouse.current.leftButton.wasReleasedThisFrame && !WorldUtils.IsPointerOverUI(EventSystem.current))
			{
				if (_userPlayer != null)
					if (BuildingManager.GetPlacerBuildingType(_userPlayer, out BuildingType type))
					{
						BuildingManager.TryPlaceBuilding(_userPlayer, out string message);
						BuildingManager.TryStartNewBuildingPlacer(_userPlayer, type, out message);
						Debug.Log(message);
					}
			}
		}

		private void IncrementBuildingType(ref BuildingType type)
		{
			type++;

			if (type >= BuildingType.Count)
				type = 0;
		}

		private void StartGameManager()
		{
			//_userPlayer = new Player(new Twitch.TwitchUser("69", "PLAYER"));
			StartupSequence();
		}

		private void Awake()
		{
			GameStateManager.ResetStateFlags();
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;

			if (Instance != null && Instance != this)
			{
				Debug.LogError("Multiple GameManager Instances found, this should not happen!");
				Destroy(this);
				return;
			}
			else
				Instance = this;

			GetAllRequiredComponents();
			//_userPlayer = new Player(new Twitch.TwitchUser("69", "PLAYER"));
			StartCoroutine(StartupSequence());
		}

		private IEnumerator WorldStart()
		{
			yield return StartCoroutine(ObjectPoolingManager.InitializePooling());
			yield return StartCoroutine(ProceduralWorldGenerator.TryGenerateWorld());
		}

		private void Update()
		{
			ProcessManagers();
			UpdateDebugBuildingControls();
		}

		private void OnDisable()
		{
			TL_Client.ForceDisconnect();
		}
	}
}