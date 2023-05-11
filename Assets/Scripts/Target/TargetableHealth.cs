using Units;

namespace Target
{
	/// <summary>
	/// A general handler for a targetable objects Health.
	/// </summary>
	public class TargetableHealth : Targetable
	{
		protected HealthHandler _healthHander;

		public HealthHandler HealthHandler => _healthHander;

		/// <summary>
		/// Listens to changes in the Target's Health and handles the change.
		/// </summary>
		public virtual void HealthChangedListener()
		{
			if (!_healthHander)
				return;

			float healthPercentage = _healthHander.HealthPercentage;

			if (healthPercentage <= 0)
				OnHealthPercentageZero();
			else if (healthPercentage < 1)
				OnHealthPercentageOther(healthPercentage);
			else
				OnHealthPercentageFull();
		}

		/// <summary>
		/// Initalizes required data.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			_healthHander = GetComponent<HealthHandler>();
		}

		/// <summary>
		/// Called when Target's health is full.
		/// </summary>
		protected virtual void OnHealthPercentageFull()
		{

		}

		/// <summary>
		/// Called when Target's health has reached zero.
		/// </summary>
		protected virtual void OnHealthPercentageZero()
		{

		}

		/// <summary>
		/// Called when Target's health is neither full or empty.
		/// </summary>
		/// <param name="value"></param>
		protected virtual void OnHealthPercentageOther(float value)
		{

		}
	}
}