using ReplicatorConsole.Menu.JobScheduleCruderList;
using ReplicatorConsole.Menu.ReplicatorParametersEdit;
using ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;

namespace ReplicatorConsole.Menu;

public static class MenuData
{
    public static List<string> MenuCommandNames { get; } =
    [
        //ძირითადი პარამეტრების რედაქტირება
        ReplicatorParametersEditor.MenuCommandName,
        //პარამეტრების ფაილის შენახვა ლოკალური სერვისისთვის
        SaveReplicatorParametersForLocalReplicatorServiceCommand.MenuCommandName,
        //სამუშაოების დროის დაგეგმვების სია
        JobScheduleCruder.MenuCommandName
    ];
}
