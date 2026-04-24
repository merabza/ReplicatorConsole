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

public sealed class ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IApplication _application;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;
    private readonly IProcesses _processes;

    public ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy(
        ILogger<ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy> logger,
        IHttpClientFactory httpClientFactory, IApplication application, IProcesses processes,
        IParametersManager parametersManager)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _application = application;
        _processes = processes;
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        return new CruderListCliMenuCommand(new ExecuteSqlCommandStepCruder(_application.AppName, _logger,
            _httpClientFactory, _processes, _parametersManager, parameters.ExecuteSqlCommandSteps));
    }
}
