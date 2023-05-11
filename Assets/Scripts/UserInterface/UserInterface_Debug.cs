using Buildings;
using Character;
using UnityEngine;
using TMPro;
using Utils;
using Level;
using System.Collections.Generic;
using Managers;
using Utils.Pooling;

namespace UserInterface
{
	/// <summary>
	/// Handles the User Interace for the Debug Menus
	/// </summary>
	public class UserInterface_Debug : MonoBehaviour
	{
		[Header("Contexts")]
		[SerializeField]
		private GameObject _characterContext;
		[SerializeField]
		private GameObject _debugContext;
		[SerializeField]
		private GameObject _buildingContext;

		[Header("Debug Context Data")]
		[SerializeField]
		private TMP_Dropdown _roleDropdownDebug;

		[Header("Character Context Data")]
		[SerializeField]
		private TextMeshProUGUI _characterRole;
		[SerializeField]
		private TextMeshProUGUI _roleLevel;
		[SerializeField]
		private TMP_Dropdown _roleDropdownCharacter;

		[Header("Building Context Data")]
		[SerializeField]
		private TextMeshProUGUI _buildingName;
		[SerializeField]
		private TextMeshProUGUI _buildingLevel;

		[SerializeField]
		private GameObject _selectionOutline;

		private object _data;

		/// <summary>
		/// Enables the Character Debug Menu.
		/// </summary>
		public void ShowCharacterContext()
		{
			_characterContext.SetActive(true);
		}

		/// <summary>
		/// Disables the Character Debug Menu.
		/// </summary>
		public void HideCharacterContext()
		{
			_selectionOutline.SetActive(false);
			_characterContext.SetActive(false);
		}

		/// <summary>
		/// Enables the Main Debug Context.
		/// </summary>
		public void ShowDebugContext()
		{
			_debugContext.SetActive(true);
		}

		/// <summary>
		/// Disables the Main Debug Context.
		/// </summary>
		public void HideDebugContext()
		{
			_debugContext.SetActive(false);
		}

		/// <summary>
		/// Enables the Building Debug Menu.
		/// </summary>
		public void ShowBuildingContext()
		{
			_buildingContext.SetActive(true);
		}

		/// <summary>
		/// Disables the Building Debug Menu.
		/// </summary>
		public void HideBuildingContext()
		{
			_selectionOutline.SetActive(false);
			_buildingContext.SetActive(false);
		}

		/// <summary>
		/// Removes a selected building from the game.
		/// </summary>
		public void RemoveBuilding()
		{
			if (((BuildingBase)_data).gameObject.activeInHierarchy)
				((BuildingBase)_data).gameObject.SetActive(false);
			else
				HideBuildingContext();
		}

		/// <summary>
		/// Levels up the selected building by 1.
		/// </summary>
		public void LevelBuilding()
		{
			if (((BuildingBase)_data).gameObject.activeInHierarchy)
			{
				((BuildingBase)_data).GetComponent<LevelHandler>().TryLevel();
				_buildingLevel.text = "Level: " + ((BuildingBase)_data).GetComponent<LevelHandler>().Level.ToString();
			}
			else
				HideBuildingContext();
		}

		/// <summary>
		/// Levels up the selected character's currently active role by 1.
		/// </summary>
		public void LevelCharacter()
		{
			if (((RoleHandler)_data).gameObject.activeInHierarchy)
			{
				((RoleHandler)_data).PlayerRoleData.LevelUp();
				_roleLevel.text = "Level: " + ((RoleHandler)_data).PlayerRoleData.CurrentLevel;
			}
			else
				HideCharacterContext();
		}

		/// <summary>
		/// Attempts to spawn a new character as the selected role. If the role is unavailable, it will spawn a builder.
		/// </summary>
		public void SpawnCharacter()
		{
			PoolableObject obj = GameManager.Instance.PoolingManager.GetPooledObject("Player");
			obj.gameObject.SetActive(true);
			obj.transform.position = GameManager.Instance.PlayerSpawnPosition;
			obj.GetComponent<RoleHandler>().SetStarterRole((PlayerRole)_roleDropdownDebug.value);
		}

