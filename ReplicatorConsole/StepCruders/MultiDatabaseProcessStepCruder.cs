using System.Collections.Generic;
using System.Net.Http;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersApiClientsEdit.FieldEditors;
using AppCliTools.CliParametersDataEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class MultiDatabaseProcessStepCruder : StepCruder<MultiDatabaseProcessStep>
{
    public MultiDatabaseProcessStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager,
        Dictionary<string, MultiDatabaseProcessStep> currentValuesDictionary) : base(appName, logger, httpClientFactory,
        processes, parametersManager, currentValuesDictionary, "Multi Database process Step",
        "Multi Database process Steps")
    {
        List<FieldEditor> tempFieldEditors = [.. FieldEditors];
        FieldEditors.Clear();

        FieldEditors.Add(new EnumFieldEditor<EMultiDatabaseActionType>(nameof(MultiDatabaseProcessStep.ActionType),
            EMultiDatabaseActionType.CheckRepairDataBase));

        //public string DatabaseServerConnectionName { get; set; }
        FieldEditors.Add(new DatabaseServerConnectionNameFieldEditor(appName, logger, httpClientFactory,
            nameof(MultiDatabaseProcessStep.DatabaseServerConnectionName), ParametersManager, true));
        //public string DatabaseWebAgentName { get; set; }
        FieldEditors.Add(new ApiClientNameFieldEditor(nameof(MultiDatabaseProcessStep.DatabaseWebAgentName), logger,
            httpClientFactory, ParametersManager, true));

        FieldEditors.Add(new EnumFieldEditor<EDatabaseSet>(nameof(MultiDatabaseProcessStep.DatabaseSet),
            EDatabaseSet.AllDatabases));
        FieldEditors.Add(new DatabaseNamesFieldEditor(appName, logger, httpClientFactory,
            nameof(MultiDatabaseProcessStep.DatabaseNames), ParametersManager,
            nameof(MultiDatabaseProcessStep.DatabaseServerConnectionName),
            nameof(MultiDatabaseProcessStep.DatabaseSet)));

        FieldEditors.AddRange(tempFieldEditors);
    }
}
