using GameEventSystem;
using GameEventSystem.Events;
using Managers;
using System;
using TwitchLib.Client.Events;

namespace Twitch.Commands 
{
    public static class EventCommands 
	{
        public static bool EventMessage(OnMessageReceivedArgs e)
		{
			string[] words = e.ChatMessage.RawIrcMessage.Split(';');

			if(words.Length >= 3)
			{
				string[] split = words[3].Split('=');

				if(split[0] == "custom-reward-id")
				{
					ProcessReward(split, e);
					return true;
				}
			}

			return false;
		}

		private static void ProcessReward(string[] split, OnMessageReceivedArgs e)
		{
			if (!PlayerManager.PlayerExistsByID(e.ChatMessage.UserId, out int index))
				return;

			switch(split[1])
			{
				// Fish God
				case "5a760033-50b5-4e47-911b-d63993d2860c":
					HandleFishGodEvent();
					break;
			}
		}

		public static void HandleFishGodEvent()
		{
			if(GameEventManager.CurrentEvent != null && GameEventManager.CurrentEvent.Event == GameEvent.EventType.FishGod)
			{
				GameEventManager.CurrentEvent.Action();
				return;
			}

			if (GameEventManager.CurrentEvent != null)
				return;

			int rand = UnityEngine.Random.Range(0, 10);

			if (rand == 0)
				GameEventManager.AddEvent(new FishGodEvent(0));
		}
	}
}