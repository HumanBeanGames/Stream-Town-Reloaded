using Character;
using Managers;
using System;
using System.Collections.Generic;
using TechTree;
using Twitch;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;
using PlayerControls;
using UnityEngine.Events;

namespace UserInterface.BottomBarMenu
{
	public class BottomBarInterface : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _contextLayoutGroup;
		[SerializeField]
		private GameObject _contextBar;
		[SerializeField]
		private GameObject _contextButtonPrefab;

		[SerializeField]
		private Button _leftArrow;

		[SerializeField]
		private Button _rightArrow;

		[SerializeField]
		private List<RecruitContextItem> _recruitContextData;

		[SerializeField]
		private List<BuildingContextItem> _buildingContextData;

		private static Dictionary<BottomBarContext, BottomBarButton> _mainButtons = new Dictionary<BottomBarContext, BottomBarButton>();
		private static int _maxShownButtonsOnBar = 10;
		private static BottomBarButton _activeButton;

		private static List<GameObject> _activeContextButtons = new List<GameObject>();

		private static List<(BuildingType, GameObject)> _buildButtons = new List<(BuildingType, GameObject)>();

		private static List<(PlayerRole, GameObject)> _roleButtons = new List<(PlayerRole, GameObject)>();

		private static int _buildScrollIndex = 0;
		private static int _roleScrollIndex = 0;
		private static int _availableBuildings = 0;
		private static int _availableRecruitRoles = 0;

		private static BottomBarContext _bottomBarContext;

		private static Action _contextHidden;
		private static Action _contextShown;
		private static Action _canScrollRight;
		private static Action _cantScrollRight;
		private static Action _canScrollLeft;
		private static Action _cantScrollLeft;

		private static Dictionary<BuildingType, BuildingContextItem> _buildingsLookup;
		private static Dictionary<PlayerRole, RecruitContextItem> _recruitsLookup;

		public static UnityAction<Resource, int, bool> OnRecruitAdded;
		public static void AddButton(BottomBarButton button)
		{
			if (!_mainButtons.ContainsKey(button.ButtonContext))
				_mainButtons.Add(button.ButtonContext, button);
		}

		public static void RemoveButton(BottomBarButton button)
		{
			if (_mainButtons.ContainsKey(button.ButtonContext))
				_mainButtons.Remove(button.ButtonContext);
		}

		public static void OnActivatedButton(BottomBarButton button)
		{
			if (_activeButton != null)
			{
				_activeButton.OnContextHidden();
				HideContext(_activeButton.ButtonContext);
			}

			button.OnContextShown();
			_activeButton = button;
			ShowContext(button.ButtonContext);
		}

		public static void OnDeactivatedButton(BottomBarButton button)
		{
			if (_activeButton != null)
			{
				_activeButton.OnContextHidden();
				HideContext(_activeButton.ButtonContext);
			}
		}

		private static void ShowContext(BottomBarContext context)
		{
			switch (context)
			{
				case BottomBarContext.BuildMenu:
					ShowBuildMenu();
					break;
				case BottomBarContext.RecruitMenu:
					ShowRecruitMenu();
					GameManager.Instance.TownResourceManager.OnAnyResourceChangeEvent.AddListener(OnRecruitAdded);
					break;
				case BottomBarContext.TechTreeMenu:
					break;
				default:
					break;
			}
		}

		private static void HideContext(BottomBarContext context)
		{
			switch (context)
			{
				case BottomBarContext.BuildMenu:
					HideContextMenu();
					break;
				case BottomBarContext.RecruitMenu:
					HideContextMenu();
					GameManager.Instance.TownResourceManager.OnAnyResourceChangeEvent.RemoveListener(OnRecruitAdded);
					break;
				case BottomBarContext.TechTreeMenu:
					break;
				default:
					break;
			}
		}

		// Note - hehehe someone spelt this wrong
		public void SrollLeft()
		{
			if (_activeButton.ButtonContext == BottomBarContext.RecruitMenu)
				_roleScrollIndex--;
			else if (_activeButton.ButtonContext == BottomBarContext.BuildMenu)
				_buildScrollIndex--;
			HideContextMenu();
			UpdateScrollButtons();
			ShowContext(_activeButton.ButtonContext);
		}

