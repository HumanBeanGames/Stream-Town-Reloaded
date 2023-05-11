using UnityEngine;

namespace Utils
{
	[ExecuteInEditMode]
	public class SimpleRandomModelEnabled : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _models;

		private void OnEnable()
		{
			if (_models == null || _models.Length <= 1)
				return;

			for (int i = 0; i < _models.Length; i++)
			{
				_models[i].SetActive(false);
			}

			_models[Random.Range(0, _models.Length)].SetActive(true);
		}
	}
}