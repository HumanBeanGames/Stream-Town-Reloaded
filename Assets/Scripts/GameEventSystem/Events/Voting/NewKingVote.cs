using Managers;
using System.Collections.Generic;
using UserInterface;
using System.Linq;
using System;

namespace GameEventSystem.Events.Voting
{
	public class NewKingVote : VoteEvent
	{
		private const int MAX_TRACKED_OPTIONS = 5;
		private List<UI_RulerOption> _trackedOptions = new List<UI_RulerOption>();
		private UserInterface_RulerVote _rulerVoteInterface;

		public NewKingVote(double delay, double eventDuration, EventType eventType = EventType.NewKingVote, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout)
		{
			_alwaysReturnSuccess = true;
			_rulerVoteInterface = GameManager.Instance.UIManager.RulerVoteInterface;
		}

		protected override bool CheckOptionIsValid(PlayerVote vote)
		{
			string optionName = vote.VoteOption.OptionName;

			if (!_options.ContainsKey(vote.VoteOption.OptionName))
			{
				if (GameManager.Instance.PlayerManager.PlayerExistsByNameToLower(optionName, out int index))
				{
					vote.VoteOption.Votes = 0;
					_options.Add(optionName, vote.VoteOption);
					OnOptionAdded(vote);
				}
				else
					return false;
			}

			return true;
		}

		private void OnOptionAdded(PlayerVote vote)
		{
			if (_options.Count >= MAX_TRACKED_OPTIONS)
			{
				UpdateOptions();
				return;
			}

			var ui = _rulerVoteInterface.AddOption(vote.VoteOption.OptionName);

			ui.TextTMP.text = $"{vote.VoteOption.OptionName} ({vote.VoteOption.Votes})";
			_trackedOptions.Add(ui);
		}

		protected override void OnStarted()
		{
			base.OnStarted();
			_rulerVoteInterface.ActivateRulerContainer();
			_rulerVoteInterface.DescriptionTMP.text = "Who should be Ruler? \n type !vote playername";
		}

		protected override void OnStopped()
		{
			base.OnStopped();
			_rulerVoteInterface.DisableRulerContainer();
		}

		public override void Update()
		{
			base.Update();
			float val = 1 - (float)((_timePassed) / (EventDuration));
			_rulerVoteInterface.TimerSlider.value = val;
			TimeSpan timespan = TimeSpan.FromSeconds(Math.Ceiling(EventDuration - _timePassed));
			_rulerVoteInterface.TimerTMP.text = $"{string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds)}";
		}

		protected override void OnVoteAdded(PlayerVote vote)
		{
			UpdateOptions();
		}

		private void UpdateOptions()
		{
			List<VoteOption> optionsSorted = new List<VoteOption>();

			foreach (var v in _options)
			{
				optionsSorted.Add(v.Value);
			}

			optionsSorted = optionsSorted.OrderByDescending(x => x.Votes).ToList();
			UserInterface_RulerVote uiManager = GameManager.Instance.UIManager.RulerVoteInterface;

			List<UI_RulerOption> rulerOptions = new List<UI_RulerOption>();

			if (uiManager.Options.Count <= 0)
				return;

			foreach (var v in uiManager.Options)
				rulerOptions.Add(v.Key);

			for (int i = 0; i < MAX_TRACKED_OPTIONS && i < rulerOptions.Count && i < optionsSorted.Count; i++)
			{
				rulerOptions[i].TextTMP.text = $"{optionsSorted[i].OptionName} ({optionsSorted[i].Votes})";
			}
		}
	}
}