using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;
using ReplicatorConsole.MenuCommands;

namespace ReplicatorConsole.Menu.ClearSteps;

public class ClearStepsCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IParametersManager _parametersManager;

    public ClearStepsCliMenuCommandFactoryStrategy(IParametersManager parametersManager)
    {
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        return new ClearStepsCliMenuCommand((ParametersManager)_parametersManager);
    }
}
