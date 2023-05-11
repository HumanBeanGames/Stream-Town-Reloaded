using Managers;
using System;
using System.Collections.Generic;
using TechTree.ScriptableObjects;
using UnityEngine;
using UserInterface;

namespace GameEventSystem.Events.Voting
{
	public class TechVote : VoteEvent
	{
		private Dictionary<UI_TechOption, VoteOption> _trackedOptions;
		private UserInterface_TownVote _townVoteInterface;

		public TechVote(double delay, double eventDuration, Node_SO[] nodes, EventType eventType = EventType.TechVote, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout)
		{
			_townVoteInterface = GameManager.Instance.UIManager.TownVoteInterface;

			_trackedOptions = new Dictionary<UI_TechOption, VoteOption>();
			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodes[i] == null)
					continue;

				VoteOption newOption = new VoteOption($"{i + 1}", nodes[i]);
				_options.Add($"{i + 1}", newOption);
				_trackedOptions.Add(_townVoteInterface.AddOption(nodes[i], i + 1), newOption);
			}
		}

		protected override void OnVoteAdded(PlayerVote vote)
		{
			base.OnVoteAdded(vote);
			UpdateOptions();
		}

		protected override void OnStarted()
		{
			base.OnStarted();
			_townVoteInterface.ActivateVoteContainer();
			_townVoteInterface.SetupButtons();
		}

		protected override void OnStopped()
		{
			base.OnStopped();
			_townVoteInterface.DeactivateVoteContainer();
		}

		public override void Update()
		{
			base.Update();
			float val = 1 - (float)((_timePassed) / (EventDuration));
			_townVoteInterface.TimerSlider.value = val;
			TimeSpan timespan = TimeSpan.FromSeconds(Math.Ceiling(EventDuration - _timePassed));
			_townVoteInterface.TimerTMP.text = $"{string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds)}";
		}

		private void UpdateOptions()
		{
			foreach (var option in _trackedOptions)
			{
				float percentage = option.Value.Votes / (float)Mathf.Max(1, _playerVotes.Count);
				option.Key.VotesSlider.value = percentage;
				option.Key.VotesAmountTMP.text = $"{Mathf.Floor(percentage * 100)}% ({option.Value.Votes})";
			}
		}

	}
}