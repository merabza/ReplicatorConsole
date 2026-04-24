using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.MenuCommands;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.GenerateStandardDatabaseSteps;

// ReSharper disable once ClassNeverInstantiated.Global
public class GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IApplication _application;
    private readonly ILogger<GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;

    public GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy(
        ILogger<GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy> logger,
        IParametersManager parametersManager, IApplication application)
    {
        _logger = logger;
        _parametersManager = parametersManager;
        _application = application;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        return new GenerateStandardDatabaseStepsCliMenuCommand(_application.AppName, _logger,
            (ParametersManager)_parametersManager);
    }
}
