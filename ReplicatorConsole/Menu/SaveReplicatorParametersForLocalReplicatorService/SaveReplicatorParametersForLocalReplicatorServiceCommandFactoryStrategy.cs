using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;

namespace ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;

public class SaveReplicatorParametersForLocalReplicatorServiceCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IParametersManager _parametersManager;

    public SaveReplicatorParametersForLocalReplicatorServiceCommandFactoryStrategy(IParametersManager parametersManager)
    {
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        return new SaveReplicatorParametersForLocalReplicatorServiceCommand(_parametersManager);
    }
}
