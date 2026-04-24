using System.Net.Http;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.JobScheduleCruderList;

public class JobScheduleCruderListCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IApplication _application;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<JobScheduleCruderListCliMenuCommandFactoryStrategy> _logger;
    private readonly IProcesses _processes;
    private readonly IParametersManager _parametersManager;

    public JobScheduleCruderListCliMenuCommandFactoryStrategy(
        ILogger<JobScheduleCruderListCliMenuCommandFactoryStrategy> logger, IHttpClientFactory httpClientFactory,
        IApplication application, IProcesses processes, IParametersManager parametersManager)
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

        return new CruderListCliMenuCommand(new JobScheduleCruder(_application.AppName, _logger, _httpClientFactory,
            _parametersManager, parameters.JobSchedules, _processes));
    }
}
