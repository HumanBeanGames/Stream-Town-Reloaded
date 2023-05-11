using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System.Collections;
using Character;
using Managers;
using Pets;
using Pets.Enumerations;
using System;
using UnityEngine.InputSystem;
using GameEventSystem.Events;

namespace Twitch
{
	/// <summary>
	/// Handles connection to Twitch and reads events triggered by the API.
	/// </summary>
	public class TL_Client : MonoBehaviour
	{
		public static Client Client;

		private string _channelName = "";

		/// <summary>
		/// Initalizes the Twitch Lib Client and connect the bot to the Twitch Channel.
		/// </summary>
		public void InitClient()
		{
			Application.runInBackground = true;

			// Set up our bot and tell it which channel to join
			ConnectionCredentials credentials = new ConnectionCredentials(TL_Secrets.BotName, TL_Secrets.BotAccessToken);
			Client = new Client();

            _channelName = GameManager.Instance.SettingsData.channelName.ToLower();

			Client.Initialize(credentials, _channelName);
			Client.AddChatCommandIdentifier('!');
			// Subscribe to any events here.
			Client.OnMessageReceived += OnMessageReceived;
			Client.OnChatCommandReceived += OnChatCommandReceived;
			Client.OnNewSubscriber += OnNewSubscriber;
			Client.OnGiftedSubscription += OnGiftedSubscription;
			Client.OnPrimePaidSubscriber += OnPrimePaidSubscriber;
			Client.OnReSubscriber += OnReSubscriber;
			Client.OnContinuedGiftedSubscription += OnContinuedGiftedSubscription;
			Client.OnCommunitySubscription += OnCommunitySubscription;
			Client.OnRaidNotification += OnRaidNotification;
			// Connect bot to channel
			Client.Connect();

			if (Client.IsConnected)
				Debug.Log("Twitch Connected");
			else
				Debug.Log("Twitch Failed to Connect!");
		}

		private void OnRaidNotification(object sender, OnRaidNotificationArgs e)
		{
			if (int.TryParse(e.RaidNotification.MsgParamViewerCount, out int viewerCount))
			{
				string[] enemies = new string[] { "Minotaur" };
				GameManager.Instance.GameEventManager.AddEvent(new RaidEvent(0, 1200, enemies, boss: "MinotaurBoss", waves: 2, enemiesPerWave: viewerCount));
			}
		}

		private void OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
		{
			UserIsSubscribed(e.GiftedSubscription.UserId);
		}

		private void OnContinuedGiftedSubscription(object sender, OnContinuedGiftedSubscriptionArgs e)
		{
			UserIsSubscribed(e.ContinuedGiftedSubscription.UserId);
		}

		private void OnReSubscriber(object sender, OnReSubscriberArgs e)
		{
			UserIsSubscribed(e.ReSubscriber.UserId);
		}

		private void OnPrimePaidSubscriber(object sender, OnPrimePaidSubscriberArgs e)
		{
			UserIsSubscribed(e.PrimePaidSubscriber.UserId);
		}

		private void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
		{
			UserIsSubscribed(e.GiftedSubscription.MsgParamRecipientId);
		}

		/// <summary>
		/// Called when a Chat Command has been received.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			TwitchChatManager.ProcessCommand(e);
		}

		/// <summary>
		/// Called when a Chat Message has been receieved.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			//Debug.Log($"Message | {e.ChatMessage.Username}: {e.ChatMessage.Message} | Id: {e.ChatMessage.UserId}");
			TwitchChatManager.ProcessMessage(e);
		}

		private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
		{
			UserIsSubscribed(e.Subscriber.UserId);
		}

		public static void UserIsSubscribed(string userId)
		{
			if (GameManager.Instance.PlayerManager.PlayerExistsByID(userId, out int playerIndex))
			{
				Player player = GameManager.Instance.PlayerManager.GetPlayer(playerIndex);
				player.PetsUnlocked[PetType.RedPanda] = true;

				if (player.Pet.ActivePet == null)
					player.Pet.TrySetActivePet(PetType.RedPanda);
			}
		}

		private void Start()
		{
			//TODO:: Move this to GameManager
			InitClient();
			StartCoroutine(SendPing());
		}

		private void Awake()
		{
			Application.logMessageReceived += OnErrorReceived;
		}

		private void Update()
		{
			if (Keyboard.current.f2Key.wasReleasedThisFrame)
				ForceConnection();

			if (Keyboard.current.f1Key.wasReleasedThisFrame)
				ForceDisconnect();
		}

		private IEnumerator SendPing()
		{
			for (; ; )
			{
				Client.SendRaw("PING");
				if (Client == null || !Client.IsConnected || Client.JoinedChannels.Count == 0)
				{
					ForceDisconnect();
					InitClient();
				}
				yield return new WaitForSeconds(30);
			}
		}

		private void OnErrorReceived(string logString, string stackTrace, LogType type)
		{
			//if (type == LogType.Error || type == LogType.Exception)
			//{
			//	MessageSender.SendMessage($"/w Uniquisher {logString} ");
			//	MessageSender.SendMessage($"/w Uniquisher {stackTrace}");
			//}
		}

		public static void ForceDisconnect()
		{
			if (Client != null)
			{
				Client.Disconnect();
                MessageSender.MessagesAllowed = false;

                if (Client.IsConnected)
					Debug.Log("Twitch Connected");
				else
					Debug.Log("Twitch Not Connected!");

				Client = null;
			}
		}

		public void ForceConnection()
		{
			InitClient();
		}
	}
}