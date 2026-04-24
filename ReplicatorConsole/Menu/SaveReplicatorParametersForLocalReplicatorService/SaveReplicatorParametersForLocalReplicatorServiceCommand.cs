using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
using AppCliTools.LibDataInput;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;

public sealed class SaveReplicatorParametersForLocalReplicatorServiceCommand : CliMenuCommand
{
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SaveReplicatorParametersForLocalReplicatorServiceCommand(IParametersManager parametersManager) : base(
        "Save Replicator Parameters For Local Replicator Service", EMenuAction.Reload)
    {
        _parametersManager = parametersManager;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        //შევამოწმოთ პარამეტრებში გვაქვს თუ არა შევსებული პარამეტრების ფაილის სახელი რესერვერისათვის ReplicatorParametersFileNameForLocalReServer
        //თუ პარამეტრი არ არსებობს, გამოვიტანოთ შესაბამისი შეტყობინება და გავჩერდეთ
        if (string.IsNullOrWhiteSpace(parameters.ReplicatorParametersFileNameForLocalReplicatorService))
        {
            StShared.WriteErrorLine("file name for local reServer Parameters is empty. please enter it first", true);
            return false;
        }

        //შევამოწმოთ არსებობს თუ არა უკვე ეს ფაილი.
        //თუ უკვე არსებობს გამოვიტანოთ შეკითხვა იმის შესახებ, გადავაწეროთ თუ არა
        //თუ პასუხი უარყოფითი იქნება, გავჩერდეთ
        if (File.Exists(parameters.ReplicatorParametersFileNameForLocalReplicatorService) &&
            !Inputer.InputBool("Parameters file already exists, Rewrite?", false, false))
        {
            return false;
        }

        //შევინახოთ პარამეტრების ფაილი
        await _parametersManager.Save(parameters, "Parameters for ReServer Saved",
            parameters.ReplicatorParametersFileNameForLocalReplicatorService, cancellationToken);

        return true;
    }
}
