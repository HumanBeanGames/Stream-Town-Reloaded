using UnityEngine;

namespace GUIDSystem 
{
	/// <summary>
	/// A class to hold information on an objects GUID
	/// </summary>
    public class GUIDComponent : MonoBehaviour 
	{
		private uint _gUID;	
		public uint GUID => _gUID;

		/// <summary>
		/// Sets the GUID
		/// </summary>
		/// <param name="gUID">The GUID to be set to</param>
		public void SetGUID(uint gUID)
		{
			_gUID = gUID;
		}
    }
}