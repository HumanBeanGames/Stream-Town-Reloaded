using System;

namespace DataStructures
{
	/// <summary>
	/// Stores a Time Stamp and a value and is used for determining a change over time.
	/// </summary>
	public struct ChangeTimeStamp
	{
		public DateTime TimeStamp;
		public int Change;

		public ChangeTimeStamp(DateTime timeStamp, int change)
		{
			TimeStamp = timeStamp;
			Change = change;
		}
	}
}