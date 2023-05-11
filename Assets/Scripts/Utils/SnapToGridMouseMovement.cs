using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Utils
{
	public class SnapToGridMouseMovement : MonoBehaviour
	{
		public Action OnPositionChanged;

		[SerializeField]
		private float _cellSize = 2.0f;

		[SerializeField]
		private LayerMask _collisionMask;
		
		private Camera _mainCamera;

		private Vector3 _lastSnappedPosition = Vector3.zero;

		private void MoveObject()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, _collisionMask))
			{
				transform.position = MathExtended.SnapPosition(hit.point, _cellSize);

				if (_lastSnappedPosition != transform.position)
					OnPositionChanged?.Invoke();

				_lastSnappedPosition = transform.position;
			}
		}

		private void Awake()
		{
			_mainCamera = Camera.main;
		}
		
		private void Update()
		{
			MoveObject();
		}
	}
}