using AppCliTools.CliMenu;
using AppCliTools.LibDataInput;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.MenuCommands;

public sealed class ClearStepsCommand : CliMenuCommand
{
    private readonly ParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ClearStepsCommand(ParametersManager parametersManager) : base("Clear Steps", EMenuAction.Reload)
    {
        _parametersManager = parametersManager;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        if (!Inputer.InputBool("Clear Steps, are you sure?", false, false))
        {
            return false;
        }

        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        parameters.ClearSteps();
        await _parametersManager.Save(parameters, "Steps cleared success", null, cancellationToken);
        return true;
    }
}