		public void ScrollRight()
		{
			if (_activeButton.ButtonContext == BottomBarContext.RecruitMenu)
				_roleScrollIndex++;
			else if (_activeButton.ButtonContext == BottomBarContext.BuildMenu)
				_buildScrollIndex++;

			HideContextMenu();
			UpdateScrollButtons();
			ShowContext(_activeButton.ButtonContext);
		}

		private static void ShowBuildMenu()
		{
			BuildingManager bm = GameManager.Instance.BuildingManager;

			List<GameObject> availableButtons = new List<GameObject>();

			for (int i = 0; i < _buildButtons.Count; i++)
			{
				if (bm.IsBuildingUnlocked(_buildButtons[i].Item1))
					availableButtons.Add(_buildButtons[i].Item2);

				if (bm.CanAffordToBuild(_buildButtons[i].Item1))
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = true;
				else
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = false;
			}

			_activeContextButtons.AddRange(availableButtons.GetRange(_buildScrollIndex, Mathf.Min(_maxShownButtonsOnBar, availableButtons.Count)));
			for (int i = 0; i < _activeContextButtons.Count; i++)
				_activeContextButtons[i].SetActive(true);

			_availableBuildings = availableButtons.Count;

			_contextShown?.Invoke();
		}

		private static void HideContextMenu()
		{
			RemoveCurrentContextButtons();
			_contextHidden?.Invoke();
		}

		private static void ShowRecruitMenu()
		{
			RoleManager rm = GameManager.Instance.RoleManager;

			List<GameObject> availableButtons = new List<GameObject>();

			for (int i = 0; i < _roleButtons.Count; i++)
			{
				if (rm.GetMaxSlots(_roleButtons[i].Item1) > 0 || rm.RoleIsInfinite(_roleButtons[i].Item1))
					availableButtons.Add(_roleButtons[i].Item2);

				if (rm.SlotsFull(_roleButtons[i].Item1) || GameManager.Instance.TownResourceManager.ResourceFull(Resource.Recruit))
					_roleButtons[i].Item2.GetComponentInChildren<Button>().interactable = false;
				else
					_roleButtons[i].Item2.GetComponentInChildren<Button>().interactable = true;
			}

			_activeContextButtons.AddRange(availableButtons.GetRange(_roleScrollIndex, Mathf.Min(_maxShownButtonsOnBar, availableButtons.Count)));
			for (int i = 0; i < _activeContextButtons.Count; i++)
				_activeContextButtons[i].SetActive(true);

			_availableRecruitRoles = availableButtons.Count;
			_contextShown?.Invoke();
		}

		private static void RemoveCurrentContextButtons()
		{
			for (int i = _activeContextButtons.Count - 1; i >= 0; i--)
			{
				GameObject go = _activeContextButtons[i];
				_activeContextButtons.Remove(go);
				go.SetActive(false);
			}
		}

		private void OnCanScrollRight()
		{
			_rightArrow.interactable = true;
		}

		private void OnCantScrollRight()
		{
			_rightArrow.interactable = false;
		}

		private void OnCanScrollLeft()
		{
			_leftArrow.interactable = true;
		}

		private void OnCantScrollLeft()
		{
			_leftArrow.interactable = false;
		}

		private void BuildContextButtons()
		{
			// Create buttons for buildings
			// Create buttons for roles

			_buildButtons = new List<(BuildingType, GameObject)>();

			for (int i = 0; i < (int)BuildingType.Count; i++)
			{
				BuildingType type = (BuildingType)i;
				if (!_buildingsLookup.ContainsKey(type))
					continue;

				GameObject contextButton = Instantiate(_contextButtonPrefab, _contextLayoutGroup);

				Button btn = contextButton.GetComponentInChildren<Button>();
				Image img = contextButton.GetComponentInChildren<Image>();
				img.sprite = _buildingsLookup[type].Sprite;
				btn.onClick.AddListener(() => OnBuildingButtonClicked(type));
				_buildButtons.Add((type, contextButton));
				contextButton.SetActive(false);
			}

			_roleButtons = new List<(PlayerRole, GameObject)>();
			for (int i = 0; i < (int)PlayerRole.Count; i++)
			{
				PlayerRole role = (PlayerRole)i;
				if (!_recruitsLookup.ContainsKey(role))
					continue;

				GameObject contextButton = Instantiate(_contextButtonPrefab, _contextLayoutGroup);

				Button btn = contextButton.GetComponentInChildren<Button>();
				Image img = contextButton.GetComponentInChildren<Image>();
				img.sprite = _recruitsLookup[role].Sprite;
				btn.onClick.AddListener(() => OnRoleButtonClicked(role));
				_roleButtons.Add((role, contextButton));
				contextButton.SetActive(false);
			}
		}

