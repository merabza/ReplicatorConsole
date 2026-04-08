using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.FieldEditors;
using AppCliTools.CliParametersExcludeSetsEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class FilesSyncStepCruder : StepCruder<FilesSyncStep>
{
    public FilesSyncStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, ParametersManager parametersManager,
        Dictionary<string, FilesSyncStep> currentValuesDictionary) : base(appName, logger, httpClientFactory, processes,
        parametersManager, currentValuesDictionary, "Files Sync Step", "Files Sync Steps")
    {
        List<FieldEditor> tempFieldEditors = [.. FieldEditors];
        FieldEditors.Clear();

        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(FilesSyncStep.SourceFileStorageName),
            ParametersManager));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(FilesSyncStep.DestinationFileStorageName),
            ParametersManager));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(FilesSyncStep.ExcludeSet), ParametersManager, true));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(FilesSyncStep.DeleteDestinationFilesSet),
            ParametersManager, true));
        FieldEditors.Add(new ReplacePairsSetNameFieldEditor(nameof(FilesSyncStep.ReplacePairsSet),
            ParametersManager, true));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
