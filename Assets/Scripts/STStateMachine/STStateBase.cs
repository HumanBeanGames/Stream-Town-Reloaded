using UnityEngine;

namespace STStateMachine
{
	/// <summary>
	/// Base class for all States in Stream Town.
	/// </summary>
	public class STStateBase : MonoBehaviour
	{
		protected StateMachine _stateMachine;

		/// <summary>
		/// Called when a state is entered.
		/// </summary>
		public virtual void OnEnter()
		{

		}

		/// <summary>
		/// Called every frame when a state is updated.
		/// </summary>
		public virtual void OnUpdate()
		{

		}

		/// <summary>
		/// Called when a state is exited.
		/// </summary>
		public virtual void OnExit()
		{

		}

		/// <summary>
		/// Called when a state is Initialized.
		/// </summary>
		protected virtual void OnInit()
		{

		}

		private void Awake()
		{
			_stateMachine = GetComponent<StateMachine>();
			OnInit();
		}
	}
}