		private static void OnBuildingButtonClicked(BuildingType buildingType)
		{
			// Try to create building placer with that building type or update current building placer?
			// Escape to cancel building
			Player player = GameManager.Instance.UserPlayer;

			BuildingManager bm = GameManager.Instance.BuildingManager;

			// This is terrible lmao
			List<Button> buttonList = new List<Button>();
			for (int i = 0; i < _buildButtons.Count; i++)
			{
				buttonList.Add(_buildButtons[i].Item2.GetComponentInChildren<Button>());
			}

			for (int i = 0; i < _buildButtons.Count; i++)
			{
				if (bm.CanAffordToBuild(_buildButtons[i].Item1))
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = true;
				else
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = false;
			}

			bm.TryCancelBuilding(player);
			bm.TryStartNewBuildingPlacer(player, buildingType, out string msg);

			GameManager.Instance.LastBuildingType = buildingType;
			for (int i = 0; i < _buildButtons.Count; i++)
			{
				if (bm.CanAffordToBuild(_buildButtons[i].Item1))
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = true;
				else
					_buildButtons[i].Item2.GetComponentInChildren<Button>().interactable = false;
			}
		}
		private static void OnRoleButtonClicked(PlayerRole role)
		{
			List<Button> buttonList = new List<Button>();
			for (int i = 0; i < _roleButtons.Count; i++)
			{
				buttonList.Add(_roleButtons[i].Item2.GetComponentInChildren<Button>());
			}

			if (GameManager.Instance.TownResourceManager.ResourceFull(Resource.Recruit))
				for (int i = 0; i < buttonList.Count; i++)
					buttonList[i].interactable = false;

			RoleManager rm = GameManager.Instance.RoleManager;
			if (rm.SlotsFull(role) || GameManager.Instance.TownResourceManager.ResourceFull(Resource.Recruit))
				return;

			Player recruit = new Player(new TwitchUser($"{-UnityEngine.Random.Range(int.MinValue, 0)}", $""), true);
			GameManager.Instance.PlayerManager.AddNewPlayer(recruit, role);

			if (GameManager.Instance.TownResourceManager.ResourceFull(Resource.Recruit))
			{
				for (int i = 0; i < buttonList.Count; i++)
					buttonList[i].interactable = false;
			}
		}

		private void ContextHiddenEvent()
		{
			_contextBar.SetActive(false);
		}

		private void ContextShownEvent()
		{
			_contextBar.SetActive(true);

			UpdateScrollButtons();
		}

		private void UpdateScrollButtons()
		{
			if (_activeButton.ButtonContext == BottomBarContext.BuildMenu)
			{
				if (_availableBuildings > _maxShownButtonsOnBar && (_availableBuildings - _buildScrollIndex > _maxShownButtonsOnBar))
					_canScrollRight?.Invoke();
				else
					_cantScrollRight?.Invoke();

				if (_buildScrollIndex == 0)
					_cantScrollLeft?.Invoke();
				else
					_canScrollLeft?.Invoke();
			}
			else if (_activeButton.ButtonContext == BottomBarContext.RecruitMenu)
			{
				if (_availableRecruitRoles > _maxShownButtonsOnBar && (_availableRecruitRoles - _roleScrollIndex > _maxShownButtonsOnBar))
					_canScrollRight?.Invoke();
				else
					_cantScrollRight?.Invoke();

				if (_roleScrollIndex == 0)
					_cantScrollLeft?.Invoke();
				else
					_canScrollLeft?.Invoke();
			}


			if (_activeButton.ButtonContext == BottomBarContext.BuildMenu && _buildScrollIndex == 0)
				_cantScrollLeft?.Invoke();
			else if (_activeButton.ButtonContext == BottomBarContext.BuildMenu && _buildScrollIndex > 0)
				_canScrollLeft?.Invoke();

			else if (_activeButton.ButtonContext == BottomBarContext.RecruitMenu && _roleScrollIndex == 0)
				_cantScrollLeft?.Invoke();
			else if (_activeButton.ButtonContext == BottomBarContext.RecruitMenu && _roleScrollIndex > 0)
				_canScrollLeft?.Invoke();

		}

