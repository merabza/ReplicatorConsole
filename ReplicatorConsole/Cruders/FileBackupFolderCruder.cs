using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.Cruders;
using ParametersManagement.LibParameters;
using ReplicatorConsole.MenuCommands;

namespace ReplicatorConsole.Cruders;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class FileBackupFolderCruder : SimpleNamesWithDescriptionsCruder
{
    private readonly Dictionary<string, string> _currentValuesDictionary;
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public FileBackupFolderCruder(IParametersManager parametersManager,
        Dictionary<string, string> currentValuesDictionary) : base("Backup Folder Path", "Backup Folder Paths", "Path")
    {
        _currentValuesDictionary = currentValuesDictionary;
        _parametersManager = parametersManager;
    }

    protected override Dictionary<string, string> GetDictionary()
    {
        return _currentValuesDictionary;
    }

    protected override void FillListMenuAdditional(CliMenuSet cruderSubMenuSet)
    {
        var multiSelectSubfoldersWithMasksCommand =
            new MultiSelectSubfoldersWithMasksCommand(_currentValuesDictionary, this);
        cruderSubMenuSet.AddMenuItem(multiSelectSubfoldersWithMasksCommand);
    }

    //public override async ValueTask Save(string message, CancellationToken cancellationToken = default)
    //{
    //    UpdateRecordWithKey(_record, _currentValuesDictionary);
    //    await base.Save(message, cancellationToken);
    //}
    public override ValueTask Save(string message, CancellationToken cancellationToken = default)
    {
        return _parametersManager.Save(_parametersManager.Parameters, message, null, cancellationToken);
    }
}
