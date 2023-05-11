using UnityEngine;

namespace STStateMachine.Helpers
{
	/// <summary>
	/// An abstract base class for all helper actions used in conjuction with states.
	/// </summary>
	public abstract class STSM_HelperBase : MonoBehaviour
	{
		public string HelperName;
		protected StateMachine _stateMachine;

		/// <summary>
		/// Initializes the helper.
		/// </summary>
		public abstract void Init();

		/// <summary>
		/// Invokes the helper and runs it's behaviour.
		/// </summary>
		public abstract void InvokeHelper();

		private void Awake()
		{
			_stateMachine = GetComponent<StateMachine>();
			Init();
		}
	}
}