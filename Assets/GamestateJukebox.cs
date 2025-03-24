using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Managers;
using Utils;

public class GamestateJukebox : MonoBehaviour
{
    [Header("Folder inside Resources")]
    [SerializeField] private string resourceFolder = "SoundTracks";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambienceSource;

    [Header("Volume Settings")]
    [SerializeField] private float _maxMusicVolume = 1f;
    [SerializeField] private float _maxAmbienceVolume = 1f;
    [SerializeField] private float _volumeChangeRate = 0.2f;

    [Header("Timing")]
    [SerializeField] private float _minTimeBetweenMusic = 600;
    [SerializeField] private float _maxTimeBetweenMusic = 900;
    [SerializeField] private float _fadeOutTime = 10;
    [SerializeField] private bool _playAmbienceDuringMusic = true;

    private Dictionary<Vector3, AudioClip[]> soundtrackMap = new();
    private AudioClip lastPlayedClip;
    private float _timeUntilMusicPlays = 30;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadSoundtracks();
    }

    IEnumerator Start()
    {
        SetVolume(_ambienceSource, _maxAmbienceVolume, false);
        SetVolume(_musicSource, 0.0f, true);

        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.SeasonManager != null);

        GameManager.Instance.SeasonManager.OnSeasonChanging += _ => StartNextTrack();
        GameManager.Instance.DayNightManager.OnDayStarting += () => StartCoroutine(DayTimeChangeRoutine(true));
        GameManager.Instance.DayNightManager.OnNightStarting += () => StartCoroutine(DayTimeChangeRoutine(false));
    }

    void Update()
    {
        if (_timeUntilMusicPlays > 0)
        {
            _timeUntilMusicPlays -= Time.deltaTime;
        }
        else
        {
            StartNextTrack();
            _ambienceSource.Play();
            _musicSource.Play();
        }
    }

    void LoadSoundtracks()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(resourceFolder);
        Debug.Log("Clips loaded");
        var tempDict = new Dictionary<Vector3, List<AudioClip>>();

        foreach (var clip in clips)
        {
            string name = clip.name;

            string season = seasonMap.Keys.FirstOrDefault(s => name.Contains(s));
            string trend = trendMap.Keys.FirstOrDefault(t => name.Contains(t));
            string combat = combatMap.Keys.FirstOrDefault(c => name.Contains(c));

            if (season == null || combat == null)
            {
                Debug.LogWarning($"Skipping clip '{name}' — missing season or combat keyword.");
                continue;
            }

            if (trend == null) trend = "Neutral";

            Vector3 key = new Vector3(
                seasonMap[season],
                combatMap[combat],
                trendMap[trend]
            );

            if (!tempDict.ContainsKey(key))
                tempDict[key] = new List<AudioClip>();

            tempDict[key].Add(clip);
        }

        soundtrackMap = tempDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        int totalClips = soundtrackMap.Values.Sum(arr => arr.Length);
        Debug.Log($"Loaded {totalClips} soundtrack clips from '{resourceFolder}'.");

        // ✅ Now that loading is done, start music
        StartCoroutine(StartMusic());
    }


    public AudioClip GetClipFromGamestate(float? season = null, float? combat = null, float? trend = null)
    {
        if (soundtrackMap.Count == 0)
        {
            Debug.LogWarning("Soundtrack map is empty.");
            return null;
        }

        Vector3 query = new Vector3(
            season ?? Random.value,
            combat ?? Random.value,
            trend ?? Random.value
        );

        Vector3 closest = soundtrackMap.Keys
            .OrderBy(v => Vector3.SqrMagnitude(v - query))
            .First();

        AudioClip[] candidates = soundtrackMap[closest];
        if (candidates == null || candidates.Length == 0) return null;

        AudioClip[] filtered = candidates.Length > 1
            ? candidates.Where(c => c != lastPlayedClip).ToArray()
            : candidates;

        AudioClip chosen = filtered[Random.Range(0, filtered.Length)];
        lastPlayedClip = chosen;
        return chosen;
    }

    private void StartNextTrack() => StartCoroutine(StartMusic());

    private IEnumerator DayTimeChangeRoutine(bool day)
    {
        yield return StopMusic();
        StartNextTrack();
    }

    private IEnumerator StartMusic()
    {
        if (!_playAmbienceDuringMusic)
            yield return VolumeToZero(_ambienceSource);

        AudioClip clip = GetStartupClip();

        if (clip == null)
        {
            Debug.LogError("No suitable music track found.");
            yield break;
        }

        _musicSource.clip = clip;
        SetVolume(_musicSource, 0f, true); // ✅ Start silent
        _musicSource.Play();

        float fadeInDelay = 5.0f;
        // Force a short delay to wait for Unity to properly start audible output
        yield return new WaitForSeconds(fadeInDelay);

        UpdateTimeUntilMusicPlays();
        yield return VolumeToFull(_musicSource, true); // ✅ Fade in to max
        yield return new WaitForSeconds(_musicSource.clip.length - _fadeOutTime);
        yield return VolumeToZero(_musicSource);

        if (!_playAmbienceDuringMusic)
        {
            _ambienceSource.Play();
            yield return VolumeToFull(_ambienceSource, false);
        }
    }

    private AudioClip GetStartupClip()
    {
        var validKeys = soundtrackMap.Keys
            .Where(v => v.y == 0.0f && v.z > 0.0f) // y = combat (0 = Peace), z = trend (> 0 = Growth or Neutral)
            .ToList();

        if (validKeys.Count == 0)
        {
            Debug.LogWarning("No suitable peaceful non-decline tracks found for startup. Falling back to full pool.");
            return GetClipFromGamestate(); // fallback to full method
        }

        Vector3 chosenKey = validKeys[Random.Range(0, validKeys.Count)];
        AudioClip[] candidates = soundtrackMap[chosenKey];

        if (candidates == null || candidates.Length == 0)
            return null;

        AudioClip[] filtered = candidates.Length > 1
            ? candidates.Where(c => c != lastPlayedClip).ToArray()
            : candidates;

        AudioClip chosen = filtered[Random.Range(0, filtered.Length)];
        lastPlayedClip = chosen;
        return chosen;
    }

    private void SetVolume(AudioSource source, float value, bool isMusic)
    {
        float max = isMusic ? _maxMusicVolume : _maxAmbienceVolume;
        source.volume = Mathf.Clamp(value, 0f, max);
    }

    private IEnumerator StopMusic()
    {
        StopCoroutine("StartMusic");
        yield return VolumeToZero(_musicSource);
    }

    private void UpdateTimeUntilMusicPlays()
    {
        _timeUntilMusicPlays += Random.Range(_minTimeBetweenMusic, _maxTimeBetweenMusic) + _musicSource.clip.length;
    }

    private IEnumerator VolumeToZero(AudioSource source)
    {
        bool isMusic = source == _musicSource;

        while (source.volume > 0)
        {
            SetVolume(source, source.volume - Time.deltaTime * _volumeChangeRate, isMusic);
            yield return null;
        }

        SetVolume(source, 0f, isMusic); // Ensure clean snap to 0
    }

    private IEnumerator VolumeToFull(AudioSource source, bool isMusic)
    {
        float target = isMusic ? _maxMusicVolume : _maxAmbienceVolume;

        while (source.volume < target)
        {
            SetVolume(source, source.volume + Time.deltaTime * _volumeChangeRate, isMusic);
            yield return null;
        }

        SetVolume(source, target, isMusic); // Ensure clean snap to target
    }

    private float GetSeasonAsFloat(Season season) => season switch
    {
        Season.Spring => 0.0f,
        Season.Summer => 0.25f,
        Season.Autumn => 0.5f,
        Season.Winter => 0.75f,
        _ => 0f
    };

    // Maps for parsing filenames only
    private static readonly Dictionary<string, float> seasonMap = new()
    {
        { "Spring", 0.0f },
        { "Summer", 0.25f },
        { "Autumn", 0.5f },
        { "Winter", 0.75f }
    };

    private static readonly Dictionary<string, float> trendMap = new()
    {
        { "Growth", 1.0f },
        { "Neutral", 0.5f },
        { "Decline", 0.0f }
    };

    private static readonly Dictionary<string, float> combatMap = new()
    {
        { "Combat", 1.0f },
        { "Peace", 0.0f }
    };
}
