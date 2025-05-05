using Buildings;
using Character;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UserInterface;
using Utils;

namespace Managers
{
	/// <summary>
	/// Manages debug objects and user interface.
	/// </summary>
	[GameManager]
	public static class DebugManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static DebugConfig Config = DebugConfig.Instance;

		[HideInInspector]
        private static UserInterface_Debug _debugUI;

        [HideInInspector]
        private static UnityEvent<SelectableObject, object> _onObjectSelected = new UnityEvent<SelectableObject, object>();

        [HideInInspector]
        private static (SelectableObject, object) _selectedObject;

        public static UnityEvent<SelectableObject, object> OnObjectSelected => _onObjectSelected;

        /// <summary>
        /// Called when SelectableObject is selected by the mouse and displays it's data.
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="data"></param>
        private static void ObjectSelected(SelectableObject selected, object data)
		{
			_selectedObject.Item1 = selected;
			_selectedObject.Item2 = data;

			Debug.Log($"Object Selected: {selected.gameObject.transform.parent.name}, {selected.SelectableType}");

			/*switch (selected.SelectableType)
			{
				//case Selectable.Player:
				//	_debugUI.OnCharacterContext((RoleHandler)data);
				//	break;
				//case Selectable.Building:
				//	_debugUI.OnBuildingContext((BuildingBase)data);
				//	break;
				//case Selectable.Enemy:
				//	_debugUI.GetComponent((BuildingBase)data);
				//	break;
				//case Selectable.Resource:
				//	_debugUI.OnBuildingContext((BuildingBase)data);
				//	break;
			}*/
		}

        // Unity Events.
        private static void Awake()
		{
			_onObjectSelected.AddListener(ObjectSelected);
		}

        private static void Update()
		{
			if (_debugUI == null)
				if ((_debugUI = GameObject.FindWithTag("UI_Main")?.GetComponent<UserInterface_Debug>()) == null)
					return;

			if (Keyboard.current.escapeKey.wasReleasedThisFrame)
			{
				_debugUI.HideBuildingContext();
				_debugUI.HideCharacterContext();
			}
		}
	}
}