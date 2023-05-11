using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControls
{
	public class PlayerInputManager : MonoBehaviour
	{
		public enum Button
		{
			None,
			LeftMouse,
			RightMouse,
			MiddleMouse,
			Count
		}

		private static PlayerInput _playerInput;

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

		private static Vector2 _mouseLastClickPosition = Vector2.zero;
		private static Vector2 _previousMousePos = Vector2.zero;
		private static Vector2 _mouseDelta = Vector2.zero;
		private static Vector2 _mousePosition = Vector2.zero;

		public static Vector2 MousePosition
		{
			get { return _playerInput.BasicControls.MousePosition.ReadValue<Vector2>(); }
		}

		public static Vector2 MousePositionDelta => _mouseDelta;
		public static Vector2 MouseLastClickPosition => _mouseLastClickPosition;
		public static bool EscapePressed
		{
			get { return _playerInput.BasicControls.Escape.ReadValue<float>() > 0.01f ? true : false; }
		}

		/// <summary>
		/// Called when script is loaded
		/// </summary>
		private void Awake()
		{
			Debug.Log("Awake" + this);
			_playerInput = new PlayerInput();

			_playerInput.BasicControls.MouseLeftClick.started += ctx => OnLeftClickPress?.Invoke(Button.LeftMouse);     // Started is called on button clicked
			_playerInput.BasicControls.MouseLeftClick.performed += ctx => OnLeftClickHold?.Invoke(Button.LeftMouse);    // Performed is called on button hold
			_playerInput.BasicControls.MouseLeftClick.canceled += ctx => OnLeftClickRelease?.Invoke(Button.LeftMouse);  // Cancelled is caled on button release

			_playerInput.BasicControls.MouseRightClick.started += ctx => OnRightClickPress?.Invoke(Button.RightMouse);
			_playerInput.BasicControls.MouseRightClick.performed += ctx => OnRightClickHold?.Invoke(Button.RightMouse);
			_playerInput.BasicControls.MouseRightClick.canceled += ctx => OnRightClickRelease?.Invoke(Button.RightMouse);

			_playerInput.BasicControls.MouseMiddleClick.started += ctx => OnMiddleClickPress?.Invoke(Button.MiddleMouse);
			_playerInput.BasicControls.MouseMiddleClick.performed += ctx => OnMiddleClickHold?.Invoke(Button.MiddleMouse);
			_playerInput.BasicControls.MouseMiddleClick.canceled += ctx => OnMiddleClickRelease?.Invoke(Button.MiddleMouse);

			_playerInput.BasicControls.MouseScroll.started += ctx => OnMouseScroll?.Invoke(ctx.ReadValue<Vector2>().y);
			_playerInput.BasicControls.MouseScroll.canceled += ctx => OnMouseScroll?.Invoke(ctx.ReadValue<Vector2>().y);

			_playerInput.BasicControls.MousePosition.performed += ctx => OnMousePosition?.Invoke(ctx.ReadValue<Vector2>());

			//_playerInput.BasicControls.Escape.started += ctx => OnEscape?.Invoke();

			_playerInput.BasicControls.BuildMenu.started += ctx => OnBuildMenu?.Invoke();

			_playerInput.BasicControls.TechTree.started += ctx => OnTechTree?.Invoke();

			_playerInput.BasicControls.Recruit.started += ctx => OnRecruit?.Invoke();

			_previousMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();

			// Temp
			_playerInput.BasicControls.TempGenerateWorld.started += ctx => OnGenerateGame?.Invoke();
			_playerInput.BasicControls.TempLoadWorld.started += ctx => OnLoadGame?.Invoke();
			_playerInput.BasicControls.TempSaveWorld.started += ctx => OnSaveGame?.Invoke();
			//

			// Add all keys to dictionary
			for (int i = 0; i < (int)Button.Count; i++)
			{
				Debug.Log("Add ");	
				_heldKeys.Add((Button)i, false);
			}
		}

		/// <summary>
		/// Runs whenever a button is pressed
		/// </summary>
		/// <param name="button">The button that is being pressed</param>
		private void ButtonPressed(Button button = Button.None)
		{
			//Debug.Log("Pressed:" + button.ToString());
		}

		/// <summary>
		/// Runs whenever a buton is being held
		/// </summary>
		/// <param name="button">The button that is being held</param>
		private void ButtonHeld(Button button = Button.None)
		{
			//Debug.Log("Holding: " + button.ToString());
			_heldKeys[button] = true;
		}

		/// <summary>
		/// Runs whenever a button is released
		/// </summary>
		/// <param name="button">The button that is being released</param>
		private void ButtonRelease(Button button = Button.None)
		{
			//Debug.Log("Released: " + button.ToString());
			_heldKeys[button] = false;
		}

		private void Update()
		{
			Vector2 currentMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
			_mouseDelta = _previousMousePos - currentMousePos;
		}

		private void LateUpdate()
		{
			_previousMousePos = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
		}

		/// <summary>
		/// Sets the click position
		/// </summary>
		/// <param name="button"></param>
		private void SetClickPosition(Button button)
		{
			_mouseLastClickPosition = _playerInput.BasicControls.MousePosition.ReadValue<Vector2>();
			//Debug.Log(_mouseLastClickPosition);
		}

		private void OnEnable()
		{
			_playerInput?.Enable();

			PlayerInputManager.OnLeftClickPress += ButtonPressed;
			PlayerInputManager.OnLeftClickHold += ButtonHeld;
			PlayerInputManager.OnLeftClickRelease += ButtonRelease;
			PlayerInputManager.OnLeftClickPress += SetClickPosition;

			PlayerInputManager.OnRightClickPress += ButtonPressed;
			PlayerInputManager.OnRightClickHold += ButtonHeld;
			PlayerInputManager.OnRightClickRelease += ButtonRelease;

			PlayerInputManager.OnMiddleClickPress += ButtonPressed;
			PlayerInputManager.OnMiddleClickHold += ButtonHeld;
			PlayerInputManager.OnMiddleClickRelease += ButtonRelease;
		}

		private void OnDestroy()
		{
			Debug.Log("Detroyed: " + this);
			_heldKeys.Clear();
		}
		private void OnDisable()
		{
			_playerInput?.Disable();

			PlayerInputManager.OnLeftClickPress -= ButtonPressed;
			PlayerInputManager.OnLeftClickHold -= ButtonHeld;
			PlayerInputManager.OnLeftClickRelease -= ButtonRelease;
			PlayerInputManager.OnLeftClickPress -= SetClickPosition;

			PlayerInputManager.OnRightClickPress -= ButtonPressed;
			PlayerInputManager.OnRightClickHold -= ButtonHeld;
			PlayerInputManager.OnRightClickRelease -= ButtonRelease;

			PlayerInputManager.OnMiddleClickPress -= ButtonPressed;
			PlayerInputManager.OnMiddleClickHold -= ButtonHeld;
			PlayerInputManager.OnMiddleClickRelease -= ButtonRelease;
		}
	}
}