		/// <summary>
		/// Attempts to switch the selected character's active role.
		/// </summary>
		public void SetCharacterRole()
		{
			//GameManager.Instance.RoleManager.QueueRoleChange((RoleHandler)_data, (PlayerRole)_roleDropdownCharacter.value);
			((RoleHandler)_data).TrySetRole((PlayerRole)_roleDropdownCharacter.value);
			_characterRole.text = "Role: " + ((RoleHandler)_data).CurrentRole.ToString();
			_roleLevel.text = "Level: " + ((RoleHandler)_data).PlayerRoleData.CurrentLevel;
		}

		/// <summary>
		/// Called when the Character Debug Menu is enabled and updates the displayed data.
		/// </summary>
		/// <param name="character"></param>
		public void OnCharacterContext(RoleHandler character)
		{
			_data = character;
			HideBuildingContext();
			_characterRole.text = "Role: " + character.CurrentRole.ToString();
			_roleLevel.text = "Level: 1";
			_roleDropdownCharacter.value = (int)character.CurrentRole;
			ShowCharacterContext();
			_selectionOutline.transform.position = new Vector3(character.transform.position.x, 0.15f, character.transform.position.z);
			_selectionOutline.transform.rotation = character.transform.rotation;
			_selectionOutline.transform.parent = character.transform;
			_selectionOutline.transform.localScale = Vector3.one * 1.25f;
			_selectionOutline.SetActive(true);
		}


		/// <summary>
		/// Called when the Building Debug Menu is enabled, and updates the displayed data.
		/// </summary>
		/// <param name="building"></param>
		public void OnBuildingContext(BuildingBase building)
		{
			_data = building;
			HideCharacterContext();
			_buildingName.text = building.BuildingData.BuildingName;
			_buildingLevel.text = "Level: " + building.GetComponent<LevelHandler>().Level.ToString();
			ShowBuildingContext();
			_selectionOutline.transform.position = new Vector3(building.transform.position.x, 0.15f, building.transform.position.z);
			_selectionOutline.transform.rotation = building.transform.rotation;
			_selectionOutline.transform.parent = building.transform;
			BoxCollider bc = building.GetComponent<BoxCollider>();
			_selectionOutline.transform.localScale = new Vector3(bc.size.x * 1.25f, 1, bc.size.z * 1.25f);
			_selectionOutline.SetActive(true);
		}

		/// <summary>
		/// Changes the time scale of the game.
		/// </summary>
		/// <param name="scale"></param>
		public void SetTimeScale(float scale)
		{
			Time.timeScale = scale;
		}

		/// <summary>
		/// Adds the specified amount of wood to the town's resources.
		/// </summary>
		/// <param name="value"></param>
		public void AddWood(int value)
		{
			GameManager.Instance.TownResourceManager.AddResource(Utils.Resource.Wood, value);
		}

		/// <summary>
		/// Adds the specified amount of ore to the town's resources.
		/// </summary>
		/// <param name="value"></param>
		public void AddOre(int value)
		{
			GameManager.Instance.TownResourceManager.AddResource(Utils.Resource.Ore, value);
		}

		/// <summary>
		/// Adds the specified amount of food to the town's resources.
		/// </summary>
		/// <param name="value"></param>
		public void AddFood(int value)
		{
			GameManager.Instance.TownResourceManager.AddResource(Utils.Resource.Food, value);
		}

		/// <summary>
		/// Adds the specified amount of gold to the town's resources.
		/// </summary>
		/// <param name="value"></param>
		public void AddGold(int value)
		{
			GameManager.Instance.TownResourceManager.AddResource(Utils.Resource.Gold, value);
		}

		/// <summary>
		/// Initializes the debug user interface.
		/// </summary>
		private void InitializeInterface()
		{
			HideBuildingContext();
			HideCharacterContext();

			_roleDropdownDebug.ClearOptions();
			_roleDropdownCharacter.ClearOptions();

			List<string> options = new List<string>();

			for (int i = 0; i < (int)PlayerRole.Count; i++)
			{
				options.Add(((PlayerRole)i).ToString());
			}

			_roleDropdownDebug.AddOptions(options);
			_roleDropdownCharacter.AddOptions(options);
		}

		private void Awake()
		{
			InitializeInterface();
		}
	}
}