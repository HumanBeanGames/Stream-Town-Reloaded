using Character;
using GameEventSystem;
using Managers;
using System;
using TwitchLib.Client.Events;

namespace Twitch.Commands
{
	public static class BroadcasterCommands
	{
		internal static void Connect(string arg, OnChatCommandReceivedArgs e)
		{
#if UNITY_EDITOR
			MessageSender.MessagesAllowed = true;
#else
			if (arg == GameManager.Instance.Code && e.Command.ChatMessage.IsBroadcaster)
				MessageSender.MessagesAllowed = true;
			else
				return;
#endif
			GameManager.Instance.CodeDisplay.text = "";
			GameManager.Instance.ConnectPanel.SetActive(false);
			TechTreeManager.StartCoroutine(TechTreeManager.DelayedSetup());
			GameEventManager.CanStartNewRulerVote = true;
			if (GameManager.Instance.MetaDatas.LoadType == MetaData.LoadType.Generate || GameManager.Instance.MetaDatas.LoadType == MetaData.LoadType.Load && GameManager.Instance.UserPlayer == null)
			{
                PlayerCommands.TryCreatePlayer(e, out Player newPlayer);
                GameManager.Instance.SetUserPlayer(newPlayer);
            }
		}
	}
}