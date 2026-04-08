using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.Cruders;
using ParametersManagement.LibParameters;

namespace ReplicatorConsole.FieldEditors;

public sealed class ArchiverFieldEditor : FieldEditor<string>
{
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ArchiverFieldEditor(string propertyName, IParametersManager parametersManager,
        bool enterFieldDataOnCreate = false) : base(propertyName, enterFieldDataOnCreate)
    {
        _parametersManager = parametersManager;
    }

    public override async ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        var archiverCruder = ArchiverCruder.Create(_parametersManager);
        List<string> keys = archiverCruder.GetKeys();
        string? def = keys.Count > 1 ? null : archiverCruder.GetKeys().SingleOrDefault();
        SetValue(recordForUpdate,
            await archiverCruder.GetNameWithPossibleNewName(FieldName, GetValue(recordForUpdate, def), null, true,
                cancellationToken));
    }
}
