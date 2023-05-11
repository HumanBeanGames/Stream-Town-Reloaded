using UnityEngine;
using UnityEngine.Events;
using UserInterface;

namespace PlayerControls.ObjectSelection 
{
    public class SelectedObject
	{
		[SerializeField]
		protected UserInterface_ObjectSelection _selectedObjectTypeUI;
        protected object _selectedObject;
        protected UnityAction OnButtonClick;
        protected UnityAction OnButtonTwoClick;

        protected UnityAction OnCheckConfirm;
        protected UnityAction OnCheckDeny;

        public UnityAction<int> OnDropDownChange;
        
        public UserInterface_ObjectSelection SelectableObjectUI
		{
            get { return _selectedObjectTypeUI; }
            set { _selectedObjectTypeUI = value; }
		}

		public bool DisplayEnabled { get { return _selectedObjectTypeUI.gameObject.activeInHierarchy; } }

        protected virtual void EnableDisplay(){}

        protected virtual void AttachEvents(){}

        protected virtual void DetachEvents(){}

        protected void DisableCheck()
		{
            _selectedObjectTypeUI.ConfirmCheck.DisableCheck();
        }
        
        protected void EnableCheck()
		{
            _selectedObjectTypeUI.ConfirmCheck.EnableCheck();
        }


        public void ToggleDisplay()
		{
            _selectedObjectTypeUI.gameObject.gameObject.SetActive(!_selectedObjectTypeUI.gameObject.activeInHierarchy);
		}

        public virtual void SetDisplay(object data) 
        {
            _selectedObject = data;
        }

        public virtual void UpdateDisplay() {}
        public virtual void HideDisplay() {}
    }
}