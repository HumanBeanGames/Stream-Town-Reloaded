using UnityEngine;
using TMPro;
using Buildings;
using Character;
using Enemies;
using GameResources;
using PlayerControls.ObjectSelection;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayerControls;


namespace UserInterface
{
	public class UserInterface_ObjectSelection : MonoBehaviour
	{
		[Header("Contexts")]
		[SerializeField]
		private GameObject _currentContext;
		[SerializeField]
		private GameObject _selectionOutline;

		[SerializeField]
		private TMP_Text _titleText;
		[SerializeField]
		private TMP_Text _descriptionText;
		[SerializeField]
		private GameObject _roleContainer;
		[SerializeField]
		private TMP_Text _roleText;
		[SerializeField]
		private Image _roleImage;
		[SerializeField]
		private GameObject _redSliderContainer;
		[SerializeField]
		private TMP_Text _redSliderText;
		[SerializeField]
		private Slider _redSlider;
		[SerializeField]
		private GameObject _greenSliderContainer;
		[SerializeField]
		private TMP_Text _greenSliderText;
		[SerializeField]
		private Slider _greenSlider;
		[SerializeField]
		private TMP_Text _IDText;
		[SerializeField]
		private GameObject _buttonHolder;
		[SerializeField]
		private Button _selectionButton;
		[SerializeField]
		private TMP_Text _selectionButtonText;
		[SerializeField]
		private GameObject _buttonHolderTwo;
		[SerializeField]
		private Button _selectionButtonTwo;
		[SerializeField]
		private TMP_Text _selectionButtonTextTwo;
		[SerializeField]
		public GameObject _dropdownHolder;
		[SerializeField]
		private TMP_Dropdown _selectionDropdown;
		[SerializeField]
		private ConfirmCheck _confirmCheck;

		public GameObject Context => _currentContext;

		public TMP_Text Title => _titleText;
		public TMP_Text Description => _descriptionText;
		public GameObject RoleContainer => _roleContainer;
		public TMP_Text Role => _roleText;
		public Image RoleImage => _roleImage;
		public GameObject RedSliderContainer => _redSliderContainer;
		public TMP_Text RedSliderValue => _redSliderText;
		public Slider RedSlider => _redSlider;
		public GameObject GreenSliderContainer => _greenSliderContainer;
		public TMP_Text GreenSliderValue => _greenSliderText;
		public Slider GreenSlider => _greenSlider;
		public TMP_Text ID => _IDText;
		public GameObject ButtonHolder => _buttonHolder;
		public Button SelectionButton => _selectionButton;
		public TMP_Text SelectionButtonText => _selectionButtonText;
		public GameObject ButtonHolderTwo => _buttonHolderTwo;
		public Button SelectionButtonTwo => _selectionButtonTwo;
		public TMP_Text SelectionButtonTextTwo => _selectionButtonTextTwo;

		public GameObject DropdownHolder => _dropdownHolder;
		public TMP_Dropdown SelectionDropdown => _selectionDropdown;
		public ConfirmCheck ConfirmCheck => _confirmCheck;

		private SelectedBuilding _selectedBuilding = null;
		private SelectedPlayer _selectedPlayer = null;
		private SelectedResource _selectedResource = null;
		private SelectedEnemyCamp _selectableEnemyCamp = null;
		private SelectedEnemy _selectableEnemy = null;
		private SelectedPlayerGroup _selectedPlayerGroup = null;

		private SelectedObject _selectedObject = null;
		private BoxCollider _boxCollider;
		private object _data;
		private ContextType _contextType;
		private enum ContextType
		{
			Character,
			Buildings,
			Enemy,
			Resource,
			EnemyCamp,
			CharacterGroup
		}

		/// <summary>
		/// Called when the Character is selected, and updates the displayed data.
		/// </summary>
		/// <param name="character"></param>
		public void OnCharacterContext(RoleHandler character)
		{
			_data = character;
			HideContext();
			_contextType = ContextType.Character;
			_selectedPlayer.SetDisplay(_data);
			_selectedObject = _selectedPlayer;
			ShowContext();
			SetSelectionOutline(character.GetComponent<BoxCollider>());
		}

		public void DisableCheckUI()
		{
			_confirmCheck.DisableCheck();
		}

		public void OnCharacterGroupContext(List<RoleHandler> characters)
		{
			_data = characters;
			HideContext();
			_contextType = ContextType.CharacterGroup;
			_selectedPlayerGroup.SetDisplay(_data);
			_selectedObject = _selectedPlayerGroup;
			ShowContext();
			//SetSelectionOutline(character.GetComponent<BoxCollider>());
		}

		/// <summary>
		/// Called when the Building is selected, and updates the displayed data.
		/// </summary>
		/// <param name="building"></param>
		public void OnBuildingContext(BuildingBase building)
		{
			_data = building;
			HideContext();
			_contextType = ContextType.Buildings;
			_selectedBuilding.SetDisplay(_data);
			_selectedObject = _selectedBuilding;
			ShowContext();
			SetSelectionOutline(building.GetComponent<BoxCollider>());
		}

