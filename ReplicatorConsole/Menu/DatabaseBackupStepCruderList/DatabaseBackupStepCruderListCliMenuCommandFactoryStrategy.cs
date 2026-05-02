using System.Net.Http;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.StepCruders;
using ReplicatorShared.Data.Models;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.DatabaseBackupStepCruderList;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class DatabaseBackupStepCruderListCliMenuCommandFactoryStrategy(
    ILogger<DatabaseBackupStepCruderListCliMenuCommandFactoryStrategy> logger,
    IHttpClientFactory httpClientFactory,
    IApplication application,
    IProcesses processes,
    IParametersManager parametersManager) : IMenuCommandFactoryStrategy
{
    public CliMenuCommand CreateMenuCommand()
    {
        var parameters = (ReplicatorParameters)parametersManager.Parameters;

        return new CruderListCliMenuCommand(new DatabaseBackupStepCruder(application, logger, httpClientFactory,
            processes, parametersManager, parameters.DatabaseBackupSteps));
    }
}
