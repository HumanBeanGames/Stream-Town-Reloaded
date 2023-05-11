using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UserInterface;

namespace GameEventSystem.Events.Voting
{
	public class KeepKingVote : VoteEvent
	{
		private List<UI_RulerOption> _trackedOptions = new List<UI_RulerOption>();
		private UserInterface_RulerVote _rulerVoteInterface;

		public KeepKingVote(double delay, double eventDuration, EventType eventType = EventType.KeepKingVote, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout)
		{
			_alwaysReturnSuccess = true;
			_rulerVoteInterface = GameManager.Instance.UIManager.RulerVoteInterface;

			_options.Add("yes", new VoteOption("yes", null));
			_options.Add("no", new VoteOption("no", null));
			InitializeOptions();
		}

		private void InitializeOptions()
		{
			var ui = _rulerVoteInterface.AddOption("no");
			ui.TextTMP.text = "!vote No";
			_trackedOptions.Add(ui);
			ui = _rulerVoteInterface.AddOption("yes");
			ui.TextTMP.text = "!vote Yes";
			_trackedOptions.Add(ui);
		}

		protected override void OnVoteAdded(PlayerVote vote)
		{
			UpdateOptions();
		}

		protected override void OnStarted()
		{
			base.OnStarted();
			_rulerVoteInterface.ActivateRulerContainer();
			_rulerVoteInterface.DescriptionTMP.text = "Keep the ruler?";
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

		private void UpdateOptions()
		{
			List<VoteOption> optionsSorted = new List<VoteOption>();

			foreach (var v in _options)
			{
				optionsSorted.Add(v.Value);
			}

			optionsSorted = optionsSorted.OrderByDescending(x => x.Votes).ToList();

			List<UI_RulerOption> rulerOptions = new List<UI_RulerOption>();

			if (_rulerVoteInterface.Options.Count <= 0)
				return;

			foreach (var v in _rulerVoteInterface.Options)
				rulerOptions.Add(v.Key);

			for (int i = 0; i < rulerOptions.Count; i++)
				rulerOptions[i].TextTMP.text = $"!vote {optionsSorted[i].OptionName} ({optionsSorted[i].Votes})";
		}

	}
}