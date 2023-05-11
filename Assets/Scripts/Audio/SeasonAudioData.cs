using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Audio 
{
	[CreateAssetMenu(fileName = "SeasonAudioData", menuName = "ScriptableObjects/SeasonAudioData", order = 1)]
	public class SeasonAudioData : ScriptableObject
	{
		public Season Season;
		public List<AudioClip> DayMusicTracks;
		public List<AudioClip> NightMusicTracks;
		public List<AudioClip> DayAmbienceTracks;
		public List<AudioClip> NightAmbienceTracks;

		public bool GetRandomDayMusicTrack(out AudioClip clip) => GetRandomAudioClipFromList(out clip, ref DayMusicTracks);
		public bool GetRandomNightMusicTrack(out AudioClip clip) => GetRandomAudioClipFromList(out clip, ref NightMusicTracks);
		public bool GetRandomDayAmbienceTrack(out AudioClip clip) => GetRandomAudioClipFromList(out clip, ref DayAmbienceTracks);
		public bool GetRandomNightAmbienceTrack(out AudioClip clip) => GetRandomAudioClipFromList(out clip, ref NightAmbienceTracks);

		private bool GetRandomAudioClipFromList(out AudioClip clip, ref List<AudioClip> list)
		{
			clip = null;

			if (list == null || list.Count == 0)
				return false;

			clip = list[Random.Range(0, list.Count)];
			return true;
		}
    }
}