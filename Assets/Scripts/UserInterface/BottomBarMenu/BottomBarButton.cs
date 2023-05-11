using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserInterface.BottomBarMenu 
{

	public enum BottomBarContext
	{
		BuildMenu,
		RecruitMenu,
		TechTreeMenu
	}
    public class BottomBarButton : MonoBehaviour
	{
		[SerializeField]
		private Image _arrowImage;

		[SerializeField]
		private BottomBarContext _buttonContext;

		protected bool _active = false;

		public bool Active => _active;

		public BottomBarContext ButtonContext => _buttonContext;

        public void ToggleContext()
		{
			if (!_active)
				BottomBarInterface.OnActivatedButton(this);
			else
				BottomBarInterface.OnDeactivatedButton(this);
		}

		public void OnContextHidden()
		{
			_arrowImage.enabled = false;
			_active = false;
		}

		public void OnContextShown()
		{
			_active = true;
			_arrowImage.enabled = true;
		}

		private void OnEnable()
		{
			StartCoroutine(DelayedAddButton());
		}

		private void OnDisable()
		{
			BottomBarInterface.RemoveButton(this);
		}

		private IEnumerator DelayedAddButton()
		{
			yield return new WaitForSeconds(0.01f);
			BottomBarInterface.AddButton(this);
			OnContextHidden();
		}
	}
}