		/// <summary>
		/// Called when an enemy is selected, and updates the displayed data.
		/// </summary>
		/// <param name="enemy"></param>
		public void OnEnemyContext(Enemy enemy)
		{
			_data = enemy;
			HideContext();
			_contextType = ContextType.Enemy;
			_selectableEnemy.SetDisplay(_data);
			_selectedObject = _selectableEnemy;
			ShowContext();
			SetSelectionOutline(enemy.GetComponent<BoxCollider>());
		}

		/// <summary>
		/// Called when a resource is selected, and updates the displayed data.
		/// </summary>
		/// <param name="resource"></param>
		public void OnResourceContext(ResourceHolder resource)
		{
			_data = resource;
			HideContext();
			_contextType = ContextType.Resource;
			_selectedResource.SetDisplay(_data);
			_selectedObject = _selectedResource;
			ShowContext();
			SetSelectionOutline(resource.GetComponent<BoxCollider>());
		}

		/// <summary>
		/// Called when a camp is selected, and updates the displayed data.
		/// </summary>
		/// <param name="camp"></param>
		public void OnEnemyCampContext(Station camp)
		{
			_data = camp;
			HideContext();
			_selectableEnemyCamp.SetDisplay(_data);
			_selectedObject = _selectableEnemyCamp;
			_contextType = ContextType.EnemyCamp;
			ShowContext();
			SetSelectionOutline(camp.GetComponent<BoxCollider>());
		}

		private void SetSelectionOutline(BoxCollider collider)
		{
			_selectionOutline.transform.position = new Vector3(collider.transform.position.x, 0.15f, collider.transform.position.z);
			_selectionOutline.transform.rotation = collider.transform.rotation;
			_selectionOutline.transform.parent = collider.transform;
			_selectionOutline.transform.localScale = new Vector3(collider.size.x * 1.25f, 1, collider.size.z * 1.25f);
			_selectionOutline.SetActive(true);
		}

		public void SetGroupSelectionArea(Vector3 startSelection, Vector3 endSelection)
		{
			_selectionOutline.transform.localPosition = startSelection + ((endSelection - startSelection) * 0.5f);
			_selectionOutline.transform.eulerAngles = Vector3.zero;
			_selectionOutline.transform.localScale = new Vector3(Mathf.Abs((endSelection.x - startSelection.x)), 1, Mathf.Abs((endSelection.z - startSelection.z)));
			_selectionOutline.transform.parent = null;
			_selectionOutline.SetActive(true);
		}

		public void HideContext()
		{
			_currentContext.SetActive(false);
			_titleText.gameObject.SetActive(false);
			_descriptionText.gameObject.SetActive(false);
			_roleContainer.gameObject.SetActive(false);
			_redSliderContainer.gameObject.SetActive(false);
			_greenSliderContainer.gameObject.SetActive(false);
			_IDText.gameObject.SetActive(false);
			_buttonHolder.gameObject.SetActive(false);
			_selectionOutline.SetActive(false);
			_buttonHolder.SetActive(false);
			_dropdownHolder.SetActive(false);
			_buttonHolderTwo.SetActive(false);

			if (_selectedObject != null)
			{
				switch (_contextType)
				{
					case ContextType.Character:
						((SelectedPlayer)_selectedObject).DetachCurrentEvents();
						break;
					case ContextType.Buildings:
						((SelectedBuilding)_selectedObject).DetachCurrentEvents();
						break;
					case ContextType.Enemy:
						((SelectedEnemy)_selectedObject).DetachCurrentEvents();
						break;
					case ContextType.Resource:
						((SelectedResource)_selectedObject).DetachCurrentEvents();
						break;
					case ContextType.CharacterGroup:
						((SelectedPlayerGroup)_selectedObject).DetachCurrentEvents();
						break;
				}
			}
			_selectedObject = null;

			// Temporary till we can use it
			_IDText.gameObject.SetActive(false);
		}

		public void HideSelectionOutline()
		{
			_selectionOutline.gameObject.SetActive(false);
		}
		public void ShowContext()
		{
			_currentContext.SetActive(true);
			_IDText.gameObject.SetActive(false);
		}

		/// <summary>
		/// Initializes the debug user interface.
		/// </summary>
		private void InitializeInterface()
		{
			_selectedBuilding = new SelectedBuilding();
			_selectableEnemy = new SelectedEnemy();
			_selectableEnemyCamp = new SelectedEnemyCamp();
			_selectedPlayer = new SelectedPlayer();
			_selectedPlayerGroup = new SelectedPlayerGroup();
			_selectedResource = new SelectedResource();

			_selectedBuilding.SelectableObjectUI = this;
			_selectableEnemy.SelectableObjectUI = this;
			_selectableEnemyCamp.SelectableObjectUI = this;
			_selectedPlayer.SelectableObjectUI = this;
			_selectedPlayerGroup.SelectableObjectUI = this;
			_selectedResource.SelectableObjectUI = this;

			HideContext();
		}

		private void Awake()
		{
			InitializeInterface();
		}
	}

}