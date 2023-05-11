namespace STStateMachine.States
{
	/// <summary>
	/// A simple idle state for enemy units.
	/// </summary>
	public class STSM_Idle_Enemy : STSM_Idle
	{
		private STSM_Action_Attack _attackAction;

		protected override void OnInit()
		{
			base.OnInit();
			_attackAction = (STSM_Action_Attack)_stateMachine.GetStateByName("Attack");
		}

		protected override void OnHasTarget()
		{
			base.OnHasTarget();

			_goToState.UsePosition = false;
			_goToState.SetNextState(_attackAction);
			_goToState.SetTarget(_targetSensor.CurrentTarget.transform);
			_goToState.SetDistanceSatisfaction(_targetSensor.CurrentTarget.SizeSqr + (_attackAction.Range * _attackAction.Range));
			_stateMachine.RequestStateChange(_goToState);
		}
	}
}