		private void InitDictionaryData()
		{
			_buildingsLookup = new Dictionary<BuildingType, BuildingContextItem>();

			for (int i = 0; i < _buildingContextData.Count; i++)
			{
				if (_buildingContextData[i].Available)
					_buildingsLookup.Add(_buildingContextData[i].Building, _buildingContextData[i]);
			}

			_recruitsLookup = new Dictionary<PlayerRole, RecruitContextItem>();

			for (int i = 0; i < _recruitContextData.Count; i++)
			{
				if (_recruitContextData[i].Available)
					_recruitsLookup.Add(_recruitContextData[i].Role, _recruitContextData[i]);
			}
		}

		public static void UpdateBarButtons(Resource resource, int amount, bool yes)
		{
			ShowRecruitMenu();
		}

		private void Awake()
		{
			_mainButtons = new Dictionary<BottomBarContext, BottomBarButton>();
			_activeButton = null;
			_mainButtons = new Dictionary<BottomBarContext, BottomBarButton>();
			InitDictionaryData();
			//_contextHidden += ContextHiddenEvent;
			//_contextShown += ContextShownEvent;
			BuildContextButtons();
			HideContextMenu();
			//_leftArrow.interactable = false;
			//_rightArrow.interactable = false;
			//_canScrollRight += OnCanScrollRight;
			//_cantScrollRight += OnCantScrollRight;
			//_canScrollLeft += OnCanScrollLeft;
			//_cantScrollLeft += OnCantScrollLeft;
		}

		private void OnEnable()
		{
			PlayerControls.PlayerInputManager.OnBuildMenu += OnBuildMenuKey;
			PlayerControls.PlayerInputManager.OnRecruit += OnRecruitMenuKey;
			PlayerControls.PlayerInputManager.OnTechTree += OnTechTreeKey;
			_contextHidden += ContextHiddenEvent;
			_contextShown += ContextShownEvent;
			_canScrollRight += OnCanScrollRight;
			_cantScrollRight += OnCantScrollRight;
			_canScrollLeft += OnCanScrollLeft;
			_cantScrollLeft += OnCantScrollLeft;
			OnRecruitAdded += UpdateBarButtons;
		}

		private void OnDisable()
		{
			PlayerControls.PlayerInputManager.OnBuildMenu -= OnBuildMenuKey;
			PlayerControls.PlayerInputManager.OnRecruit -= OnRecruitMenuKey;
			PlayerControls.PlayerInputManager.OnTechTree -= OnTechTreeKey;
			_contextHidden -= ContextHiddenEvent;
			_contextShown -= ContextShownEvent;
			_canScrollRight -= OnCanScrollRight;
			_cantScrollRight -= OnCantScrollRight;
			_canScrollLeft -= OnCanScrollLeft;
			_cantScrollLeft -= OnCantScrollLeft;
			OnRecruitAdded -= UpdateBarButtons;
		}
		private void OnBuildMenuKey()
		{
			_mainButtons[BottomBarContext.BuildMenu].ToggleContext();
		}

		private void OnRecruitMenuKey()
		{
			_mainButtons[BottomBarContext.RecruitMenu].ToggleContext();
		}

		private void OnTechTreeKey()
		{
			GameManager.Instance.UIManager.TownVoteInterface.ToggleVotingMenu();
		}

		[Serializable]
		public class ContextItem
		{
			public Sprite Sprite;
			public bool Available;
		}

		[Serializable]
		public class RecruitContextItem : ContextItem
		{
			public PlayerRole Role;
		}

		[Serializable]
		public class BuildingContextItem : ContextItem
		{
			public BuildingType Building;
		}
	}
}