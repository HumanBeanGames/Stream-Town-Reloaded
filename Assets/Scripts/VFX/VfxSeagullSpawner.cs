using UnityEngine;

namespace VFX 
{
    public class VfxSeagullSpawner : MonoBehaviour 
	{
		[SerializeField]
		private GameObject _seagull;
		[SerializeField]
		private float _movementTime;
		[SerializeField]
		private SpawnArea[] _spawnAreas;
		[SerializeField]
		private float _height;

		[SerializeField]
		private AudioClip[] _clips;

		private AudioSource _audioSource;

		private Vector3 _startPosition;
		private Vector3 _endPosition;
		
		private float _time;
		private int _currentArea;
		private float _audioTime;
		private float _audioTimer;

		[System.Serializable]
		class SpawnArea
		{
			public Vector3 _position;
			public Vector3 _Area;
		}

		private void RandomPosition()
		{
			int randomArea = new int();
			randomArea = Random.Range(0, _spawnAreas.Length);
			if(randomArea == _currentArea)
			{
				if(_currentArea != 3)
					randomArea++;
				else
					randomArea--;
			}
			_startPosition =_spawnAreas[randomArea]._position + new Vector3(Random.Range(-_spawnAreas[randomArea]._Area.x, _spawnAreas[randomArea]._Area.x),0, Random.Range(-_spawnAreas[randomArea]._Area.z, _spawnAreas[randomArea]._Area.z)) / 2;
			_startPosition.y = _height;
			_endPosition = -_startPosition;
			_endPosition.y = _height;
		}

		private void PlayRandomAudioSound()
		{
			_audioTime = Random.Range(1.0f, 5.0f);
			_audioSource.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
			_audioTimer = 0;
		}

		private void Awake()
		{
			RandomPosition();
			_audioSource = GetComponentInChildren<AudioSource>();
		}

		private void Update()
		{
			_audioTimer += Time.deltaTime;
			if (_audioTimer > _audioTime)
			{
				PlayRandomAudioSound();
			}
			if (Vector3.Distance(_seagull.transform.position, _endPosition) > 1)
			{
				_time += Time.deltaTime;

				_seagull.transform.position = Vector3.Lerp(_startPosition, _endPosition, _time / _movementTime);
				_seagull.transform.LookAt(_endPosition);
			}
			else
			{
				_time = 0;
				RandomPosition();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(1,0,0,1);
			if(_spawnAreas.Length > 0)
			{
				for (int i = 0; i < _spawnAreas.Length; i++)
				{
					Gizmos.DrawWireCube(new Vector3(_spawnAreas[i]._position.x, _height, _spawnAreas[i]._position.z), _spawnAreas[i]._Area);
				}
			}

			if (_endPosition != Vector3.zero)
				Gizmos.DrawSphere(_endPosition, 1);
		}

	}
}