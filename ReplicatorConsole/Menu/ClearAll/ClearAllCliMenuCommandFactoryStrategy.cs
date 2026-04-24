using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;
using ReplicatorConsole.MenuCommands;

namespace ReplicatorConsole.Menu.ClearAll;

public class ClearAllCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IParametersManager _parametersManager;

    public ClearAllCliMenuCommandFactoryStrategy(IParametersManager parametersManager)
    {
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        return new ClearAllCliMenuCommand((ParametersManager)_parametersManager);
    }
}
