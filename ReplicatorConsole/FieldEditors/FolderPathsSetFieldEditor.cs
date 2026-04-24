using System.Collections.Generic;
using System.Linq;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.FieldEditors;
using ReplicatorConsole.Cruders;
using ReplicatorConsole.MenuCommands;

namespace ReplicatorConsole.FieldEditors;

public sealed class FolderPathsSetFieldEditor : FieldEditor<List<string>>
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public FolderPathsSetFieldEditor(string propertyName, bool enterFieldDataOnCreate = false) : base(propertyName,
        enterFieldDataOnCreate, null, true)
    {
    }

    public void Update(object recordForUpdate, List<string> data)
    {
        SetValue(recordForUpdate, data);
    }

    public override CliMenuSet GetSubMenu(object record)
    {
        List<string> currentValuesList = GetValue(record) ?? [];

        var folderPathsSetCruder = new FolderPathsSetCruder(currentValuesList, record, this);
        CliMenuSet foldersSet = folderPathsSetCruder.GetListMenu();

        foldersSet.InsertMenuItem(1, new MultiSelectSubfoldersCommand(currentValuesList, folderPathsSetCruder));
        return foldersSet;
    }

    public override string GetValueStatus(object? record)
    {
        List<string>? val = GetValue(record);

        if (val == null || val.Count == 0)
        {
            return "No Folders";
        }

        return val.Count != 1 ? $"{val.Count} folders" : val.Single();
    }
}
