using Character;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
	public class SimpleCancelBuildingPlacer : MonoBehaviour
	{
		[SerializeField]
		private bool _resetOnEnable = true;
		[SerializeField]
		private float _timeInSeconds = 300;

		private float _timer = 0;

		private Player _player;

		public void SetPlayer(Player player)
		{
			_player = player;
		}

		public void ResetTimer()
		{
			_timer = _timeInSeconds;
		}


		private void Update()
		{
			_timer -= Time.deltaTime;

			if (_timer <= 0)
			{
				BuildingManager.TryCancelBuilding(_player);
				ResetTimer();
			}
		}

		private void OnEnable()
		{
			ResetTimer();
		}
	}
}