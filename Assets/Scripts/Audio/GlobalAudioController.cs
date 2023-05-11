using Managers;
using System.Collections;
using UnityEngine;
using Utils;

namespace Audio
{
	public class GlobalAudioController : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _musicSource;
		[SerializeField]
		private AudioSource _ambienceSource;
		[SerializeField]
		private float _maxMusicVolume;
		[SerializeField]
		private float _maxAmbienceVolume;
		[SerializeField]
		private float _volumeChangeRate = 0.2f;

		[SerializeField]
		private SeasonAudioData[] _audioData;

		[SerializeField]
		private float _minTimeBetweenMusic = 600;
		[SerializeField]
		private float _maxTimeBetweenMusic = 900;
		[SerializeField]
		private float _fadeOutTime = 10;
		[SerializeField]
		private bool _playAmbienceDuringMusic = true;

		private float _timeUntilMusicPlays = 30;

		private void OnSeasonChange(Season season)
		{
			StartNextTrack(season, true);
		}

		private void OnDayStarted() => StartCoroutine(DayTimeChangeRoutine(true));

		private void OnNightStarted() => StartCoroutine(DayTimeChangeRoutine(false));


		private IEnumerator DayTimeChangeRoutine(bool day)
		{
			yield return StopMusic(GameManager.Instance.SeasonManager.CurrentSeason);
			StartNextTrack(GameManager.Instance.SeasonManager.CurrentSeason, day);
		}

		private void StartNextTrack(Season season, bool day)
		{
			StopCoroutine("StartMusic");
			StartCoroutine(StartMusic(season, day));

			if (_playAmbienceDuringMusic)
			{
				SeasonAudioData data = GetDataBySeason(season);
				if (data.GetRandomDayAmbienceTrack(out AudioClip clip))
					_ambienceSource.clip = clip;
				else
					Debug.LogError($"Couldn't find ambience clip for season '{data.Season}'");
				_ambienceSource.Play();
			}
		}

		private IEnumerator StartMusic(Season season, bool day)
		{
			if (!_playAmbienceDuringMusic)
				StartCoroutine(VolumeToZero(_ambienceSource));
			SeasonAudioData data = GetDataBySeason(season);

			if (day)
			{
				if (data.GetRandomDayMusicTrack(out AudioClip clip))
					_musicSource.clip = clip;
				else
					Debug.LogError($"Couldnt find day music clip for season '{data.Season}'");
			}
			else
			{
				if (data.GetRandomNightMusicTrack(out AudioClip clip))
					_musicSource.clip = clip;
				else
					Debug.LogError($"Couldnt find day music clip for season '{data.Season}'");
			}

			_musicSource.Play();

			UpdateTimeUntilMusicPlays();

			StartCoroutine(VolumeToFull(_musicSource, true));

			yield return new WaitForSeconds(_musicSource.clip.length - _fadeOutTime);

			StartCoroutine(VolumeToZero(_musicSource));

			if (!_playAmbienceDuringMusic)
			{
				AudioClip ambienceClip;

				if (day ? data.GetRandomDayAmbienceTrack(out ambienceClip) : data.GetRandomNightAmbienceTrack(out ambienceClip))
					_ambienceSource.clip = ambienceClip;
				else
					Debug.LogError($"Couldn't find ambience clip for season '{data.Season}'");
				_ambienceSource.Play();


				StartCoroutine(VolumeToFull(_ambienceSource, false));
			}
		}

		private IEnumerator StopMusic(Season season)
		{
			StopCoroutine("StartMusic");
			yield return StartCoroutine(VolumeToZero(_musicSource));
		}

		private void UpdateTimeUntilMusicPlays()
		{
			_timeUntilMusicPlays += Random.Range(_minTimeBetweenMusic, _maxTimeBetweenMusic) + _musicSource.clip.length;
		}

		private IEnumerator VolumeToZero(AudioSource audioSource)
		{
			while (audioSource.volume > 0)
			{
				audioSource.volume -= Time.deltaTime * _volumeChangeRate;
				if (audioSource.volume < 0)
					audioSource.volume = 0;

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator VolumeToFull(AudioSource audioSource, bool music)
		{
			while (audioSource.volume < (music ? _maxMusicVolume : _maxAmbienceVolume))
			{
				audioSource.volume += Time.deltaTime * _volumeChangeRate;

				if (audioSource.volume > (music ? _maxMusicVolume : _maxAmbienceVolume))
					audioSource.volume = (music ? _maxMusicVolume : _maxAmbienceVolume);

				yield return new WaitForEndOfFrame();
			}
		}

		private SeasonAudioData GetDataBySeason(Season season)
		{
			for (int i = 0; i < _audioData.Length; i++)
			{
				if (_audioData[i].Season == season)
					return _audioData[i];
			}

			Debug.LogError($"No audio data found for the season '{season}'");
			return null;
		}

		private void Start()
		{
			GameManager.Instance.SeasonManager.OnSeasonChanging += OnSeasonChange;
			GameManager.Instance.DayNightManager.OnNightStarting += OnNightStarted;
			GameManager.Instance.DayNightManager.OnDayStarting += OnDayStarted;
			StartMusic(GameManager.Instance.SeasonManager.CurrentSeason, true);
			_ambienceSource.volume = _maxAmbienceVolume;
		}

		private void Update()
		{
			if (_timeUntilMusicPlays > 0)
			{
				_timeUntilMusicPlays -= Time.deltaTime;
			}
			else
			{
				StartNextTrack(GameManager.Instance.SeasonManager.CurrentSeason, GameManager.Instance.DayNightManager.IsDayTime);
				_ambienceSource.Play();
				_musicSource.Play();
			}
		}

	}
}