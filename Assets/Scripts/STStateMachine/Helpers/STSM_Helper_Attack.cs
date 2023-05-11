using Sensors;
using STStateMachine.States;
using Target;
using Units;

namespace STStateMachine.Helpers
{
	/// <summary>
	/// A state helper to help a unit attack another unit.
	/// </summary>
	public class STSM_Helper_Attack : STSM_HelperBase
	{
		public int Damage;
		public STSM_GoToLocation GoToState;
		public TargetSensor TargetSensor;
		public HealthHandler TargetHealth;
		public Targetable Target;
		public Targetable Owner;

		public override void Init()
		{
			HelperName = "Attack";
			GoToState = (STSM_GoToLocation)_stateMachine.GetStateByName("GoTo");
			TargetSensor = GetComponent<TargetSensor>();
			Owner = GetComponent<Targetable>();
		}

		public override void InvokeHelper()
		{
			// If we don't have a target, swap back to idle.
			if (Target == null || !Target.gameObject.activeInHierarchy)
			{
				_stateMachine.RequestStateChange("Idle");
				return;
			}

			// Ensure that we have the target's health component.
			if (TargetHealth == null || TargetHealth.gameObject != Target.gameObject)
				TargetHealth = Target.GetComponent<HealthHandler>();

			// Deal damage to the target.
			TargetHealth.TakeDamage(Damage, Owner);
		}
	}
}