using UnityEngine;
using TMPro;
using Enemies;
using Units;

namespace PlayerControls.ObjectSelection 
{
    public class SelectedEnemy : SelectedObject
	{
		public override void SetDisplay(object data)
		{
			base.SetDisplay(data);
			AttachEvents();
			EnableDisplay();
			SetEnemyName();
			UpdateDisplay();
		}
		protected override void AttachEvents()
		{
			Enemy enemy = ((Enemy)_selectedObject);
			enemy.HealthHandler.OnHealthChange += UpdateHealth;
			enemy.HealthHandler.OnDeath += OnDeath;
		}

		public void OnDeath(bool died)
		{
			DetachCurrentEvents();
			_selectedObjectTypeUI.HideContext();
		}

		protected override void DetachEvents()
		{
			Enemy enemy = ((Enemy)_selectedObject);
			enemy.HealthHandler.OnHealthChange -= UpdateHealth;
			enemy.HealthHandler.OnDeath -= OnDeath;
		}

		public void DetachCurrentEvents() { DetachEvents(); }
		protected override void EnableDisplay()
		{
			_selectedObjectTypeUI.Title.gameObject.SetActive(true);
			_selectedObjectTypeUI.RedSliderContainer.gameObject.SetActive(true);
			_selectedObjectTypeUI.ID.gameObject.SetActive(true);
		}

		public override void UpdateDisplay()
		{
			Enemy enemy = ((Enemy)_selectedObject);
			UpdateHealth(enemy.HealthHandler);	
		}

		public void SetEnemyName()
		{
			_selectedObjectTypeUI.Title.text = (((Enemy)_selectedObject).EnemyType.ToString()).ToUpper(); ;
		}

		public void UpdateHealth(HealthHandler health)
		{
			_selectedObjectTypeUI.RedSliderValue.text = health.Health + " / " + health.MaxHealth;
			if (health.Health != 0)
				_selectedObjectTypeUI.RedSlider.value = health.HealthPercentage;
			else
				_selectedObjectTypeUI.RedSlider.value = 0;
		}
	}
}