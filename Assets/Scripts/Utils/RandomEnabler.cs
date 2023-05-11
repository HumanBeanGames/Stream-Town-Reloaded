using System.Collections;
using UnityEngine;

namespace Utils 
{
    public class RandomEnabler : MonoBehaviour 
	{
		[SerializeField]
		private float _minTime;

		[SerializeField]
		private float _maxTime;

		[SerializeField]
		private GameObject _gameObjectToEnable;
		private IEnumerator EnableObject(float time)
		{
			yield return new WaitForSeconds(time);
			_gameObjectToEnable.SetActive(true);
		}

		private void Awake()
		{
			StartCoroutine(EnableObject(Random.Range(_minTime, _maxTime)));
		}
	}
}