using UnityEngine;
using UnityEngine.InputSystem;
using UserInterface.MainMenu;

public class CreditsManager : MonoBehaviour 
{
    private LoadingManager _loadingManager;

    public void SkipCredits()
    {
        _loadingManager.LoadNonWorldScenes(1);
    }

    private void Awake()
    {
        _loadingManager = FindAnyObjectByType<LoadingManager>();
    }

    private void Update()
    {
        if(Keyboard.current.escapeKey.wasReleasedThisFrame)
            SkipCredits();
    }
}