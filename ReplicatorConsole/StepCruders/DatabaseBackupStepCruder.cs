using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersDataEdit.FieldEditors;
using AppCliTools.CliParametersEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.StepCruders;

public sealed class DatabaseBackupStepCruder : StepCruder<DatabaseBackupStep>
{
    public DatabaseBackupStepCruder(IApplication application, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, DatabaseBackupStep> currentValuesDictionary) : base(application.AppName, logger,
        httpClientFactory, processes, parametersManager, currentValuesDictionary, "Database Backup Step",
        "Database Backup Steps")
    {
        string? parametersFileName = parametersManager.ParametersFileName;

        var tempFieldEditors = new List<FieldEditor>();
        tempFieldEditors.AddRange(FieldEditors);
        FieldEditors.Clear();

        FieldEditors.Add(new DatabaseServerConnectionNameFieldEditor(application, logger, httpClientFactory,
            nameof(DatabaseBackupStep.DatabaseServerConnectionName), ParametersManager, true));

        FieldEditors.Add(new DatabaseParametersFieldEditor(application, logger, httpClientFactory,
            nameof(DatabaseBackupStep.DatabaseBackupParameters), parametersManager));

        FieldEditors.Add(new EnumFieldEditor<EDatabaseSet>(nameof(DatabaseBackupStep.DatabaseSet),
            EDatabaseSet.AllDatabases));

        FieldEditors.Add(new DbServerFoldersSetNameFieldEditor(application.AppName, logger, httpClientFactory,
            nameof(DatabaseBackupStep.DbServerFoldersSetName), parametersManager,
            nameof(DatabaseBackupStep.DatabaseServerConnectionName)));

        FieldEditors.Add(new DatabaseNamesFieldEditor(application.AppName, logger, httpClientFactory,
            nameof(DatabaseBackupStep.DatabaseNames), ParametersManager,
            nameof(DatabaseBackupStep.DatabaseServerConnectionName), nameof(DatabaseBackupStep.DatabaseSet)));

        FieldEditors.Add(new SmartSchemaNameFieldEditor(nameof(DatabaseBackupStep.SmartSchemaName), ParametersManager));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(DatabaseBackupStep.FileStorageName),
            ParametersManager));
        FieldEditors.Add(new IntFieldEditor(nameof(DatabaseBackupStep.DownloadProcLineId), 1));
        FieldEditors.Add(new LocalPathFieldEditor(nameof(DatabaseBackupStep.LocalPath), ParametersManager,
            nameof(DatabaseBackupStep.DatabaseBackupParameters), parametersFileName));
        FieldEditors.Add(new SmartSchemaNameFieldEditor(nameof(DatabaseBackupStep.LocalSmartSchemaName),
            ParametersManager));
        FieldEditors.Add(new ArchiverFieldEditor(nameof(DatabaseBackupStep.ArchiverName), ParametersManager));
        FieldEditors.Add(new ArchiverProcLineIdFieldEditor(nameof(DatabaseBackupStep.CompressProcLineId), 1,
            nameof(DatabaseBackupStep.ArchiverName)));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(DatabaseBackupStep.UploadFileStorageName),
            ParametersManager));
        FieldEditors.Add(new IntFieldEditor(nameof(DatabaseBackupStep.UploadProcLineId), 1));
        FieldEditors.Add(new SmartSchemaNameFieldEditor(nameof(DatabaseBackupStep.UploadSmartSchemaName),
            ParametersManager));
        FieldEditors.AddRange(tempFieldEditors);
    }
}
