using Managers;
using UnityEngine;
using Utils;
using System.Collections;
using World;
using UnityEngine.EventSystems;

namespace PlayerControls
{
	public class CameraController : MonoBehaviour
	{
		[Header("Camera Constraints")]

		[SerializeField]
		private float _maxCameraHeight = 30.0f;

		[SerializeField]
		private float _minCameraHeight = 15.0f;

		[SerializeField]
		private Vector2 _minimumCameraPosition = Vector2.zero;

		[SerializeField]
		private Vector2 _maximumCameraPosition = Vector2.zero;

		[Header("Pan")]

		[SerializeField]
		private bool _canPan = true;

		[SerializeField]
		private float _panSensitivity = 1.0f;

		[SerializeField]
		private float _panSmoothness = 0.5f;

		[Header("Zoom")]

		[SerializeField]
		private bool _canZoom = true;

		[SerializeField]
		private float _zoomSenitivity = 1.0f;

		[SerializeField]
		private float _zoomSmoothness;

		[Header("Move")]

		[SerializeField]
		private bool _canMove = true;

		[SerializeField]
		private bool _borderDetection;

		[SerializeField]
		private bool _mouseControls;

		[SerializeField]
		private int _edgeSize = 5;

		[SerializeField]
		private float _wasdSenitivity = 1.0f;

		[SerializeField]
		private float _borderDetectionSenitivity = 20.0f;

		[SerializeField]
		private float _moveSmoothness;

		[Header("Command Settings")]

		[SerializeField]
		private float _moveTime = 2f;

		//Private variables
		private GameManager _gameManager;

		private Transform _transform = null;

		private Camera _camera = null;

		private SelectableObject _target = null;

		private PlayerInput _playerInput;

		private float _scrollWheelInput = 0.0f;
		private float _scrollPosition = 15.0f;
		private float _transitionTime = 0.0f;

		private bool _autofollow = false;
		private bool _isPanning;
		private bool _movingWithMouse;
		private bool _isIdle = false;

		private Vector2 _keyboardInput;

		private Vector3 _oldMovePosition;
		private Vector3 _newMovePosition;
		private Vector3 _startPosDuringMovement;
		private Vector3 _movePos;

		public Vector3 StartPosition { get; private set; }
		public bool BorderDetection
		{
			get { return _borderDetection; }
			set { _borderDetection = value; }
		}
		public bool MouseControls
		{
			get { return _mouseControls; }
			set { _mouseControls = value; }
		}
		public float PanSensitivity
		{
			get { return _panSensitivity; }
			set { _panSensitivity = value; }
		}
		public float ZoomSensitivity
		{
			get { return _zoomSenitivity; }
			set { _zoomSenitivity = value; }
		}
		public float WasdSensitivity
		{
			get { return _wasdSenitivity; }
			set { _wasdSenitivity = value; }
		}
		public float BorderDetectionSensitivity
		{
			get { return _borderDetectionSenitivity; }
			set { _borderDetectionSenitivity = value;}
		}
		public bool IsIdle
		{
			get { return _isIdle; }
			set { _isIdle = value; }
		}

		/// <summary>
		/// Runs everything for the camera to update
		/// </summary>
		private void UpdateCamera()
		{
			if (!_isIdle)
			{
				Pan();
				Move();
			}

			Zoom();
		}

		/// <summary>
		/// Pans by physically moving the camera on the X/Z - axis
		/// </summary>
		private void Pan()
		{
			if (_canPan && MouseControls)
			{
				if (PlayerInputManager.IsButtonHeld(PlayerInputManager.Button.MiddleMouse))
				{
					_isPanning = true;
					_autofollow = false;

					StopCoroutine(SmoothCameraMovement());

					Vector3 movementVec = new Vector3(PlayerInputManager.MousePositionDelta.y, 0, -PlayerInputManager.MousePositionDelta.x);
					movementVec *= _panSensitivity;
					Vector3 movementVecLerped = Vector3.Lerp(new Vector3(0, 0, 0), movementVec, Time.fixedUnscaledDeltaTime * _panSmoothness);
					_transform.position += movementVecLerped;
					_transform.position = MoveConstraints(_transform.position);

					_oldMovePosition = _transform.position;
					_newMovePosition = _transform.position;
					_movePos = transform.position;
				}
				else
				{
					_isPanning = false;
				}
			}
			else
			{
				_isPanning = false;
			}
		}

