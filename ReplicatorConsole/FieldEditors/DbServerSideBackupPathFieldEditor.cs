using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibDataInput;
using AppCliTools.LibMenuInput;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.FieldEditors;

public sealed class DbServerSideBackupPathFieldEditor : FieldEditor<string>
{
    private readonly string _databaseBackupParametersPropertyName;

    private readonly string _databaseWebAgentNamePropertyName;
    private readonly ParametersManager _parametersManager;

    public DbServerSideBackupPathFieldEditor(string propertyName, ParametersManager parametersManager,
        string databaseWebAgentNamePropertyName, string databaseBackupParametersPropertyName,
        bool enterFieldDataOnCreate = false) : base(propertyName, enterFieldDataOnCreate)
    {
        _parametersManager = parametersManager;
        _databaseWebAgentNamePropertyName = databaseWebAgentNamePropertyName;
        _databaseBackupParametersPropertyName = databaseBackupParametersPropertyName;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        string? databaseWebAgentName = GetValue<string>(recordForUpdate, _databaseWebAgentNamePropertyName);
        var databaseBackupParameters =
            GetValue<DatabaseBackupParametersDomain>(recordForUpdate, _databaseBackupParametersPropertyName);
        string? currentPath = GetValue(recordForUpdate);

        if (currentPath != null && Inputer.InputBool("Clear?", false, false))
        {
            SetValue(recordForUpdate, null);
            return ValueTask.CompletedTask;
        }

        if (!string.IsNullOrWhiteSpace(databaseWebAgentName))
        {
            StShared.WriteWarningLine("Cannot set Db Server Side Backup Path, because Web Agent is used", true, null,
                true);
            return ValueTask.CompletedTask;
        }

        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        string? workFolderCandidateForLocalPath = databaseBackupParameters is null
            ? null
            : parameters.CountLocalPath(currentPath, _parametersManager.ParametersFileName,
                $"Database{databaseBackupParameters.BackupType}Backups");

        string? newValue = MenuInputer.InputFolderPath(FieldName, workFolderCandidateForLocalPath);

        SetValue(recordForUpdate, string.IsNullOrWhiteSpace(newValue) ? null : newValue);
        return ValueTask.CompletedTask;
    }
}
