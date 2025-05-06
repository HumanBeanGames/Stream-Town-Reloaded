using UnityEngine;
using UnityEngine.Events;
using Utils;
using UserInterface;
using PlayerControls;
using Character;
using Buildings;
using Enemies;
using GameResources;
using System.Collections.Generic;
using Utils.Pooling;
using World;
using UnityEngine.EventSystems;
using Sensors;
using Target;
using SavingAndLoading.SavableObjects;
using PlayerControls.ObjectSelection;
using Sirenix.OdinInspector;

namespace Managers
{
	[GameManager]
	public static class ObjectSelectionManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static ObjectSelectionConfig Config = ObjectSelectionConfig.Instance;

        [SerializeField]
		private static UserInterface_ObjectSelection _selectionUI;

		private static UnityEvent<SelectableObject, object> _onObjectSelected = new UnityEvent<SelectableObject, object>();

		private static bool _startedGroupSelection = false;
		private static bool _startedSelection = false;
		private static Vector3 _startedSelectionPosition = Vector3.zero;
		private static Vector3 _endedSelectionPosition = Vector3.zero;

		private static (SelectableObject, object) _selectedObject;
		private static bool _objectSelected = false;

		private static List<RoleHandler> _selectedPlayerGroup;
		private static bool _groupSelected = false;

		public static UnityEvent<SelectableObject, object> OnObjectSelected => _onObjectSelected;

		private static void ObjectSelected(SelectableObject selected, object data)
		{
			if (_selectionUI == null)
				return;

			_selectedObject.Item1 = selected;
			_selectedObject.Item2 = data;

			_objectSelected = true;
			switch (selected.SelectableType)
			{
				case Selectable.Player:
					_selectionUI.OnCharacterContext((RoleHandler)data);
					break;
				case Selectable.Building:
					_selectionUI.OnBuildingContext((BuildingBase)data);
					break;
				case Selectable.Enemy:
					_selectionUI.OnEnemyContext((Enemy)data);
					break;
				case Selectable.Resource:
					_selectionUI.OnResourceContext((ResourceHolder)data);
					break;
				case Selectable.EnemyCamp:
					_selectionUI.OnEnemyCampContext((Station)data);
					break;
				default:
					SetSelectionFalse();
					break;
			}
		}

