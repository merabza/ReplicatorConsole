using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliParameters.Cruders;
using AppCliTools.CliParameters.FieldEditors;
using ReplicatorConsole.FieldEditors;
using ReplicatorConsole.Models;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Cruders;

public sealed class FolderPathsSetCruder : Cruder
{
    private readonly List<string> _currentValuesList;
    private readonly FolderPathsSetFieldEditor _folderPathsSetFieldEditor;
    private readonly object _record;

    public FolderPathsSetCruder(List<string> currentValuesList, object record,
        FolderPathsSetFieldEditor folderPathsSetFieldEditor) : base("Priority Folder Path", "Priority Folder Paths",
        true, false)
    {
        _currentValuesList = currentValuesList;
        _record = record;
        _folderPathsSetFieldEditor = folderPathsSetFieldEditor;
        FieldEditors.Add(new FolderPathFieldEditor(nameof(FolderPathItemData.Path)));
    }

    protected override Dictionary<string, ItemData> GetCrudersDictionary()
    {
        return _currentValuesList.ToDictionary(p => p, ItemData (p) => new FolderPathItemData { Path = p });
    }

    protected override ItemData CreateNewItem(string? recordKey, ItemData? defaultItemData)
    {
        return new FolderPathItemData();
    }

    public override bool ContainsRecordWithKey(string recordKey)
    {
        return _currentValuesList.Contains(recordKey);
    }

    protected override ValueTask RemoveRecordWithKey(string recordKey, CancellationToken cancellationToken = default)
    {
        _currentValuesList.Remove(recordKey);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask AddRecordWithKey(string recordKey, ItemData newRecord,
        CancellationToken cancellationToken = default)
    {
        string? newPath = ((FolderPathItemData)newRecord).Path;
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            _currentValuesList.Add(newPath);
        }

        return ValueTask.CompletedTask;
    }

    public override async ValueTask Save(string message, CancellationToken cancellationToken = default)
    {
        _folderPathsSetFieldEditor.Update(_record, _currentValuesList);
        await base.Save(message, cancellationToken);
    }
}
