using Character;
using System.Collections.Generic;
using Target;
using UnityEngine;
using UserInterface;
using VFX;

namespace Managers
{
	/// <summary>
	/// Used to manage the display text over targetable objects.
	/// This used as the game will run poorly if everything has it's own text component.
	/// </summary>
	public static class UtilDisplayManager
	{
		private static Dictionary<Targetable, UnitTextDisplay> _activeTextDisplays = new Dictionary<Targetable, UnitTextDisplay>();
		private static Dictionary<Player, GameObject> _pingObjects = new Dictionary<Player, GameObject>();
		/// <summary>
		/// Adds a target and it's display to the dictionary.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="text"></param>
		/// <param name="time"></param>
		public static void AddTextDisplay(Targetable target, string text, float time = 15.0f)
		{
			UnitTextDisplay display = default;

			if (!_activeTextDisplays.ContainsKey(target))
			{
				var textDisplay = ObjectPoolingManager.GetPooledObject("TextDisplay");
				textDisplay.gameObject.SetActive(true);
				var rectTransform = textDisplay.GetComponent<RectTransform>();
				rectTransform.SetParent(target.TextDisplayTransform, false);
				rectTransform.localPosition = target.TextDisplayTransform.localPosition;

				display = textDisplay.GetComponent<UnitTextDisplay>();
				display.Targetable = target;

				_activeTextDisplays.Add(target, display);
			}
			else
				display = _activeTextDisplays[target];

			if (!display.gameObject.activeInHierarchy)
				display.gameObject.SetActive(true);

			display.SetDisplayText(text);
			display.SetDisplayTextAfterTime($"", time);
		}

		/// <summary>
		/// Removes the target and its display from the dictionary.
		/// </summary>
		/// <param name="target"></param>
		public static void RemoveTextDisplay(Targetable target)
		{
			if (target != null && _activeTextDisplays.ContainsKey(target))
				_activeTextDisplays.Remove(target);
		}

		public static void AddPingObject(Player player)
		{
			if (_pingObjects.ContainsKey(player))
				return;

			VFXArrowPointer pingObject = ObjectPoolingManager.GetPooledObject("VFXPing").GetComponent<VFXArrowPointer>();

			pingObject.transform.parent = player.Character.transform;
			pingObject.transform.localPosition = Vector3.zero;
			pingObject.SetPlayer(player);
			pingObject.gameObject.SetActive(true);

			_pingObjects.Add(player, pingObject.gameObject);
		}

		public static void RemovePingObject(Player player)
		{
			if (player != null && !_pingObjects.ContainsKey(player))
				return;

			_pingObjects.Remove(player);
		}
	}
}