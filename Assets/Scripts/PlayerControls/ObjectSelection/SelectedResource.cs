using UnityEngine;
using TMPro;
using GameResources;

namespace PlayerControls.ObjectSelection
{
	public class SelectedResource : SelectedObject
	{
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			EnableDisplay();
			SetResourceName();
			UpdateDisplay();
			AttachEvents();
		}

		public void OnResourceDepleted()
		{

		}
		protected override void AttachEvents()
		{
			ResourceHolder resourceHolder = ((ResourceHolder)_selectedObject);
			resourceHolder.OnAmountChange += UpdateResourceAmount;
		}

		protected override void DetachEvents()
		{
			ResourceHolder resourceHolder = ((ResourceHolder)_selectedObject);
			resourceHolder.OnAmountChange -= UpdateResourceAmount;
		}

		public void DetachCurrentEvents() { DetachEvents(); }

		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.Title.gameObject.SetActive(true);
			_selectedObjectTypeUI.Description.gameObject.SetActive(true);
			//_selectedObjectTypeUI.RedSlider.gameObject.SetActive(true);
			_selectedObjectTypeUI.ID.gameObject.SetActive(true);
		}

		public override void UpdateDisplay()
		{
			ResourceHolder resourceHolder = ((ResourceHolder)_selectedObject);
			UpdateResourceAmount(resourceHolder);
		}

		public void SetResourceName()
		{
			_selectedObjectTypeUI.Title.text = (((ResourceHolder)_selectedObject).ResourceType.ToString()).ToUpper();
		}

		public void UpdateResourceAmount(ResourceHolder resource)
		{
			_selectedObjectTypeUI.Description.text = "Remaining: " + resource.Amount.ToString();
			// set slider value here (currently doesnt have a max vale)
		}
	}
}