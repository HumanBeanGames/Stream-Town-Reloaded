using Managers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	[GameManager]
	public static class AudioSourcesManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static AudioSourcesConfig Config = AudioSourcesConfig.Instance;

        [HideInInspector]
        private static Queue<AudioHandler> _audioHandlers = new Queue<AudioHandler>();
        [HideInInspector]
        private static int _updatesPerTick = 30;

		public static void Initialize()
		{
			_audioHandlers.Clear();
		}

		internal static void ProcessSources()
		{
			for (int i = 0; i < _updatesPerTick && i < _audioHandlers.Count; i++)
			{
				AudioHandler audioHandler = _audioHandlers.Dequeue();
				if (audioHandler.enabled)
				{
					audioHandler.UpdateLogic();
					_audioHandlers.Enqueue(audioHandler);
				}
				else
					audioHandler.Tracked = false;
			}
		}

		public static void AddSourceToQueue(AudioHandler handler)
		{
			_audioHandlers.Enqueue(handler);
			handler.Tracked = true;
		}
	}
}