using System;
using System.Collections;
using System.Collections.Generic;
using TechTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;
using Utils;

namespace Managers
{
	// TODO: MAKE PARTIAL CLASS
	public class UIManager : MonoBehaviour
	{
		// RESOURCES
		[SerializeField]
		private TextMeshProUGUI _woodDisplayText;
		[SerializeField]
		private TextMeshProUGUI _foodDisplayText;
		[SerializeField]
		private TextMeshProUGUI _oreDisplayText;
		[SerializeField]
		private TextMeshProUGUI _goldDisplayText;

		// RATES OF CHANGE
		[SerializeField]
		private TextMeshProUGUI _woodRateOfChangeText;
		[SerializeField]
		private TextMeshProUGUI _foodRateOfChangeText;
		[SerializeField]
		private TextMeshProUGUI _oreRateOfChangeText;
		[SerializeField]
		private TextMeshProUGUI _goldRateOfChangeText;

		// SEASON
		[SerializeField]
		private Slider _seasonalSlider;
		[SerializeField]
		private float _seaonSliderStartOffset;

		// PLAYER COUNT
		[SerializeField]
		private TextMeshProUGUI _playerCountText;

		// BUILDING COUNT
		[SerializeField]
		private TextMeshProUGUI _buildingCountText;

		// TIME DISPLAY
		[SerializeField]
		private TextMeshProUGUI _timeDisplayText;

        private UserInterface_TownGoal _townGoalInterface;
        private UserInterface_RulerVote _rulerVoteInterface;
        private UserInterface_TownVote _townVoteInterface;
        private UserInterface_Event _eventInterface;

        public UserInterface_TownGoal TownGoalInterface => _townGoalInterface;
        public UserInterface_RulerVote RulerVoteInterface => _rulerVoteInterface;
        public UserInterface_TownVote TownVoteInterface => _townVoteInterface;
        public UserInterface_Event EventInterface => _eventInterface;

		private GameManager _gm;

		private void UpdateResourcesDisplay()
		{
			_woodDisplayText.text = FormattedResourceString(_gm.TownResourceManager.CurrentResourceAmount(Resource.Wood), _gm.TownResourceManager.MaxResourceAmount(Resource.Wood));
			_foodDisplayText.text = FormattedResourceString(_gm.TownResourceManager.CurrentResourceAmount(Resource.Food), _gm.TownResourceManager.MaxResourceAmount(Resource.Food));
			_oreDisplayText.text = FormattedResourceString(_gm.TownResourceManager.CurrentResourceAmount(Resource.Ore), _gm.TownResourceManager.MaxResourceAmount(Resource.Ore));
			_goldDisplayText.text = FormattedResourceString(_gm.TownResourceManager.CurrentResourceAmount(Resource.Gold));
		}

		private void UpdateResourcesRateOfChange()
        {
			_woodRateOfChangeText.text = FormattedRateOfChangeString(_gm.TownResourceManager.RateOfChangeForResource(Resource.Wood));
			_foodRateOfChangeText.text = FormattedRateOfChangeString(_gm.TownResourceManager.RateOfChangeForResource(Resource.Food));
			_oreRateOfChangeText.text = FormattedRateOfChangeString(_gm.TownResourceManager.RateOfChangeForResource(Resource.Ore));
			_goldRateOfChangeText.text = FormattedRateOfChangeString(rateOfChange: _gm.TownResourceManager.RateOfChangeForResource(Resource.Gold));
        }

		private void UpdateCountTexts()
		{
			_buildingCountText.text = _gm.BuildingManager.NumberOfBuildings.ToString();
			_playerCountText.text = _gm.PlayerManager.PlayerCount().ToString();
		}

		private void UpdateSeasonSlider()
		{
			float newValue = ((_gm.TimeManager.WorldTimePassed + _seaonSliderStartOffset) / (float)_gm.TimeManager.SecondsPerDay / (float)_gm.SeasonManager.DaysPerSeason) / 4f;
			int roundedDown = (int)Mathf.Floor(newValue);
			_seasonalSlider.value = newValue - roundedDown;
		}

		private void UpdateTimeOfDay()
		{
			TimeSpan t = TimeSpan.FromSeconds(_gm.TimeManager.WorldTimePassed);
			string newString = "";
			string formatted = string.Format("{0:D1}", t.Days);
			newString += $"<size=48>{formatted}</size><size=32><color=#958450>D</color></size> ";
			formatted = string.Format("{0:D2}", t.Hours);
			newString += $"<size=48>{formatted}</size><size=32><color=#958450>H</color></size> ";
			formatted = string.Format("{0:D2}", t.Minutes);
			newString += $"<size=48>{formatted}</size><size=32><color=#958450>M</color></size> ";
			formatted = string.Format("{0:D2}", t.Seconds);
			newString += $"<size=48>{formatted}</size><size=32><color=#958450>S</color></size>";
			_timeDisplayText.text = newString;
			//_timeDisplayText.text = string.Format("{0:D1}D:{1:D2}H,{2:D2}M:{3:D3}S", t.Days, t.Hours, t.Minutes, t.Seconds);
		}

		private string FormattedResourceString(int currentAmount, int maxAmount = -1)
		{
			string newString = $"<size=48>{StringUtils.GetShortenedNumberAsString(currentAmount)}</size>";

			if (maxAmount != -1)
				newString += $"<size=32><color=#958450> / {StringUtils.GetShortenedNumberAsString(maxAmount)}</color></size>";

			return newString;
		}

		private string FormattedRateOfChangeString(int rateOfChange = 0)
        {
			string newString = $"<size=27><color=#3af826>";

			newString += rateOfChange >= 0 ? "+" : "-";
			newString += $"{StringUtils.GetShortenedNumberAsString(rateOfChange)}/HR</size></color>";

            return newString;
        }

		private void Awake()
		{
			//_initializedTechnologyTree = false;
			//_showingTechTreePanel = false;
			//_techTreePanel.SetActive(_showingTechTreePanel);
		}

		private void Start()
		{
			_gm = GameManager.Instance;
			_seaonSliderStartOffset = 0;
            _townGoalInterface = GetComponent<UserInterface_TownGoal>();
            _rulerVoteInterface = GetComponent<UserInterface_RulerVote>();
            _townVoteInterface = GetComponent<UserInterface_TownVote>();
            _eventInterface = GetComponent<UserInterface_Event>();
            _gm.UIManager = this;
		}

		private void Update()
		{
			UpdateResourcesDisplay();
            UpdateResourcesRateOfChange();
            UpdateCountTexts();
			UpdateSeasonSlider();
			UpdateTimeOfDay();
		}
	}
}