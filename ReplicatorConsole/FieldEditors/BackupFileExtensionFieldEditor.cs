using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibDataInput;
using DatabaseTools.DbTools;
using ReplicatorConsole.Counters;

namespace ReplicatorConsole.FieldEditors;

public sealed class BackupFileExtensionFieldEditor : TextFieldEditor
{
    private readonly string _backupTypePropertyName;

    public BackupFileExtensionFieldEditor(string propertyName, string backupTypePropertyName) : base(propertyName)
    {
        _backupTypePropertyName = backupTypePropertyName;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        var backupType = GetValue<EBackupType>(recordForUpdate, _backupTypePropertyName);
        var backupFileExtensionCounter = new BackupFileExtensionCounter(backupType);
        SetValue(recordForUpdate,
            Inputer.InputText(FieldName, GetValue(recordForUpdate, backupFileExtensionCounter.Count())));
        return ValueTask.CompletedTask;
    }
}
