using System.Collections.Generic;
using System.Linq;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters;
using AppCliTools.CliParameters.CliMenuCommands;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.Cruders;

public sealed class ReplacePairsSetCruder : ParCruder<ReplacePairsSet>
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public ReplacePairsSetCruder(IParametersManager parametersManager,
        Dictionary<string, ReplacePairsSet> currentValuesDictionary) : base(parametersManager, currentValuesDictionary,
        "Replace Pairs Set", "Replace Pairs Sets", true)
    {
    }

    public static ReplacePairsSetCruder Create(IParametersManager parametersManager)
    {
        var parameters = (ReplicatorParameters)parametersManager.Parameters;
        return new ReplacePairsSetCruder(parametersManager, parameters.ReplacePairsSets);
    }

    //public საჭიროა Replicator პროექტისათვის
    public override void FillDetailsSubMenu(CliMenuSet itemSubMenuSet, string itemName)
    {
        base.FillDetailsSubMenu(itemSubMenuSet, itemName);

        var parameters = (ReplicatorParameters)ParametersManager.Parameters;
        Dictionary<string, ReplacePairsSet> replacePairsSets = parameters.ReplacePairsSets;
        ReplacePairsSet replacePairsSet = replacePairsSets[itemName];

        var detailsCruder = new ReplacePairsSetFileMaskCruder(ParametersManager, itemName);

        //var detailsCruder = new SimpleNamesWithDescriptionsFieldEditor<ReactAppTypeCruder>(
        //    nameof(ReplacePairsSet.PairsDict), ParametersManager);

        var newItemCommand = new NewItemCliMenuCommand(detailsCruder, itemName, $"Create New {detailsCruder.CrudName}");
        itemSubMenuSet.AddMenuItem(newItemCommand);

        foreach (ItemSubMenuCliMenuCommand detailListCommand in replacePairsSet.PairsDict.Select(mask =>
                     new ItemSubMenuCliMenuCommand(detailsCruder, mask.Key, itemName, true)))
        {
            itemSubMenuSet.AddMenuItem(detailListCommand);
        }
    }
}
