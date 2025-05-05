using UnityEngine;
using Managers;
using TMPro;
using Utils;

namespace UserInterface
{
	/// <summary>
	/// Handles the User Interface for the Town Resources.
	/// </summary>
	public class UserInterface_Resources : MonoBehaviour
	{
		[SerializeField]
		private GameObject _resourcePanel;
		[SerializeField]
		private TextMeshProUGUI[] _resourceTMPs;
		[SerializeField]
		private Color _positiveColor = Color.green;
		[SerializeField]
		private Color _negativeColor = Color.red;

		/// <summary>
		/// Called when a resource amount has changed and updates the text accordingly.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="amount"></param>
		/// <param name="purchase"></param>
		private void OnResourceChange(Resource resource, int amount, bool purchase)
		{
			return;

			//int rateOfChange = TownResourceManager.RateOfChangeForResource(resource);
			//bool positiveROC = rateOfChange >= 0;

			//Color toUse = positiveROC ? _positiveColor : _negativeColor;
			//string rateOfChangeString = $"<color=#{ColorUtility.ToHtmlStringRGBA(toUse)}>" + (positiveROC ? " + " : "-") + $"{StringUtils.GetShortenedNumberAsString(rateOfChange * 60)}/h</color>";
			//if ((int)resource - 1 >= _resourceTMPs.Length)
			//	return;
			//_resourceTMPs[(int)resource - 1].text = $"{TownResourceManager.ResourcePrint(resource)} {rateOfChangeString}";
		}

		private void Start()
		{
			TownResourceManager.OnAnyResourceChangeEvent.AddListener(OnResourceChange);

			for (int i = 1; i < (int)Resource.Count-1; i++)
			{
				TownResourceManager.OnAnyResourceChangeEvent?.Invoke((Utils.Resource)i, 0, false);
			}
		}
	}
}