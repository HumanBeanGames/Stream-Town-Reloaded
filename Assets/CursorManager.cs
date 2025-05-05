using Managers;
using UnityEngine;


public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private GameObject _uiGameMenu;
    private GameObject _loadingScreen;

    private bool _lastShouldShowCursor = false;
    private bool _wasIdleLastFrame = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _loadingScreen = GameObject.Find("LoadingScreen");
    }

    void Update()
    {
        if (_uiGameMenu == null)
        {
            _uiGameMenu = GameObject.FindWithTag("GameMenu");
            if (_uiGameMenu == null)
                return; // still not ready, skip this frame
        }

        bool isIdle = GameManager.Instance != null && GameManager.Instance.IdleMode;
        bool loadingScreenActive = _loadingScreen != null && _loadingScreen.activeInHierarchy;
        bool uiMenuActive = _uiGameMenu.activeSelf;

        bool shouldShowCursor = (!isIdle || uiMenuActive) && !loadingScreenActive;

        // Only change if state actually changed
        if (shouldShowCursor != _lastShouldShowCursor)
        {
            Cursor.visible = shouldShowCursor;
            Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;

            if (shouldShowCursor)
                Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
            else
                Cursor.SetCursor(null, Vector2.zero, cursorMode);

            _lastShouldShowCursor = shouldShowCursor;
        }
    }
}
