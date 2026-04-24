using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.FieldEditors;
using AppCliTools.CliParametersExcludeSetsEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class FilesMoveStepCruder : StepCruder<FilesMoveStep>
{
    public FilesMoveStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, FilesMoveStep> currentValuesDictionary) : base(appName, logger, httpClientFactory, processes,
        parametersManager, currentValuesDictionary, "Files Move Step", "Files Move Steps")
    {
        List<FieldEditor> tempFieldEditors = [.. FieldEditors];
        FieldEditors.Clear();

        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(FilesMoveStep.SourceFileStorageName),
            ParametersManager));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(FilesMoveStep.DestinationFileStorageName),
            ParametersManager));
        FieldEditors.Add(new TextFieldEditor(nameof(FilesMoveStep.MoveFolderMask), "yyyyMMddHHmmss"));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(FilesMoveStep.ExcludeSet), parametersManager, true));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(FilesMoveStep.DeleteDestinationFilesSet),
            ParametersManager, true));
        FieldEditors.Add(
            new ReplacePairsSetNameFieldEditor(nameof(FilesMoveStep.ReplacePairsSet), parametersManager, true));
        FieldEditors.Add(new IntFieldEditor(nameof(FilesMoveStep.MaxFolderCount), 2));
        FieldEditors.Add(new BoolFieldEditor(nameof(FilesMoveStep.CreateFolderWithDateTime), true));
        FieldEditors.Add(new FolderPathsSetFieldEditor(nameof(FilesMoveStep.PriorityPoints)));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
