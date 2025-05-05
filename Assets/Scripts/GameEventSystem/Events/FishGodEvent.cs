using Character;
using Managers;
using Pets.Enumerations;
using System.Collections;
using Twitch;
using UnityEngine;
using UserInterface;

namespace GameEventSystem.Events
{
	public class FishGodEvent : GameEvent
	{
		private int _praisesRequired = 20;
		private int _praisesGiven = 0;
		private GameObject _fishGod;
		private Animator _animator;
		private UserInterface_Event _eventInterface;

		public FishGodEvent(double startTime, double eventDuration = 300, EventType eventType = EventType.FishGod, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(startTime, eventDuration, EventType.FishGod, data, overrideCurrentEvent, timeout)
		{
			GetFishGodGameObject();

			_eventInterface = GameManager.Instance.UIManager.EventInterface;
		}

		protected override void OnStarted()
		{
			GameEventManager.FallingFishVFX.gameObject.SetActive(true);
			_eventInterface.Slider.gameObject.SetActive(true);
			UpdateSlider();
			_eventInterface.TitleTMP.text = "Fish God";
			_eventInterface.DescriptionTMP.text = "Praise the Fish God!";
			_eventInterface.ActivateEventContainer();
		}

		protected override void OnStopped()
		{
			_animator.SetTrigger("Exit");
			GameEventManager.StartCoroutine(DisableAfterTime());
			GameEventManager.FallingFishVFX.gameObject.SetActive(false);
			_eventInterface.DeactivateEventContainer();

			if (Success)
			{
				TownResourceManager.AddResource(Utils.Resource.Food, 1000, true);

				// Try to give a player a fish pet if roll hits
				int roll = Random.Range(0, 100);

				if (roll < 70)
				{
					if (PlayerManager.PlayerCount() <= 0)
						return;
					
					Player player = null;
					int iters = 0;
					do
					{
						iters++;
						if (iters >= 50)
							break;

						int playerIndex = Random.Range(0, PlayerManager.PlayerCount());
						player = PlayerManager.GetPlayer(playerIndex);
						if (player.IsNPC)
							continue;
					}
					while (player.IsNPC);

					if (player == null || player.IsNPC)
						return;

					player.PetsUnlocked[PetType.FishGod] = true;
					MessageSender.SendMessage($"{player.TwitchUser.Username} unlocked the fishgod pet!");

				}
			}
		}

		protected void UpdateSlider()
		{
			_eventInterface.SliderTMP.text = $"{_praisesGiven}  /  {_praisesRequired}";
			_eventInterface.Slider.value = (float)_praisesGiven / _praisesRequired;
		}

		protected override void OnActioned(object data = null)
		{
			_praisesGiven++;

			if (_praisesGiven >= _praisesRequired)
				OnCompleteEvent();

			UpdateSlider();
		}

		private void GetFishGodGameObject()
		{
			_fishGod = ObjectPoolingManager.GetPooledObject("FishGod").gameObject;
			_animator = _fishGod.GetComponentInChildren<Animator>();
			_fishGod.transform.position = GameEventManager.FishGodSpawn.position;
			_fishGod.SetActive(true);
		}

		public IEnumerator DisableAfterTime()
		{
			float time = 2.5f;
			float trackedTime = 0;

			while (trackedTime < time)
			{
				trackedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			_fishGod.SetActive(false);
		}
	}
}