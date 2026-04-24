using ReplicatorConsole.Menu.JobScheduleCruderList;
using ReplicatorConsole.Menu.ReplicatorParametersEdit;
using ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;

namespace ReplicatorConsole.Menu;

public static class MenuData
{
    public static List<string> MainMenuCommandFactoryStrategyNames { get; } =
    [
        //ძირითადი პარამეტრების რედაქტირება
        nameof(ParametersEditorListCliMenuCommandFactoryStrategy),
        //პარამეტრების ფაილის შენახვა ლოკალური სერვისისთვის
        nameof(SaveReplicatorParametersForLocalReplicatorServiceCommandFactoryStrategy),
        //სამუშაოების დროის დაგეგმვების სია
        nameof(JobScheduleCruderListCliMenuCommandFactoryStrategy)
    ];
}
