using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class UnZipOnPlaceStepCruder : StepCruder<UnZipOnPlaceStep>
{
    public UnZipOnPlaceStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, UnZipOnPlaceStep> currentValuesDictionary) : base(appName, logger, httpClientFactory,
        processes, parametersManager, currentValuesDictionary, "Unzip On Place Step", "Unzip On Place Steps")
    {
        List<FieldEditor> tempFieldEditors = [];
        tempFieldEditors.AddRange(FieldEditors);
        FieldEditors.Clear();

        FieldEditors.Add(new FolderPathFieldEditor(nameof(UnZipOnPlaceStep.PathWithZips)));
        FieldEditors.Add(new BoolFieldEditor(nameof(UnZipOnPlaceStep.WithSubFolders), true));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
