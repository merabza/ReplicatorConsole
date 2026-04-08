using System.Globalization;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibDataInput;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.FieldEditors;

public sealed class DateTimeFieldEditor : FieldEditor<DateTime>
{
    private readonly DateTime _defaultValue;

    public DateTimeFieldEditor(string propertyName, DateTime defaultValue, bool enterFieldDataOnCreate = false) : base(
        propertyName, enterFieldDataOnCreate)
    {
        _defaultValue = defaultValue;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        SetValue(recordForUpdate, Inputer.InputDateTime(FieldName, GetValue(recordForUpdate, _defaultValue)));
        return ValueTask.CompletedTask;
    }

    public override string GetValueStatus(object? record)
    {
        DateTime val = GetValue(record);
        return val.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        //სტანდარტული ფორმატის გადაყვანა custom ფორმატში
        //DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
        //var v = DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
        //ამ მასივი პირველი ელემენტი ემთხვევა სტანდარტულ ფორმატს. დანარჩენები ალბათ გამოიყენება სტრიქონის გაპარსვისას
    }

    public override void SetDefault(ItemData currentItem)
    {
        SetValue(currentItem, _defaultValue);
    }
}
