using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;

namespace ReplicatorConsole;

public interface IMenuCommandFactoryStrategy
{
    string MenuCommandName { get; }
    CliMenuCommand? CreateMenuCommand(IParametersManager parametersManager);
}
