using AppCliTools.CliMenu;
using AppCliTools.LibMenuInput;
using ReplicatorConsole.Cruders;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.MenuCommands;

public sealed class MultiSelectSubfoldersCommand : CliMenuCommand
{
    private readonly FolderPathsSetCruder _folderPathsSetCruder;
    private readonly List<string> _masksAndFolders;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MultiSelectSubfoldersCommand(List<string> masksAndFolders, FolderPathsSetCruder folderPathsSetCruder) : base(
        "Multi Select Subfolders", EMenuAction.Reload)
    {
        _masksAndFolders = masksAndFolders;
        _folderPathsSetCruder = folderPathsSetCruder;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        string? folderName =
            MenuInputer.InputFolderPath("Folder which subfolders you wont to add to backups folders list");
        if (string.IsNullOrWhiteSpace(folderName))
        {
            return false;
        }

        DirectoryInfo? dir = CheckFolder(folderName);
        if (dir == null)
        {
            return false;
        }

        //დადგინდეს ამ ფოლდერებიდან რომელიმე არის თუ არა დასაბექაპებელ სიაში. და თუ არის მისთვის ჩაირთოს ჭეშმარიტი
        Dictionary<string, bool> foldersChecks = dir.GetDirectories().OrderBy(o => o.Name)
            .ToDictionary(k => k.Name, v => _masksAndFolders.Contains(v.FullName));
        //გამოვიდეს სიიდან ამრჩევი
        MenuInputer.MultipleInputFromList($"Select subfolders from {folderName}", foldersChecks);

        foreach (KeyValuePair<string, bool> kvp in foldersChecks)
        {
            string path = Path.Combine(folderName, kvp.Key);
            if (kvp.Value)
            {
                //ჩართული ჩავამატოთ თუ არ არსებობს
                if (!_masksAndFolders.Contains(path))
                {
                    _masksAndFolders.Add(path);
                }
            }
            else
            {
                //გამორთული ამოვაკლოთ თუ არსებობს
                _masksAndFolders.Remove(path);
            }
        }

        await _folderPathsSetCruder.Save("Changes saved", cancellationToken);

        return true;
    }

    private DirectoryInfo? CheckFolder(string folderName)
    {
        var dir = new DirectoryInfo(folderName);

        if (!dir.Exists)
        {
            StShared.WriteErrorLine($"folder {folderName} does not exists", true);
            return null;
        }

        //დადგინდეს ეს ფოლდერი თვითონ და ამ ფოლდერის რომელიმე მშობელი ფოლდერი არის თუ არა დასაბექაპებელ სიაში.
        //და თუ არის, მაშინ არ დავუშვათ ამ ფოლდერის ქვეფოლდერების სიის გამოძახება.
        //მიზეზი კი გამოვიტანოთ შეტყობინების სახით
        //არ შეიძლება ქვეფოლდერის არჩევა დასაბეკაპებლად, თუ მისი წინაპარი უკვე არჩეულია. ჯერ ამოიღეთ სიიდან წინაპარი
        if (_masksAndFolders.Any(x => Contains(x, dir.FullName)))
        {
            StShared.WriteErrorLine($"folder {folderName} can not use because of existing backup folders", true);
            return null;
        }

        if (dir.GetDirectories().Length > 0)
        {
            return dir;
        }

        StShared.WriteErrorLine($"folder {folderName} have not subfolders", true);
        return null;
    }

    private static bool Contains(DirectoryInfo di1, DirectoryInfo di2)
    {
        while (true)
        {
            if (di2.FullName == di1.FullName)
            {
                return true;
            }

            if (di2.Parent == null)
            {
                return false;
            }

            di2 = di2.Parent;
        }
    }

    private static bool Contains(string dir1, string dir2)
    {
        var di1 = new DirectoryInfo(dir1);
        var di2 = new DirectoryInfo(dir2);
        return Contains(di1, di2);
    }
}
