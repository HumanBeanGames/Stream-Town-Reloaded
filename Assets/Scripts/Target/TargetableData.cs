using Utils;

namespace Target
{
	/// <summary>
	/// Holds Targetable Object data.
	/// </summary>
	[System.Serializable]
	public struct TargetableData
	{
		public TargetMask TargetType;
		public StationUpdate UpdateType;
	}
}