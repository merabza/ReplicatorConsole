using AppCliTools.CliMenu;
using AppCliTools.LibDataInput;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.MenuCommands;

public sealed class ClearAllCommand : CliMenuCommand
{
    private readonly ParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ClearAllCommand(ParametersManager parametersManager) : base("Clear All", EMenuAction.Reload)
    {
        _parametersManager = parametersManager;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        if (!Inputer.InputBool("Clear All, are you sure?", false, false))
        {
            return false;
        }

        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        parameters.ClearAll();
        await _parametersManager.Save(parameters, "Data cleared success", null, cancellationToken);
        return true;
    }
}
