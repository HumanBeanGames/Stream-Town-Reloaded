using GameEventSystem.Events.Voting;

using System;
using System.Collections.Generic;
using TechTree.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserInterface
{
	public class UserInterface_TownVote : MonoBehaviour
	{
		public GameObject TownVoteContainer;
		public Transform TownVoteOptionsContainer;
		public GameObject TechOptionPrefab;
		public Slider TimerSlider;
		public TextMeshProUGUI TitleTMP;
		public TextMeshProUGUI TimerTMP;
		public  UnityAction<int> OnBroadcasterVote;

		[SerializeField]
		private Button _bottomBarButton;

		private List<UI_TechOption> _techOptions;

		private bool _canOpenVoteContainer = false;

		public void ActivateVoteContainer()
		{
			TownVoteContainer.SetActive(true);

			_bottomBarButton.interactable = true;
			_canOpenVoteContainer = true;
		}

		public void SetupButtons()
		{

			for (int i = 0; i < _techOptions.Count; i++)
			{
				int num = i;
				_techOptions[i].TechButton.gameObject.SetActive(true);
				_techOptions[i].TechButton.onClick.AddListener(() => BroadcasterVote(num + 1));
			}
		}

		public void BroadcasterVote(int voteId)
		{
			((VoteEvent)GameManager.Instance.GameEventManager.CurrentEvent).Action(new PlayerVote(GameManager.Instance.UserPlayer, new VoteOption(voteId.ToString(), null)));
			for (int i = 0; i < _techOptions.Count; i ++)
			{
				_techOptions[i].TechButton.onClick.RemoveAllListeners();
				_techOptions[i].TechButton.interactable = false;
			}
		}

		public void DeactivateVoteContainer()
		{
			_bottomBarButton.interactable = false;
			_canOpenVoteContainer = false;

			if (_techOptions != null)
			{
				for (int i = _techOptions.Count - 1; i >= 0; i--)
				{
					Destroy(_techOptions[i].gameObject);
					_techOptions.RemoveAt(i);
				}
			}

			_techOptions = new List<UI_TechOption>();

			TownVoteContainer.SetActive(false);
		}

		public void ToggleVotingMenu()
		{
			if (_canOpenVoteContainer)
			{
				TownVoteContainer.SetActive(!TownVoteContainer.activeSelf);
			}
		}

		public UI_TechOption AddOption(Node_SO node, int index)
		{
			GameObject go = Instantiate(TechOptionPrefab, TownVoteOptionsContainer);

			UI_TechOption uiTech = go.GetComponent<UI_TechOption>();

			uiTech.TitleTMP.text = node.NodeTitle;
			uiTech.DescriptionTMP.text = node.Description;
			uiTech.VoteCommandTMP.text = $"!vote {index}";
			uiTech.TechButton.gameObject.SetActive(true);
			string modPath = node.IconPath.Remove(0, 17);
			modPath = modPath.Remove(modPath.Length - 4, 4);
			Debug.Log(modPath);
			uiTech.TechIcon.sprite = Resources.Load<Sprite>(modPath) as Sprite;
			_techOptions.Add(uiTech);
			return uiTech;
		}

		private void Start()
		{
			DeactivateVoteContainer();
		}
	}
}