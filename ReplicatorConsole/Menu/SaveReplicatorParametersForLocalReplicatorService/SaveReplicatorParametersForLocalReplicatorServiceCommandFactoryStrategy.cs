using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;

namespace ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;

public class SaveReplicatorParametersForLocalReplicatorServiceCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    public string MenuCommandName => SaveReplicatorParametersForLocalReplicatorServiceCommand.MenuCommandName;

    public CliMenuCommand CreateMenuCommand(IParametersManager parametersManager)
    {
        return new SaveReplicatorParametersForLocalReplicatorServiceCommand(parametersManager);
    }
}
