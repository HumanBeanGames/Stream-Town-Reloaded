using Managers;
using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Audio 
{
    public class SimpleMusicController : MonoBehaviour 
	{
		[SerializeField]
		private AudioClip _winterTrack;
		[SerializeField]
		private AudioClip _autumnTrack;
		[SerializeField]
		private AudioClip _summerTrack;
		[SerializeField]
		private AudioClip _springTrack;

		[SerializeField]
		private float _maxVolume;
		[SerializeField]
		private float _volumeChangeRate = 0.2f;

		private AudioSource _audioSource;

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			SeasonManager.OnSeasonChanging += OnSeasonChange;
		}

		private void OnSeasonChange(Season season)
		{
			switch (season)
			{
				case Season.Summer:
					StartCoroutine(LerpAudio(_summerTrack));
					break;
				case Season.Autumn:
					StartCoroutine(LerpAudio(_autumnTrack));
					break;
				case Season.Winter:
					StartCoroutine(LerpAudio(_winterTrack));
					break;
				case Season.Spring:
					StartCoroutine(LerpAudio(_springTrack));
					break;
			}
		}

		private IEnumerator LerpAudio(AudioClip nextClip)
		{
			yield return StartCoroutine(VolumeToZero());

			_audioSource.clip = nextClip;
			_audioSource.Play();

			yield return StartCoroutine(VolumeToFull());
		}

		private IEnumerator VolumeToZero()
		{
			while(_audioSource.volume > 0)
			{
				_audioSource.volume -= Time.deltaTime * _volumeChangeRate;
				if (_audioSource.volume < 0)
					_audioSource.volume = 0;

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator VolumeToFull()
		{
			while(_audioSource.volume < _maxVolume)
			{
				_audioSource.volume += Time.deltaTime * _volumeChangeRate;

				if (_audioSource.volume > _maxVolume)
					_audioSource.volume = _maxVolume;

				yield return new WaitForEndOfFrame();
			}
		}
	}
}