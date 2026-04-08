using DatabaseTools.DbTools;

namespace ReplicatorConsole.Counters;

public sealed class BackupNameMiddlePartCounter
{
    private readonly EBackupType _backupType;

    public BackupNameMiddlePartCounter(EBackupType backupType)
    {
        _backupType = backupType;
    }

    public string Count()
    {
        return $"_Backup_{_backupType}";
    }
}
