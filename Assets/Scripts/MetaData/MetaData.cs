using UnityEngine;

namespace MetaData
{
	public enum LoadType
	{
		Generate,
		Load,
	}

	public class MetaData : MonoBehaviour
	{
		public LoadType LoadType;

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}
}