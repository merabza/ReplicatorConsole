using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class RunProgramStepCruder : StepCruder<RunProgramStep>
{
    public RunProgramStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, RunProgramStep> currentValuesDictionary) : base(appName, logger, httpClientFactory,
        processes, parametersManager, currentValuesDictionary, "Run Program Step", "Run Program Steps")
    {
        List<FieldEditor> tempFieldEditors = [.. FieldEditors];
        FieldEditors.Clear();

        FieldEditors.Add(new TextFieldEditor(nameof(RunProgramStep.Program)));
        FieldEditors.Add(new TextFieldEditor(nameof(RunProgramStep.Arguments)));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
