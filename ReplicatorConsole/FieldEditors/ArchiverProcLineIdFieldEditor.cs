using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibDataInput;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.FieldEditors;

public sealed class ArchiverProcLineIdFieldEditor : FieldEditor<int>
{
    private readonly string _archiverNamePropertyName;
    private readonly int _defaultValue;

    public ArchiverProcLineIdFieldEditor(string propertyName, int defaultValue, string archiverNamePropertyName,
        bool enterFieldDataOnCreate = false) : base(propertyName, enterFieldDataOnCreate)
    {
        _defaultValue = defaultValue;
        _archiverNamePropertyName = archiverNamePropertyName;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        string? archiverName = GetValue<string>(recordForUpdate, _archiverNamePropertyName);

        SetValue(recordForUpdate,
            Inputer.InputInt(FieldName, archiverName == null ? 1 : GetValue(recordForUpdate, _defaultValue)));

        return ValueTask.CompletedTask;
    }

    public override void SetDefault(ItemData currentItem)
    {
        SetValue(currentItem, _defaultValue);
    }
}