		/// <summary>
		/// Zooms by phisically moving the camera on the Y - axis
		/// </summary>
		private void Zoom()
		{
			if (_canZoom)
			{
				Vector3 direction = Vector3.zero;
				Vector3 oldPos = _transform.position;
				if (_target != null && _autofollow)
					direction = (_transform.position - _target.transform.position).normalized;
				else
					direction = Vector3.up;

				_scrollPosition += _scrollWheelInput * _zoomSenitivity * 0.004f;
				_scrollPosition = Mathf.Clamp(_scrollPosition, _minCameraHeight, _maxCameraHeight);

				Vector3 movementVecLerped = Vector3.Lerp(_transform.position, new Vector3(0, _scrollPosition, 0), Time.fixedUnscaledDeltaTime * _zoomSmoothness);
				movementVecLerped = new Vector3(_transform.position.x, movementVecLerped.y, transform.position.z);
				_transform.position = movementVecLerped;

				_scrollWheelInput = 0.0f;
			}
		}

		private void Move()
		{
			if (_canMove && !_isPanning)
			{
				if (Application.isFocused)
				{
					if (_mouseControls && _borderDetection)
					{
						Vector2 mouseInput = new Vector2();
						if (PlayerInputManager.MousePosition.x > Screen.width - _edgeSize)
						{
							mouseInput.x = 1;
							_movingWithMouse = true;
						}
						else if (PlayerInputManager.MousePosition.x < _edgeSize)
						{
							mouseInput.x = -1;
							_movingWithMouse = true;
						}
						else
						{
							mouseInput.x = 0;
							_movingWithMouse = false;
						}

						if (PlayerInputManager.MousePosition.y > Screen.height - _edgeSize)
						{
							mouseInput.y = 1;
							_movingWithMouse = true;
						}
						else if (PlayerInputManager.MousePosition.y < _edgeSize)
						{
							mouseInput.y = -1;
							_movingWithMouse = true;
						}
						else
						{
							mouseInput.y = 0;
							_movingWithMouse = false;
						}
						_movePos += new Vector3(mouseInput.y, 0, -mouseInput.x).normalized * _borderDetectionSenitivity * Time.fixedUnscaledDeltaTime * 2;
						_movePos = MoveConstraints(_movePos);

						Vector3 movementVecLerped = Vector3.Lerp(_transform.position, _movePos, Time.fixedUnscaledDeltaTime * _moveSmoothness);
						movementVecLerped.y = _transform.position.y;

						_transform.position = movementVecLerped;
					}
					else
					{
						_movingWithMouse = false;
					}

					if (!_movingWithMouse)
					{
						float zoomOutBoost = _scrollPosition / (_maxCameraHeight - _minCameraHeight);
						zoomOutBoost += 1;
						zoomOutBoost = Mathf.Pow(zoomOutBoost, zoomOutBoost);
						_movePos += new Vector3(_keyboardInput.y, 0, -_keyboardInput.x) * (zoomOutBoost *_wasdSenitivity) * Time.fixedUnscaledDeltaTime * 2;
						_movePos = MoveConstraints(_movePos);

						Vector3 movementVecLerped = Vector3.Lerp(_transform.position, _movePos, Time.fixedUnscaledDeltaTime * _moveSmoothness);
						movementVecLerped.y = _transform.position.y;

						_transform.position = movementVecLerped;
					}
				}
			}
		}

		private IEnumerator SmoothCameraMovement()
		{
			Vector3 newPos = new Vector3();

			while (_transitionTime / _moveTime < 1)
			{
				_transitionTime += Time.fixedUnscaledDeltaTime;

				newPos = Vector3.Lerp(_startPosDuringMovement, _newMovePosition, Easings.EaseInOutCubic(Mathf.Clamp(_transitionTime / _moveTime, 0.0f, 1.0f)));
				newPos.y = _transform.position.y;
				_movePos = _transform.position;
				_transform.position = newPos;

				yield return null;
			}
			_transform.position = new Vector3(_newMovePosition.x, _transform.position.y, _newMovePosition.z);
		}

		/// <summary>
		/// Sets the target position for orbiting
		/// </summary>
		/// <param name="button"></param>
		private void SetTarget(PlayerInputManager.Button button)
		{
			Debug.Log("Clicked");
			if (Physics.Raycast(_camera.ScreenPointToRay(PlayerInputManager.MousePosition), out RaycastHit hitInfo, float.MaxValue))
			{
				SelectableObject obj = hitInfo.transform.GetComponentInChildren<SelectableObject>();
				if (obj != null)
				{
					_target = obj;
					_gameManager.SelectionManager.OnObjectSelected.Invoke(_target, _target.Data);
					_autofollow = true;
				}
				else
					_gameManager.SelectionManager.HideUI();
			}
		}

