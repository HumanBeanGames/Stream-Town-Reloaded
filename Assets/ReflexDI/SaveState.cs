public class SaveState
{
    public bool _savedData;
    public bool _loadingData;

    public bool SafeSave()
    {
        if (!_loadingData)
        {
            _savedData = false;
            return false;
        }
        return true;
    }
}
