using System.Collections.Generic;
using AppCliTools.CliParameters.Cruders;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.Cruders;

public sealed class ReplacePairsSetFileMaskCruder : SimpleNamesWithDescriptionsCruder
{
    private readonly IParametersManager _parametersManager;
    private readonly string _replacePairSetName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ReplacePairsSetFileMaskCruder(IParametersManager parametersManager, string replacePairSetName) : base(
        "Replace Mask", "Replace Masks")
    {
        _parametersManager = parametersManager;
        _replacePairSetName = replacePairSetName;
    }

    protected override Dictionary<string, string> GetDictionary()
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;
        Dictionary<string, ReplacePairsSet> replacePairSets = parameters.ReplacePairsSets;
        return replacePairSets.TryGetValue(_replacePairSetName, out ReplacePairsSet? replacePairSet)
            ? replacePairSet.PairsDict
            : [];
    }
}
