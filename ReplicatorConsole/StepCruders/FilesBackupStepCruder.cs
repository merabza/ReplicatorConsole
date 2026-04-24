using System;
using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.FieldEditors;
using AppCliTools.CliParametersExcludeSetsEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Counters;
using ReplicatorConsole.Cruders;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.StepCruders;

public sealed class FilesBackupStepCruder : StepCruder<FilesBackupStep>
{
    public FilesBackupStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, FilesBackupStep> currentValuesDictionary) : base(appName, logger, httpClientFactory,
        processes, parametersManager, currentValuesDictionary, "Files Backup Step", "Files Backup Steps")
    {
        string? parametersFileName = parametersManager.ParametersFileName;
        const string dateMask = DateMaskKeeper.DateMask;

        List<FieldEditor> tempFieldEditors = [];
        tempFieldEditors.AddRange(FieldEditors);
        FieldEditors.Clear();

        FieldEditors.Add(new TextFieldEditor(nameof(FilesBackupStep.MaskName),
            $"{Environment.MachineName.Capitalize()}_"));
        FieldEditors.Add(new TextFieldEditor(nameof(FilesBackupStep.DateMask), dateMask));
        FieldEditors.Add(new LocalPathFieldEditor(nameof(FilesBackupStep.LocalPath), ParametersManager, null,
            parametersFileName));
        FieldEditors.Add(new ArchiverFieldEditor(nameof(FilesBackupStep.ArchiverName), ParametersManager));
        FieldEditors.Add(
            new SmartSchemaNameFieldEditor(nameof(FilesBackupStep.LocalSmartSchemaName), ParametersManager));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(FilesBackupStep.UploadFileStorageName),
            ParametersManager));
        FieldEditors.Add(new IntFieldEditor(nameof(FilesBackupStep.UploadProcLineId), 1));
        FieldEditors.Add(new SmartSchemaNameFieldEditor(nameof(FilesBackupStep.UploadSmartSchemaName),
            ParametersManager));
        FieldEditors.Add(new BoolFieldEditor(nameof(FilesBackupStep.BackupSeparately), true));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(FilesBackupStep.ExcludeSetName), ParametersManager));
        //FieldEditors.Add(new BackupFolderPathsFieldEditor(nameof(FilesBackupStep.BackupFolderPaths)));

        FieldEditors.Add(
            new SimpleNamesWithDescriptionsFieldEditor<FileBackupFolderCruder>(
                nameof(FilesBackupStep.BackupFolderPaths), ParametersManager));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
