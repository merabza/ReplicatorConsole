using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.MenuCommands;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.GenerateStandardDatabaseSteps;

// ReSharper disable once ClassNeverInstantiated.Global
public class GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy(
    ILogger<GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy> logger,
    IParametersManager parametersManager,
    IApplication application) : IMenuCommandFactoryStrategy
{
    public CliMenuCommand CreateMenuCommand()
    {
        return new GenerateStandardDatabaseStepsCliMenuCommand(application, logger,
            (ParametersManager)parametersManager);
    }
}
