using Character;
using GameEventSystem.Events.Voting;
using Twitch;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameEventSystem.Events
{
	public class EventTester : MonoBehaviour
	{
		private bool _voteStarted = false;

		private void Update()
		{
			GameEventManager.ProcessEvents();
			//Start vote
			//if (Keyboard.current.oKey.wasReleasedThisFrame)
			//{
			//	//VoteEvent ev = new VoteEvent(0, 60);			
			//	//ev.AddOption(new VoteOption("yes", null));
			//	//ev.AddOption(new VoteOption("no", null));
			//	//ev.EventEnded += PrintVoteResults;
			//	//manager.AddEvent(ev);
			//	//ev.AddOption(new VoteOption("playerName", null));

			//	// New King Vote
			//	NewKingVote ev = new NewKingVote(0, 20);
			//	ev.EventEnded += PrintVoteResults;
			//	manager.AddEvent(ev);
			//	Debug.Log("New King Vote Event Added!");
			//}

			//if (Keyboard.current.kKey.wasReleasedThisFrame)
			//{
			//	////Create fake vote
			//	//VoteEvent currentEvent = manager.CurrentEvent as VoteEvent;
			//	//string randomVal = Random.Range(0, 1000).ToString();
			//	//bool yes = Random.Range(0, 2) == 1 ? true : false;
			//	//currentEvent.Action(new PlayerVote(new Player(new TwitchUser(randomVal, randomVal)), new VoteOption(yes ? "yes" : "no", null)));

			//	// Fish God
			//	if(manager.CurrentEvent == null)
			//	{
			//		FishGodEvent fishgodEvent = new FishGodEvent(0);
			//		manager.AddEvent(fishgodEvent);
			//	}
			//	else
			//	{
			//		FishGodEvent ev = manager.CurrentEvent as FishGodEvent;
			//		ev.Action();
			//	}
			//}
		}

		private void PrintVoteResults(bool b, GameEvent.EventType t, object data)
		{
			if(data == null)
			{
				Debug.Log($"No Votes Found");
				return;

			}
			VoteOption voteOption = data as VoteOption;

			Debug.Log($"Winning Vote: '{voteOption.OptionName}' with {voteOption.Votes}");
		}
	}
}