using Character;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem.Events.Voting
{
	/// <summary>
	/// An event used for handling any type of voting needed for the game.
	/// </summary>
	public class VoteEvent : GameEvent
	{
		protected float _addedTime = 0;
		protected VoteOption _winningOption;
		protected Dictionary<string, VoteOption> _options;
		protected Dictionary<Player, PlayerVote> _playerVotes;

		protected float _timePassed = 0;
		public Dictionary<string, VoteOption> Options => _options;
		public VoteOption WinningOption => _winningOption;
		public VoteEvent(double delay, double eventDuration, EventType eventType = EventType.Vote, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout)
		{
			_options = new Dictionary<string, VoteOption>();
			_playerVotes = new Dictionary<Player, PlayerVote>();
			_alwaysReturnSuccess = true;
		}

		protected override void OnActioned(object data = null)
		{
			PlayerVote vote = data as PlayerVote;
			AddVote(vote);
		}

		public bool HasVoted(Player player)
		{
			return _playerVotes.ContainsKey(player);
		}

		public override void Update()
		{
			base.Update();
			if (_playerVotes.Count == 0)
			{
				_addedTime += Time.deltaTime;
				//_eventDuration += Time.deltaTime;
				_eventStartTime += Time.deltaTime;
			}
			else
				_timePassed += Time.deltaTime;
		}

		/// <summary>
		/// Adds an option to the voting event.
		/// </summary>
		/// <param name="option"></param>
		public void AddOption(VoteOption option)
		{
			if (_options.ContainsKey(option.OptionName))
			{
				Debug.LogError($"{option.OptionName} already exists");
				return;
			}

			_options.Add(option.OptionName, option);
		}

		protected override void OnStopped() => CalculateWinningVote();

		/// <summary>
		/// Checks if the chosen option is a valid option for this vote.
		/// </summary>
		/// <param name="vote"></param>
		/// <returns></returns>
		protected virtual bool CheckOptionIsValid(PlayerVote vote) => _options.ContainsKey(vote.VoteOption.OptionName);

		/// <summary>
		/// Adds a player's vote the tallies.
		/// </summary>
		/// <param name="vote"></param>
		private void AddVote(PlayerVote vote)
		{
			if (_playerVotes.ContainsKey(vote.Player))
			{
				Debug.Log($"'{vote.Player.TwitchUser.Username}' Already Voted");
				return;
			}

			if (!CheckOptionIsValid(vote))
			{
				Debug.Log($"'{vote.VoteOption.OptionName}' Was not a Valid Option");
				return;
			}

			_options[vote.VoteOption.OptionName].Votes++;
			_playerVotes.Add(vote.Player, vote);
			OnVoteAdded(vote);
		}

		protected virtual void OnVoteAdded(PlayerVote vote) { }
		/// <summary>
		/// Calculates the winning vote.
		/// </summary>
		private void CalculateWinningVote()
		{
			VoteOption bestOption = null;

			foreach (KeyValuePair<string, VoteOption> option in _options)
			{
				if (bestOption == null || bestOption.Votes < option.Value.Votes)
					bestOption = option.Value;
			}

			_winningOption = bestOption;
			_returnData = bestOption;
		}
	}
}