using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	public class AudioSourcesManager : MonoBehaviour
	{
		private Queue<AudioHandler> _audioHandlers = new Queue<AudioHandler>();
		private int _updatesPerTick = 30;
		public void Initialize()
		{
			_audioHandlers.Clear();
		}

		internal void ProcessSources()
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

		public void AddSourceToQueue(AudioHandler handler)
		{
			_audioHandlers.Enqueue(handler);
			handler.Tracked = true;
		}
	}
}