		private void UpdateScrollWheelInput(float value)
		{
			if (!_isIdle && !WorldUtils.IsPointerOverUI(EventSystem.current) && _mouseControls)
			{
				_scrollWheelInput = -value;
			}
		}

		/// <summary>
		/// Applies constraints to the cameras movement
		/// </summary>

		private Vector3 MoveConstraints(Vector3 movement)
		{
			// Camera XZ position constraints
			if (movement.x < _minimumCameraPosition.x)
				movement = new Vector3(_minimumCameraPosition.x, movement.y, movement.z);
			if (_minimumCameraPosition.y > movement.z)
				movement = new Vector3(movement.x, movement.y, _minimumCameraPosition.y);

			if (_maximumCameraPosition.x < movement.x)
				movement = new Vector3(_maximumCameraPosition.x, movement.y, movement.z);
			if (_maximumCameraPosition.y < movement.z)
				movement = new Vector3(movement.x, movement.y, _maximumCameraPosition.y);
			return movement;
		}

		/// <summary>
		/// Command Movements
		/// </summary>

		public void MoveCamera(Vector3 moveVec)
		{
			if (_isIdle)
			{
				_oldMovePosition = new Vector3(_newMovePosition.x, 0, _newMovePosition.z);
				_newMovePosition = _oldMovePosition + moveVec;
				_startPosDuringMovement = _transform.position;

				_transitionTime = 0;

				_newMovePosition = MoveConstraints(_newMovePosition);

				StopCoroutine(SmoothCameraMovement());
				StartCoroutine(SmoothCameraMovement());
			}
		}

		public void ZoomCamera(int zoomFactor)
		{
			if (_isIdle)
			{
				_scrollPosition += zoomFactor;
				_scrollPosition = Mathf.Clamp(_scrollPosition, _minCameraHeight, _maxCameraHeight);
			}
		}

		public void ResetCamera()
		{
			if (_isIdle)
			{
				_newMovePosition = StartPosition;
				_oldMovePosition = StartPosition;
				_scrollPosition = 20f;
				_transform.position = StartPosition;
			}
		}

		private void Awake()
		{
			GameManager.Instance.CameraController = this;
			_playerInput = new PlayerInput();

			if (!TryGetComponent(out _transform))
				Debug.LogError("CameraController: missing transform component" + this);  // should never occur

			if (!TryGetComponent(out _camera))
				Debug.LogError("CameraController: missing camera component" + this);

			if (_maxCameraHeight - _minCameraHeight < 0)
				Debug.LogError("CameraController: MaxCameraHeight is lower than MinCameraHeight" + this);

			if (_maximumCameraPosition.x - _minimumCameraPosition.x < 0)
				Debug.LogError("CameraController: MaximumCameraPosition.x is lower than MinimumCamerPosition.x" + this);

			if (_maximumCameraPosition.y - _minimumCameraPosition.y < 0)
				Debug.LogError("CameraController: MaximumCameraPosition.y is lower than MinimumCamerPosition.y" + this);
			if (!_gameManager)
				_gameManager = GameManager.Instance;

			StartPosition = transform.position;

			if (_gameManager)
			{
				_panSensitivity = _gameManager.SettingsData.panSensitivity;
				_zoomSenitivity = _gameManager.SettingsData.zoomSensitivity;
				_wasdSenitivity = _gameManager.SettingsData.wasdSensitivity;
			}
			else
				Debug.LogError("No game manager presetned in scene");

			_playerInput.BasicControls.KeyboardMovement.performed += ctx => _keyboardInput = ctx.ReadValue<Vector2>();
			_playerInput.BasicControls.KeyboardMovement.canceled += ctx => _keyboardInput = Vector2.zero;

			_newMovePosition = transform.position;
			_oldMovePosition = transform.position;
			_scrollPosition = 15.0f;
			_movePos = transform.position;
			this.enabled = false;
		}

		private void OnEnable()
		{
			PlayerInputManager.OnMouseScroll += UpdateScrollWheelInput;
			_playerInput.Enable();
		}

		private void OnDisable()
		{
			PlayerInputManager.OnMouseScroll -= UpdateScrollWheelInput;
			_playerInput.Disable();
		}

		private void Update()
		{
			UpdateCamera();
		}
	}
}