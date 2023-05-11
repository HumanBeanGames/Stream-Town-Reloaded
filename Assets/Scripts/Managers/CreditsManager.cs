using UnityEngine;
using UnityEngine.InputSystem;
using UserInterface.MainMenu;

namespace Managers 
{
    public class CreditsManager : MonoBehaviour 
	{
		private LoadingManager _loadingManager;

		public void SkipCredits()
		{
			_loadingManager.LoadNonWorldScenes(1);
		}

		private void Awake()
		{
			_loadingManager = FindObjectOfType<LoadingManager>();
		}

		private void Update()
		{
			if(Keyboard.current.escapeKey.wasReleasedThisFrame)
				SkipCredits();
		}
	}
}