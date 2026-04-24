using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliParameters.FieldEditors;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Cruders;

namespace ReplicatorConsole.FieldEditors;

public sealed class ReplacePairsSetNameFieldEditor : FieldEditor<string>
{
    private readonly IParametersManager _parametersManager;
    private readonly bool _useNone;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ReplacePairsSetNameFieldEditor(string propertyName, IParametersManager parametersManager, bool useNone,
        bool enterFieldDataOnCreate = false) : base(propertyName, enterFieldDataOnCreate)
    {
        _parametersManager = parametersManager;
        _useNone = useNone;
    }

    public override async ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        var replacePairsSetCruderCruder = ReplacePairsSetCruder.Create(_parametersManager);
        SetValue(recordForUpdate,
            await replacePairsSetCruderCruder.GetNameWithPossibleNewName(FieldName, GetValue(recordForUpdate), null,
                _useNone, cancellationToken));
    }
}
