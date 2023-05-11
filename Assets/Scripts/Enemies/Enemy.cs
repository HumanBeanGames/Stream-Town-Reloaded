using GameResources;
using GUIDSystem;
using Managers;
using Sensors;
using System;
using Units;
using UnityEngine;
using Utils;

namespace Enemies
{
	/// <summary>
	/// Base class for all Enemy Units in the game.
	/// </summary>
	public class Enemy : MonoBehaviour
	{
		[SerializeField]
		private EnemyType _enemyType;
		[SerializeField]
		private float _additionalHealthPerPlayer = 0.05f;
		private HealthHandler _healthHandler;
		private TargetSensor _targetSensor;
		private GUIDComponent _gUIDComponent;
		private StationSensor _stationSensor;

		private ActiveResourceIncrementer _activeResourceIncrementer;

		public HealthHandler HealthHandler => _healthHandler;
		public TargetSensor TargetSensor => _targetSensor;
		public GUIDComponent GUIDComponent => _gUIDComponent;
		public StationSensor StationSensor => _stationSensor;
		public Action<Enemy> OnDied;

		public EnemyType EnemyType => _enemyType;
		/// <summary>
		/// Called when the enemy is pooled.
		/// </summary>
		public void OnPooled()
		{
		}

		/// <summary>
		/// Initializes all required data and components.
		/// </summary>
		private void Init()
		{
			if (_healthHandler)
				return;

			_healthHandler = GetComponent<HealthHandler>();
			_activeResourceIncrementer = GetComponent<ActiveResourceIncrementer>();
			_targetSensor = GetComponent<TargetSensor>();
			_gUIDComponent = GetComponent<GUIDComponent>();
		}

		// Unit Events.
		private void Awake()
		{
			Init();
		}

		private void Start()
		{
			_healthHandler.OnDeath += OnDeath;
		}

		private void OnEnable()
		{
			Init();
			if (_healthHandler.BaseMaxHealth <= 0)
				return;

			_healthHandler.SetMaxHealth(_healthHandler.BaseMaxHealth + (int)(_additionalHealthPerPlayer * (GameManager.Instance.PlayerManager.PlayerCount() + GameManager.Instance.PlayerManager.RecruitCount())));
			_healthHandler.SetHealth(_healthHandler.MaxHealth);
		}

		private void OnDeath(bool killedByPlayer)
		{
			if (killedByPlayer)
			{
				EventManager.EnemyKilled?.Invoke(_enemyType);
				_activeResourceIncrementer.Increment();
			}
			OnDied?.Invoke(this);
		}
	}
}