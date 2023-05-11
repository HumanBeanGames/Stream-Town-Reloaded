namespace Globals 
{
    public struct ScriptableObjectAssetData<T>
	{
		public string GUID;
		public string AssetPath;
		public T Asset;
    }
}