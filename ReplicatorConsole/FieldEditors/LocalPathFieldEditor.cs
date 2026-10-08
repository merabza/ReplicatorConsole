using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibMenuInput;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.FieldEditors;

public sealed class LocalPathFieldEditor : FieldEditor<string>
{
    private readonly string? _databaseBackupParametersPropertyName;
    private readonly string? _parametersFileName;
    private readonly IParametersManager _parametersManager;

    public LocalPathFieldEditor(string propertyName, IParametersManager parametersManager,
        string? databaseBackupParametersPropertyName, string? parametersFileName, bool enterFieldDataOnCreate = false) :
        base(propertyName, enterFieldDataOnCreate)
    {
        _parametersManager = parametersManager;
        _databaseBackupParametersPropertyName = databaseBackupParametersPropertyName;
        _parametersFileName = parametersFileName;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        string? workFolderCandidateForLocalPath = CountWorkFolderCandidateForLocalPath(recordForUpdate);

        SetValue(recordForUpdate,
            MenuInputer.InputFolderPath(FieldName, GetValue(recordForUpdate, workFolderCandidateForLocalPath)));
        return ValueTask.CompletedTask;
    }

    internal string? CountWorkFolderCandidateForLocalPath(object recordForUpdate)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;
        if (_databaseBackupParametersPropertyName == null)
        {
            return parameters.CountLocalPath(null, _parametersFileName, "FilesBackups");
        }

        var databaseBackupParameters =
            GetValue<DatabaseParameters>(recordForUpdate, _databaseBackupParametersPropertyName);

        return databaseBackupParameters is null
            ? null
            : parameters.CountLocalPath(null, _parametersFileName,
                $"Database{databaseBackupParameters.BackupType ?? DatabaseParameters.DefaultBackupType}Backups");
    }
}
