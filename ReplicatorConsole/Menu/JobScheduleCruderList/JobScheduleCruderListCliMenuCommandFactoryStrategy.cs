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

    public JobScheduleCruderListCliMenuCommandFactoryStrategy(
        ILogger<JobScheduleCruderListCliMenuCommandFactoryStrategy> logger, IHttpClientFactory httpClientFactory,
        IApplication application, IProcesses processes)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _application = application;
        _processes = processes;
    }

    public string MenuCommandName => JobScheduleCruder.MenuCommandName;

    public CliMenuCommand CreateMenuCommand(IParametersManager parametersManager)
    {
        var parameters = (ReplicatorParameters)parametersManager.Parameters;

        return new CruderListCliMenuCommand(new JobScheduleCruder(_application.Name, _logger, _httpClientFactory,
            parametersManager, parameters.JobSchedules, _processes));
    }
}
