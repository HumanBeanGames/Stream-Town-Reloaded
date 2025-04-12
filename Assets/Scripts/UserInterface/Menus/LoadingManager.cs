using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


namespace UserInterface.MainMenu
{
	public class LoadingManager : MonoBehaviour
	{
		[SerializeField]
		private float _loadingSpeed = 0.5f;

		[SerializeField]
		private GameObject _loadingUI;

		[SerializeField]
		private float _waitTime = 0.5f;

		[SerializeField]
		private TextMeshProUGUI _tooltipText;

		[SerializeField, TextArea]
		private string[] _toolTips;

		private float _loadProgress;

		private void DisableUI()
		{
			_loadingUI.SetActive(false);
			GameStateManager.ReadiedPlayer -= DisableUI;
		}

		public void LoadNonWorldScenes(int sceneIndex)
		{
			Debug.Log("Loading scene " + sceneIndex);
			StartCoroutine(LoadAsyncScene(sceneIndex, false));
		}

		IEnumerator LoadAsyncScene(int sceneIndex, bool loadingWorld)
		{
			RandomizeTooltip();
			_loadingUI.SetActive(true);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
			asyncLoad.allowSceneActivation = false;

			float currentProgress = 0.0f;
			float targetProgress = new float();

			while (currentProgress < 1)
			{
				targetProgress = asyncLoad.progress / 0.9f;
				currentProgress = Mathf.MoveTowards(_loadProgress, targetProgress, _loadingSpeed * Time.deltaTime);
				_loadProgress = currentProgress;
				yield return null;
			}
			Scene scene = new Scene();

			asyncLoad.allowSceneActivation = true;

			while (scene != SceneManager.GetSceneByBuildIndex(sceneIndex))
			{
				scene = SceneManager.GetActiveScene();
				yield return null;
			}
			yield return new WaitForSeconds(_waitTime);

			if (loadingWorld)

				GameStateManager.ReadiedPlayer += DisableUI;
			else
				_loadingUI.SetActive(false);
		}

		public void LoadWorldScene(int sceneIndex)
		{
			StartCoroutine(LoadAsyncScene(sceneIndex, true));
		}

		private void RandomizeTooltip()
		{
			_tooltipText.text = _toolTips[UnityEngine.Random.Range(0,_toolTips.Length)];
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}

