using Buildings;
using Character;
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
	public class DebugManager : MonoBehaviour
	{
		[SerializeField]
		private UserInterface_Debug _debugUI;

		private UnityEvent<SelectableObject, object> _onObjectSelected = new UnityEvent<SelectableObject, object>();

		private (SelectableObject, object) _selectedObject;

		public UnityEvent<SelectableObject, object> OnObjectSelected => _onObjectSelected;

		/// <summary>
		/// Called when SelectableObject is selected by the mouse and displays it's data.
		/// </summary>
		/// <param name="selected"></param>
		/// <param name="data"></param>
		private void ObjectSelected(SelectableObject selected, object data)
		{
			_selectedObject.Item1 = selected;
			_selectedObject.Item2 = data;

			Debug.Log($"Object Selected: {selected.gameObject.transform.parent.name}, {selected.SelectableType}");

			switch (selected.SelectableType)
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
			}
		}

		// Unity Events.
		private void Awake()
		{
			_onObjectSelected.AddListener(ObjectSelected);
		}

		private void Update()
		{
			if (Keyboard.current.escapeKey.wasReleasedThisFrame)
			{
				_debugUI.HideBuildingContext();
				_debugUI.HideCharacterContext();
			}
		}
	}
}