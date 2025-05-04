using Managers;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private GameObject _uiGameMenu;
    private GameObject _loadingScreen;
    private bool cursorSet = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _loadingScreen = GameObject.Find("LoadingScreen");
    }

    void Update()
    {
        // Refresh reference to UI_GameMenu if it was dynamically instantiated
        if (_uiGameMenu == null)
            _uiGameMenu = GameObject.Find("UI_GameMenu");

        bool isIdle = GameManager.Instance != null && GameManager.Instance.IdleMode;
        bool loadingScreenActive = _loadingScreen != null && _loadingScreen.activeInHierarchy;

        // If we can't find UI_GameMenu yet, assume it's active
        bool uiMenuActive = _uiGameMenu == null || _uiGameMenu.activeSelf;

        bool shouldShowCursor = (!isIdle || uiMenuActive) && !loadingScreenActive;

        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;

        if (shouldShowCursor && !cursorSet)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
            cursorSet = true;
        }
        else if (!shouldShowCursor && cursorSet)
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
            cursorSet = false;
        }
    }
}
