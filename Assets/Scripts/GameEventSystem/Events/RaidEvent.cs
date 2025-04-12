using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UserInterface;
using Units;
using Utils;
using Utils.Pooling;

namespace GameEventSystem.Events
{
	public class RaidEvent : GameEvent
	{
		protected string[] _pooledEnemyNames;
		protected string _bossName;

		protected int _waves;
		protected int _currentWave;
		protected bool _bossOnLastWave = true;
		protected int _enemiesPerWave = 50;
		protected List<Enemy> _trackedEnemies;

		protected ObjectPoolingManager _poolingManager;
		private UserInterface_Event _eventInterface;

		protected StationMask _prevEnemyStationMask;
		protected GameEventManager _eventManager;

		protected bool _forceStop = false;

		public RaidEvent(double delay, double eventDuration, string[] enemies, int waves = 5, int enemiesPerWave = 50, string boss = null, EventType eventType = EventType.MonsterRaid, object data = null, bool overrideCurrentEvent = false, double timeout = -1) : base(delay, eventDuration, eventType, data, overrideCurrentEvent, timeout)
		{
			_pooledEnemyNames = enemies;
			_waves = waves;
			_enemiesPerWave = enemiesPerWave;

			if (boss == null)
				_bossOnLastWave = false;
			else
			{
				_bossOnLastWave = true;
				_bossName = boss;

			}

			_poolingManager = GameManager.Instance.PoolingManager;
			_eventInterface = GameManager.Instance.UIManager.EventInterface;
			_eventManager = GameManager.Instance.GameEventManager;

			_trackedEnemies = new List<Enemy>();
		}

		public void SetEnemies(string[] enemies)
		{
			_pooledEnemyNames = enemies;
		}

		protected override void OnStarted()
		{
			_eventInterface.Slider.gameObject.SetActive(true);
			UpdateSlider();
			_eventInterface.TitleTMP.text = "Raid";
			_eventInterface.DescriptionTMP.text = "";
			_eventInterface.ActivateEventContainer();
			GameManager.Instance.GameEventManager.StartCoroutine(HandleWaves());

			GameManager.Instance.EnemySpawner.CanSpawnEnemies = false;
		}

		protected override void OnStopped()
		{
			_forceStop = true;
			GameManager.Instance.GameEventManager.StopCoroutine(HandleWaves());

			for (int i = _trackedEnemies.Count - 1; i >= 0; i--)
			{
				_trackedEnemies[i].HealthHandler.SetHealth(0);
			}

			_eventInterface.DeactivateEventContainer();
			GameManager.Instance.EnemySpawner.CanSpawnEnemies = true;
		}

		public override void Update()
		{
			if (_currentWave < _waves - 1 || !_bossOnLastWave)
				return;
			UpdateSlider();
		}

		protected IEnumerator HandleWaves()
		{
			for (_currentWave = 0; _currentWave < _waves; _currentWave++)
			{
				while (_trackedEnemies.Count > 0)
					yield return new WaitForEndOfFrame();
				SpawnNewWave();
			}
		}

		protected void UpdateSlider()
		{
			if (_currentWave < _waves || !_bossOnLastWave)
			{
				_eventInterface.SliderTMP.text = $"Waves Completed: {_currentWave}/{_waves}";
				_eventInterface.Slider.value = (float)_currentWave / _waves;
			}
			else
			{
				if (_trackedEnemies.Count > 0 && _trackedEnemies[0] != null)
				{
					_eventInterface.SliderTMP.text = $"Boss HP: {_trackedEnemies[0].HealthHandler.Health}/{_trackedEnemies[0].HealthHandler.MaxHealth}";
					_eventInterface.Slider.value = _trackedEnemies[0].HealthHandler.HealthPercentage;
				}
			}
		}

		protected void SpawnNewWave()
		{
			if (_forceStop)
				return;

			// On waves prior to last wave or there is no final boss
			if (_currentWave < _waves - 1 || !_bossOnLastWave)
			{
				for (int i = 0; i < _enemiesPerWave; i++)
				{
					string enemyName = _pooledEnemyNames[Random.Range(0, _pooledEnemyNames.Length)];
					PoolableObject go = _poolingManager.GetPooledObject(enemyName);
					Enemy enemy = go.GetComponent<Enemy>();
					enemy.OnDied += OnEnemyDeath;
					_trackedEnemies.Add(enemy);
					enemy.transform.position = GameManager.Instance.EnemySpawner.GetRandomSpawnLocation().position;
					enemy.gameObject.SetActive(true);
				}
			}
			else // On Last Wave and Should spawn boss.
			{
				PoolableObject go = _poolingManager.GetPooledObject(_bossName);
				Enemy enemy = go.GetComponent<Enemy>();
				enemy.OnDied += OnEnemyDeath;
				_trackedEnemies.Add(enemy);
				enemy.HealthHandler.SetMaxHealth(Mathf.Max(1000, 50 * (GameManager.Instance.PlayerManager.PlayerCount() + GameManager.Instance.PlayerManager.RecruitCount())));
				enemy.gameObject.SetActive(true);
				enemy.transform.position = GameManager.Instance.EnemySpawner.GetRandomSpawnLocation().position;
			}

			UpdateSlider();
			UpdateUI();
		}

		protected void OnEnemyDeath(Enemy enemy)
		{
			enemy.OnDied -= OnEnemyDeath;
			_trackedEnemies.Remove(enemy);

			UpdateUI();

			if (_currentWave == _waves)
				Stop(true);
		}

		protected void CheckComplete()
		{
			if (_currentWave == _waves && _trackedEnemies.Count == 0)
				OnCompleteEvent();
		}

		protected void UpdateUI()
		{
			_eventInterface.DescriptionTMP.text = $"Enemies In Wave: {_trackedEnemies.Count}";
		}
	}
}