using Buildings;
using Character;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using PlayerControls;
using System;
using GameResources;
using Enemies;

namespace Utils
{
	/// <summary>
	/// A component that allows an object to be selected by the mouse.
	/// Requires an object to be a Building or Player.
	/// </summary>
	public class SelectableObject : MonoBehaviour
	{
		[SerializeField]
		private Selectable _type;

		private object _data;

		public object Data => _data;
		public Selectable SelectableType => _type;

		public Selectable Type => _type;

		private void Awake()
		{
			switch (_type)
			{
				case Selectable.Player:
					_data = GetComponentInParent<RoleHandler>();
					break;
				case Selectable.Building:
					_data = GetComponentInParent<BuildingBase>();
					break;
				case Selectable.Enemy:
					_data = GetComponentInParent<Enemy>();
					break;
				case Selectable.Resource:
					_data = GetComponentInParent<ResourceHolder>();
					break;
				case Selectable.EnemyCamp:
					_data = GetComponentInParent<Station>();
					break;
			}
		}
	}
}