		private static void Select(PlayerInputManager.Button button)
		{
			_startedSelection = true;
			if (RayTraceFromCamera(Camera.main, PlayerInputManager.MousePosition, out Vector3 hitPos))
				_startedSelectionPosition = hitPos;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputManager.MousePosition), out RaycastHit hitInfo, float.MaxValue) && !WorldUtils.IsPointerOverUI(EventSystem.current))
			{
				SelectableObject obj = hitInfo.transform.GetComponentInChildren<SelectableObject>();
				if (obj != null)
				{
					if (_selectedObject.Item1 == obj)
					{
						HideUI();
						return;
					}
					OnObjectSelected.Invoke(obj, obj.Data);
				}
				else
					HideUI();

				_selectionUI.DisableCheckUI();
			}
		}

		private static void StartGroupSelect(PlayerInputManager.Button button)
		{
			_startedGroupSelection = true;
			_selectionUI.HideContext();
		}

		public static bool RayTraceFromCamera(Camera cam, Vector2 mousePos, out Vector3 hitPosition)
        {
			Vector3 camRay = cam.ScreenPointToRay(mousePos).direction;
			float t = (0 - cam.transform.position.y) / camRay.y;
			hitPosition = cam.transform.position + (camRay * t);

			return true;
        }

		private static void GroupSelect(PlayerInputManager.Button button)
		{
			if (_startedGroupSelection)
			{
				if (RayTraceFromCamera(Camera.main, PlayerInputManager.MousePosition, out Vector3 hitPos))
					_endedSelectionPosition = hitPos;
				else
					Debug.LogError("This should not be happening" + typeof(ObjectSelectionManager));

				List<PoolableObject> objs = ObjectPoolingManager.GetAllActiveObjectsOfTypeWithinAABB(_startedSelectionPosition, _endedSelectionPosition, "Player");
				List<RoleHandler> roleHandlers = new List<RoleHandler>();
				for (int i = 0; i < objs.Count; i++)
				{
					if (objs[i].PoolType == PoolType.Player)
					{
						SelectableObject selectable = objs[i].transform.GetComponentInChildren<SelectableObject>();
						if (((RoleHandler)selectable.Data).Player.TwitchUser.Username == "")
							roleHandlers.Add((RoleHandler)selectable.Data);
					}
				}
				if (roleHandlers.Count > 1)
				{
					_selectionUI.OnCharacterGroupContext(roleHandlers);
					_selectedPlayerGroup = roleHandlers;
					_groupSelected = true;
					_objectSelected = false;
				}
				else if (roleHandlers.Count == 1)
				{
					_selectionUI.OnCharacterContext(roleHandlers[0]);
					_objectSelected = true;
				}
				else
				{
					_objectSelected = false;
					_groupSelected = false;
				}
				_startedGroupSelection = false;
				_startedSelection = false;

			}
			if (!_objectSelected && !_groupSelected)
			{
				_selectionUI.HideContext();
				_selectionUI.DisableCheckUI();
			}
		}

		public static void HideUI()
		{
			if (_selectionUI == null)
				return;
			_selectedObject.Item1 = null;
			_selectionUI.HideContext();
		}

		public static void SetSelectionFalse()
		{
			_groupSelected = false;
			_objectSelected = false;
		}

		private static void OnEnable()
		{
			PlayerInputManager.OnLeftClickPress += Select;
			PlayerInputManager.OnRightClickPress += OnRightClick;
			PlayerInputManager.OnLeftClickRelease += GroupSelect;
			PlayerInputManager.OnLeftClickHold += StartGroupSelect;
		}

		private static void OnDisable()
		{
			PlayerInputManager.OnLeftClickPress -= Select;
			PlayerInputManager.OnRightClickPress -= OnRightClick;
			PlayerInputManager.OnLeftClickRelease -= GroupSelect;
			PlayerInputManager.OnLeftClickHold -= StartGroupSelect;
		}

		private static void Update()
		{
			if (_startedGroupSelection)
			{
				Vector3 mousePos = Vector3.zero;
				if (RayTraceFromCamera(Camera.main, PlayerInputManager.MousePosition, out Vector3 hitPos ))
					mousePos = hitPos + (Vector3.up * 0.01f);
				else
					Debug.LogError("Cant find mouse point" + typeof(ObjectSelectionManager));

				_selectionUI.SetGroupSelectionArea(_startedSelectionPosition, mousePos);
			}

			if (PlayerInputManager.EscapePressed)
			{
				_selectionUI.HideContext();
			}
		}

		public static void OnRightClick(PlayerInputManager.Button button)
		{

			if (_groupSelected)
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputManager.MousePosition), out RaycastHit hitInfo, float.MaxValue) && !WorldUtils.IsPointerOverUI(EventSystem.current))
				{
					SelectableObject obj = hitInfo.transform.GetComponentInChildren<SelectableObject>();
					if (obj != null)
					{
						switch (obj.Type)
						{
							case Selectable.Building:
								Station station = ((BuildingBase)obj.Data).Station;
								SetGroupStation(_selectedPlayerGroup, station);

								break;
							case Selectable.Enemy:
								Enemy enemy = ((Enemy)obj.Data);
								SetGroupTarget(_selectedPlayerGroup, enemy.GetComponent<Targetable>());

								break;
							case Selectable.Resource:
								ResourceHolder resource = ((ResourceHolder)obj.Data);
								SetGroupTarget(_selectedPlayerGroup, resource.GetComponent<Targetable>());
								break;
						}
					}
				}
			}
			else if (_objectSelected)
			{
				if (_selectedObject.Item1 != null && _selectedObject.Item1.Type == Selectable.Player && (((RoleHandler)_selectedObject.Item2).Player.TwitchUser.Username == "" || ((RoleHandler)_selectedObject.Item2).Player.TwitchUser.TwitchUserType == TwitchLib.Client.Enums.UserType.Broadcaster))
				{
					if (Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputManager.MousePosition), out RaycastHit hitInfo, float.MaxValue) && !WorldUtils.IsPointerOverUI(EventSystem.current))
					{
						SelectableObject obj = hitInfo.transform.GetComponentInChildren<SelectableObject>();
						if (obj != null)
						{
							switch (obj.Type)
							{
								case Selectable.Building:
									Station station = ((BuildingBase)obj.Data).Station;
									((RoleHandler)_selectedObject.Item2).Player.StationSensor.TrySetStation(station, ((RoleHandler)_selectedObject.Item2).Player);
									break;
								case Selectable.Enemy:
									Enemy enemy = ((Enemy)obj.Data);
									((RoleHandler)_selectedObject.Item2).Player.TargetSensor.TrySetTarget(enemy.gameObject.GetComponent<Targetable>(), ((RoleHandler)_selectedObject.Item2).Player);
									break;
								case Selectable.Resource:
									ResourceHolder resource = ((ResourceHolder)obj.Data);
									((RoleHandler)_selectedObject.Item2).Player.TargetSensor.TrySetTarget(resource.gameObject.GetComponent<Targetable>(), ((RoleHandler)_selectedObject.Item2).Player);
									break;
							}
						}
					}
				}
			}
		}

		public static void SetGroupTarget(List<RoleHandler> recruits, Targetable target)
		{
			for (int i = 0; i < recruits.Count; i++)
				recruits[i].Player.TargetSensor.TrySetTarget(target, recruits[i].Player);
		}

		public static void SetGroupStation(List<RoleHandler> recruits, Station station)
		{
			for (int i = 0; i < recruits.Count; i++)
				recruits[i].Player.StationSensor.TrySetStation(station, recruits[i].Player);
		}

		private static void OnDrawGizmos()
		{
			if (_startedGroupSelection)
			{
				Gizmos.DrawSphere(_startedSelectionPosition, 1.0f);

				Vector3 mousePos = Vector3.zero;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputManager.MousePosition), out RaycastHit hitInfo, float.MaxValue))
					mousePos = hitInfo.point;
				Gizmos.DrawSphere(mousePos, 1.0f);
			}
			else if (!_objectSelected)
				_selectionUI.HideSelectionOutline();
		}
	}
}