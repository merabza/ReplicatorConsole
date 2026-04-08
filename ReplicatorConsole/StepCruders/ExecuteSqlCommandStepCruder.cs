using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersApiClientsEdit.FieldEditors;
using AppCliTools.CliParametersDataEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;

namespace ReplicatorConsole.StepCruders;

public sealed class ExecuteSqlCommandStepCruder : StepCruder<ExecuteSqlCommandStep>
{
    public ExecuteSqlCommandStepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, ParametersManager parametersManager,
        Dictionary<string, ExecuteSqlCommandStep> currentValuesDictionary) : base(appName, logger, httpClientFactory,
        processes, parametersManager, currentValuesDictionary, "Execute SQL Command Step", "Execute SQL Command Steps")
    {
        List<FieldEditor> tempFieldEditors = [..FieldEditors];

        FieldEditors.Clear();

        //public string DatabaseServerConnectionName { get; set; }
        FieldEditors.Add(new DatabaseServerConnectionNameFieldEditor(appName, logger, httpClientFactory,
            nameof(ExecuteSqlCommandStep.DatabaseServerConnectionName), ParametersManager, true));
        //public string DatabaseWebAgentName { get; set; }
        FieldEditors.Add(new ApiClientNameFieldEditor(nameof(ExecuteSqlCommandStep.DatabaseWebAgentName), logger,
            httpClientFactory, ParametersManager, true));

        FieldEditors.Add(new OneDatabaseNameFieldEditor(appName, logger, httpClientFactory,
            nameof(ExecuteSqlCommandStep.DatabaseName), ParametersManager,
            nameof(ExecuteSqlCommandStep.DatabaseServerConnectionName)));

        FieldEditors.Add(new TextFieldEditor(nameof(ExecuteSqlCommandStep.ExecuteQueryCommand)));
        FieldEditors.Add(new IntFieldEditor(nameof(ExecuteSqlCommandStep.CommandTimeOut), 1));
        FieldEditors.AddRange(tempFieldEditors);
    }
}
