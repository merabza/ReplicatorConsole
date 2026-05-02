using System.Net.Http;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.StepCruders;
using ReplicatorShared.Data.Models;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.ExecuteSqlCommandStepCruderList;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy(
    ILogger<ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy> logger,
    IHttpClientFactory httpClientFactory,
    IApplication application,
    IProcesses processes,
    IParametersManager parametersManager) : IMenuCommandFactoryStrategy
{
    public CliMenuCommand CreateMenuCommand()
    {
        var parameters = (ReplicatorParameters)parametersManager.Parameters;

        return new CruderListCliMenuCommand(new ExecuteSqlCommandStepCruder(application, logger, httpClientFactory,
            processes, parametersManager, parameters.ExecuteSqlCommandSteps));
    }
}
