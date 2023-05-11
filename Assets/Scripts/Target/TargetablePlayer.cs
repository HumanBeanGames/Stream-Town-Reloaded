using Utils;

namespace Target
{
	/// <summary>
	/// Targetable Component for the Player Unit.
	/// </summary>
	public class TargetablePlayer : TargetableHealth
	{
		/// <summary>
		/// Called when Player's health is full.
		/// </summary>
		protected override void OnHealthPercentageFull()
		{
			SetTargetType(TargetMask.Player);
		}

		/// <summary>
		/// Called when Player's health has reached zero.
		/// </summary>
		protected override void OnHealthPercentageZero()
		{
			SetTargetType(TargetMask.DeadPlayer);
		}

		/// <summary>
		/// Called when Player's health is neither full or empty.
		/// </summary>
		/// <param name="value"></param>
		protected override void OnHealthPercentageOther(float value)
		{
			SetTargetType(TargetMask.InjuredPlayer);
		}
	}
}