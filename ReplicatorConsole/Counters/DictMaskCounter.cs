namespace ReplicatorConsole.Counters;

public sealed class DictMaskCounter : MaskCounter
{
    private readonly Dictionary<string, string> _masksAndFolders;

    public DictMaskCounter(Dictionary<string, string> masksAndFolders)
    {
        _masksAndFolders = masksAndFolders;
    }

    protected override bool MaskExists(string mask)
    {
        return _masksAndFolders.ContainsKey(mask);
    }
}
