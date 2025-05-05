using Managers;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControls
{
    [GameManager]
    public static class PlayerInputManager
    {
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static PlayerInputConfig Config = PlayerInputConfig.Instance;

        public enum Button
        {
            None,
            LeftMouse,
            RightMouse,
            MiddleMouse,
            Count
        }

        [HideInInspector]
        private static PlayerInput _playerInput = new PlayerInput();
        [HideInInspector]
        private static Dictionary<Button, bool> _heldKeys = new Dictionary<Button, bool>();
        public static bool IsButtonHeld(Button button) => _heldKeys[button];

        public static event Action<Button> OnLeftClickPress;
        public static event Action<Button> OnLeftClickHold;
        public static event Action<Button> OnLeftClickRelease;

        public static event Action<Button> OnRightClickPress;
        public static event Action<Button> OnRightClickHold;
        public static event Action<Button> OnRightClickRelease;

        public static event Action<Button> OnMiddleClickPress;
        public static event Action<Button> OnMiddleClickHold;
        public static event Action<Button> OnMiddleClickRelease;

        public static event Action<float> OnMouseScroll;
        public static event Action<Vector2> OnMousePosition;
        public static event Action OnEscape;
        public static event Action OnBuildMenu;
        public static event Action OnTechTree;
        public static event Action OnRecruit;

        // Temp
        public static event Action OnSaveGame;
        public static event Action OnLoadGame;
        public static event Action OnGenerateGame;
        //
        [HideInInspector]
        private static Vector2 _mouseLastClickPosition = Vector2.zero;
        [HideInInspector]
        private static Vector2 _previousMousePos = Vector2.zero;
        [HideInInspector]
        private static Vector2 _mouseDelta = Vector2.zero;
        [HideInInspector]
        private static Vector2 _mousePosition = Vector2.zero;

        [HideInInspector]
        private static readonly Queue<Action> _deferredInputQueue = new();

        public static Vector2 MousePosition => _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
        public static Vector2 MousePositionDelta => _mouseDelta;
        public static Vector2 MouseLastClickPosition => _mouseLastClickPosition;
        public static bool EscapePressed => _playerInput.BasicControls.Escape.ReadValue<float>() > 0.01f;

        private class Runner : MonoBehaviour {
            private void OnDestroy()
            {
                _heldKeys.Clear();
            }

            private void OnEnable()
            {
                EnableFunctionality();
            }

            private void OnDisable()
            {
                DisableFunctionality();
            }
        }
        [HideInInspector]
        private static Runner runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Awake();

            GameObject runnerObject = new GameObject("PlayerInputManagerRunner");
            runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            runner.StartCoroutine(UpdateEachFrame());
        }

        //MUST DO SOMETHING HERE
        private static void Awake()
        {
            _playerInput.BasicControls.MouseLeftClick.started += ctx =>
                _deferredInputQueue.Enqueue(() => OnLeftClickPress?.Invoke(Button.LeftMouse));
            _playerInput.BasicControls.MouseLeftClick.performed += ctx =>
                _deferredInputQueue.Enqueue(() => OnLeftClickHold?.Invoke(Button.LeftMouse));
            _playerInput.BasicControls.MouseLeftClick.canceled += ctx =>
                _deferredInputQueue.Enqueue(() => OnLeftClickRelease?.Invoke(Button.LeftMouse));

            _playerInput.BasicControls.MouseRightClick.started += ctx =>
                _deferredInputQueue.Enqueue(() => OnRightClickPress?.Invoke(Button.RightMouse));
            _playerInput.BasicControls.MouseRightClick.performed += ctx =>
                _deferredInputQueue.Enqueue(() => OnRightClickHold?.Invoke(Button.RightMouse));
            _playerInput.BasicControls.MouseRightClick.canceled += ctx =>
                _deferredInputQueue.Enqueue(() => OnRightClickRelease?.Invoke(Button.RightMouse));

            _playerInput.BasicControls.MouseMiddleClick.started += ctx =>
                _deferredInputQueue.Enqueue(() => OnMiddleClickPress?.Invoke(Button.MiddleMouse));
            _playerInput.BasicControls.MouseMiddleClick.performed += ctx =>
                _deferredInputQueue.Enqueue(() => OnMiddleClickHold?.Invoke(Button.MiddleMouse));
            _playerInput.BasicControls.MouseMiddleClick.canceled += ctx =>
                _deferredInputQueue.Enqueue(() => OnMiddleClickRelease?.Invoke(Button.MiddleMouse));

            _playerInput.BasicControls.MouseScroll.performed += ctx =>
            {
                float delta = ctx.ReadValue<Vector2>().y;
                _deferredInputQueue.Enqueue(() => OnMouseScroll?.Invoke(delta));
            };

            _playerInput.BasicControls.MousePosition.performed += ctx =>
                _deferredInputQueue.Enqueue(() => OnMousePosition?.Invoke(ctx.ReadValue<Vector2>()));

            _playerInput.BasicControls.Escape.started += ctx => _deferredInputQueue.Enqueue(() => OnEscape?.Invoke());
            _playerInput.BasicControls.BuildMenu.started += ctx => _deferredInputQueue.Enqueue(() => OnBuildMenu?.Invoke());
            _playerInput.BasicControls.TechTree.started += ctx => _deferredInputQueue.Enqueue(() => OnTechTree?.Invoke());
            _playerInput.BasicControls.Recruit.started += ctx => _deferredInputQueue.Enqueue(() => OnRecruit?.Invoke());

            _playerInput.BasicControls.TempGenerateWorld.started += ctx => _deferredInputQueue.Enqueue(() => OnGenerateGame?.Invoke());
            _playerInput.BasicControls.TempLoadWorld.started += ctx => _deferredInputQueue.Enqueue(() => OnLoadGame?.Invoke());
            _playerInput.BasicControls.TempSaveWorld.started += ctx => _deferredInputQueue.Enqueue(() => OnSaveGame?.Invoke());

            _previousMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();

            for (int i = 0; i < (int)Button.Count; i++)
                _heldKeys.Add((Button)i, false);
        }

        //MUST DO A RUNNER HERE
        private static IEnumerator UpdateEachFrame()
        {
            while (true)
            {
                Vector2 currentMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
                _mouseDelta = _previousMousePos - currentMousePos;

                while (_deferredInputQueue.Count > 0)
                {
                    var action = _deferredInputQueue.Dequeue();
                    action?.Invoke();
                }

                //THIS WAS ORIGINALLY IN LATEUPDATE; WORRIED IT MAY NOT WORK PROPERLY.
                _previousMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
                yield return new WaitForEndOfFrame();
            }
        }

        private static void SetClickPosition(Button button)
        {
            _mouseLastClickPosition = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
        }

        //Hoping this loads everything after each scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnableFunctionality()
        {
            _playerInput?.Enable();

            OnLeftClickPress += ButtonPressed;
            OnLeftClickHold += ButtonHeld;
            OnLeftClickRelease += ButtonRelease;
            OnLeftClickPress += SetClickPosition;

            OnRightClickPress += ButtonPressed;
            OnRightClickHold += ButtonHeld;
            OnRightClickRelease += ButtonRelease;

            OnMiddleClickPress += ButtonPressed;
            OnMiddleClickHold += ButtonHeld;
            OnMiddleClickRelease += ButtonRelease;
        }

        //And this clears it after each scene finishes
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DisableFunctionality()
        {
            _playerInput?.Disable();

            OnLeftClickPress -= ButtonPressed;
            OnLeftClickHold -= ButtonHeld;
            OnLeftClickRelease -= ButtonRelease;
            OnLeftClickPress -= SetClickPosition;

            OnRightClickPress -= ButtonPressed;
            OnRightClickHold -= ButtonHeld;
            OnRightClickRelease -= ButtonRelease;

            OnMiddleClickPress -= ButtonPressed;
            OnMiddleClickHold -= ButtonHeld;
            OnMiddleClickRelease -= ButtonRelease;
        }

        private static void ButtonPressed(Button button = Button.None) => _heldKeys[button] = true;
        private static void ButtonHeld(Button button = Button.None) => _heldKeys[button] = true;
        private static void ButtonRelease(Button button = Button.None) => _heldKeys[button] = false;
    }
}