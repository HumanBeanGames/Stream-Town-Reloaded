using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utils 
{
    public class SimpleEventOnStart : MonoBehaviour 
	{
		public List<UnityEvent> Events;

		private void Start()
		{
			for (int i = 0; i < Events.Count; i++)
				Events[i]?.Invoke();
		}
	}
}