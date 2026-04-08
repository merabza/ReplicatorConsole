using DatabaseTools.DbTools;

namespace ReplicatorConsole.Counters;

public sealed class BackupFileExtensionCounter
{
    private readonly EBackupType _backupType;

    public BackupFileExtensionCounter(EBackupType backupType)
    {
        _backupType = backupType;
    }

    public string Count()
    {
        return Count(_backupType);
    }

    private string Count(EBackupType backupType)
    {
        return backupType switch
        {
            EBackupType.Full => "bak",
            EBackupType.Diff => "dif",
            EBackupType.TrLog => "trn",
            _ => throw new ArgumentOutOfRangeException(nameof(backupType), backupType,
                $"Unexpected backup type value: {backupType}")
        };
    }
}
