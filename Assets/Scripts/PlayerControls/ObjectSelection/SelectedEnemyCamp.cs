using UnityEngine;
using TMPro;
using Buildings;
using Units;

namespace PlayerControls.ObjectSelection 
{
    public class SelectedEnemyCamp : SelectedObject
	{
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			EnableDisplay();
			SetEnemyCampName();
			//SetEnemyCampID();
			UpdateDisplay();
		}

		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.RedSliderContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.Title.gameObject.SetActive(true);
			_selectedObjectTypeUI.ID.gameObject.SetActive(true);
		}

		public override void UpdateDisplay()
		{
			Station station = ((Station)_selectedObject);
			UpdateHealth(station);
		}

		public void UpdateHealth(Station station)
		{
			HealthHandler health = station.transform.GetComponent<HealthHandler>();
			_selectedObjectTypeUI.RedSliderValue.text = health.Health + " / " + health.MaxHealth;
			if (health.Health != 0)
				_selectedObjectTypeUI.RedSlider.value = (health.Health / health.MaxHealth);
			else
				_selectedObjectTypeUI.RedSlider.value = 0;
		}

		private void SetEnemyCampName()
		{
			_selectedObjectTypeUI.Title.text = (((Station)_selectedObject).Flags.ToString()).ToUpper();
		}
		private void SetEnemyCampID()
		{
			_selectedObjectTypeUI.Title.text = ((Station)_selectedObject).Flags.ToString();
		}
	}
}