using Utils;

namespace GameResources
{
	/// <summary>
	/// An interface for any class that requires holding resources.
	/// </summary>
	public interface IResourceHolder
	{
		public void AddResource(Utils.Resource type, int amount);

		public void RemoveResource(Utils.Resource type, int amount);

		public bool ResourceFull(Utils.Resource type);
	}
}