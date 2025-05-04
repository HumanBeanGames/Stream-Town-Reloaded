using Units;
using UnityEngine;
using UnityEngine.UI;
using Scriptables;
namespace UserInterface
{
	public class UserInterface_BuildingHealthBar : MonoBehaviour
	{
		private GameObject _displayUI;

		private bool _initialized = false;

		private HealthHandler _healthHandler;

		private Slider _healthBar;

		private BuildingHealthDisplayOption _displayOption;

		[SerializeField]
		private SettingsScriptable _settingsScriptable;

		/// <summary>
		/// Checks and changes the display option
		/// </summary>
		private void CheckWhatDisplayOption()
		{
			if (!_initialized)
				return;

			switch (_settingsScriptable.displayBuildings)
			{
				case 1:
					_displayOption = BuildingHealthDisplayOption.Damaged;
					break;
				case 2:
					_displayOption = BuildingHealthDisplayOption.Always;
					break;
				default:
					_displayOption = BuildingHealthDisplayOption.None;
					break;
			}

			switch (_displayOption)
			{
				case BuildingHealthDisplayOption.Damaged:
					if (_healthHandler.HealthPercentage < 1)
						_displayUI.SetActive(true);
					else
						_displayUI.SetActive(false);
					break;
				case BuildingHealthDisplayOption.Always:
					_displayUI.SetActive(true);
					break;
				default:
					_displayUI.SetActive(false);
					break;
			}
		}

		/// <summary>
		/// Updates the health bar to equal the health percentage
		/// </summary>
		public void UpdateHealthBar()
		{
			if (!_initialized)
				return;

			_healthBar.value = _healthHandler.HealthPercentage;
		}
		
		private void Awake()
		{
			_healthHandler = GetComponentInParent<HealthHandler>();
			_healthBar = GetComponentInChildren<Slider>();
			_displayUI = transform.GetChild(0).gameObject;
			_initialized = true;
		}

		private void Update()
		{
			CheckWhatDisplayOption();
		}
	}

	public enum BuildingHealthDisplayOption
	{
		None,
		Damaged,
		Always
	}
}