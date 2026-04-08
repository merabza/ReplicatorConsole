namespace ReplicatorConsole.Counters;

public /*open*/ class MaskCounter
{
    protected virtual bool MaskExists(string mask)
    {
        return false;
    }

    public string CountMask(string path)
    {
        var dir = new DirectoryInfo(path);
        string mask = dir.Name;

        string startDefVal = mask;
        int index = 1;
        while (MaskExists(mask))
        {
            index++;
            mask = $"{startDefVal}{index}";
        }

        return mask;
    }
}
