
using Target;
using TMPro;
using UnityEngine;
using Utils;

namespace UserInterface
{
	/// <summary>
	/// Used for displaying text above a unit and rotates it to face the camera.
	/// </summary>
	public class UnitTextDisplay : MonoBehaviour
	{
		[SerializeField]
		private TextMeshPro _displayText;

		private bool _counterEnabled = false;
		private float _lifeTime = 5;
		private string _nextText = "";
		private bool _disableOnSet = false;
		private SimpleLookAtCamera _cameraLookat;

		public Targetable Targetable {get;set;}
		/// <summary>
		/// Set the displayed text immediately.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplayText(string text)
		{
			_cameraLookat.enabled = true;
			_displayText.enabled = true;
			_displayText.text = text;
		}

		/// <summary>
		/// Set the displayed text after a defined delay.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="time"></param>
		/// <param name="disableOnSet"></param>
		public void SetDisplayTextAfterTime(string text, float time, bool disableOnSet = true)
		{
			_cameraLookat.enabled = true;
			_displayText.enabled = true;
			_counterEnabled = true;
			_lifeTime = time;
			_nextText = text;
			_disableOnSet = disableOnSet;
		}

		/// <summary>
		/// Sets the color of the text.
		/// </summary>
		/// <param name="color"></param>
		public void SetTextColor(Color color)
		{
			_displayText.color = color;
		}

		private void Awake()
		{
			_cameraLookat = GetComponent<SimpleLookAtCamera>();
			_cameraLookat.enabled = false;
			_displayText.enabled = false;
		}

		private void Update()
		{
			if (_counterEnabled)
			{
				_lifeTime -= Time.deltaTime;
				if (_lifeTime <= 0)
				{
					_counterEnabled = false;
					_displayText.text = _nextText;

					if (_disableOnSet)
					{
						_displayText.enabled = false;
						_cameraLookat.enabled = false;
						gameObject.SetActive(false);
					}
				}
			}
		}

		private void OnDisable()
		{
			UtilDisplayManager.RemoveTextDisplay(Targetable);
		}
	}
}