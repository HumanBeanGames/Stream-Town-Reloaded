using Enemies;
using UnityEngine;

namespace STStateMachine.States
{
	/// <summary>
	/// Simple action state for enemy attacks.
	/// </summary>
	public class STSM_Action_EnemyAttack : STSM_Action_Attack
	{
		protected EnemyModelHandler _enemyModelHandler;

		protected override void OnInit()
		{
			base.OnInit();
			_enemyModelHandler = GetComponentInChildren<EnemyModelHandler>();
		}

		public override void OnEnter()
		{
			if (_useAnimation)
			{
				_actionAnimation = _enemyModelHandler.GetAttackAnimation();
				_actionVariants = _enemyModelHandler.GetAttackVariantCount();
			}
			base.OnEnter();
		}
	}
}