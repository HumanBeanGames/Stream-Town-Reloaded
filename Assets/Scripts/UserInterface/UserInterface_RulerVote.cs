using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
	public class UserInterface_RulerVote : MonoBehaviour
	{
		public GameObject RulerVotingContainer;
		public TextMeshProUGUI DescriptionTMP;
		public Slider TimerSlider;
		public TextMeshProUGUI TimerTMP;
		public GameObject RulerVoteOptionPrefab;
		public GameObject RulerOptionsContainer;

		private Dictionary<UI_RulerOption, GameObject> _options;

		public Dictionary<UI_RulerOption, GameObject> Options => _options;

		public void ActivateRulerContainer()
		{
			RulerVotingContainer.gameObject.SetActive(true);
		}

		public void DisableRulerContainer()
		{
			if (_options != null)
			{
				List<UI_RulerOption> optionsToRemove = new List<UI_RulerOption>();
				foreach (var v in _options)
				{
					optionsToRemove.Add(v.Key);
				}

				for (int i = 0; i < optionsToRemove.Count; i++)
				{
					_options.Remove(optionsToRemove[i]);
					Destroy(optionsToRemove[i].gameObject);
				}
			}
			_options = new Dictionary<UI_RulerOption, GameObject>();
			RulerVotingContainer.gameObject.SetActive(false);
		}

		public UI_RulerOption AddOption(string text)
		{
			GameObject go = Instantiate(RulerVoteOptionPrefab, RulerOptionsContainer.transform);

			UI_RulerOption uiRuler = go.GetComponent<UI_RulerOption>();

			uiRuler.TextTMP.text = text;
			_options.Add(uiRuler, go);
			return uiRuler;
		}

		public void RemoveOption(UI_RulerOption option)
		{
			_options.Remove(option);
		}

		private void Start()
		{
			DisableRulerContainer();
		}
	}
}