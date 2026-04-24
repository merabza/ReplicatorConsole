using System.Collections.Generic;
using AppCliTools.CliParameters.CliMenuCommands;
using ReplicatorConsole.Menu.ClearAll;
using ReplicatorConsole.Menu.ClearSteps;
using ReplicatorConsole.Menu.DatabaseBackupStepCruderList;
using ReplicatorConsole.Menu.ExecuteSqlCommandStepCruderList;
using ReplicatorConsole.Menu.FilesBackupStepCruderList;
using ReplicatorConsole.Menu.FilesMoveStepCruderList;
using ReplicatorConsole.Menu.FilesSyncStepCruderList;
using ReplicatorConsole.Menu.GenerateStandardDatabaseSteps;
using ReplicatorConsole.Menu.JobScheduleCruderList;
using ReplicatorConsole.Menu.MultiDatabaseProcessStepCruderList;
using ReplicatorConsole.Menu.ReplicatorParametersEdit;
using ReplicatorConsole.Menu.RunProgramStepCruderList;
using ReplicatorConsole.Menu.SaveReplicatorParametersForLocalReplicatorService;
using ReplicatorConsole.Menu.UnZipOnPlaceStepCruderList;

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
        nameof(JobScheduleCruderListCliMenuCommandFactoryStrategy),
        //მონაცემთა ბაზების ბექაპირების ნაბიჯების სია
        nameof(DatabaseBackupStepCruderListCliMenuCommandFactoryStrategy),
        //რამდენიმე ბაზის დამუშავების ბრძანებების სია
        nameof(MultiDatabaseProcessStepCruderListCliMenuCommandFactoryStrategy),
        //პროგრამის გაშვების ნაბიჯების სია
        nameof(RunProgramStepCruderListCliMenuCommandFactoryStrategy),
        //მონაცემთა ბაზების მხარეს გასაშვები ბრძანებების ნაბიჯების სია
        nameof(ExecuteSqlCommandStepCruderListCliMenuCommandFactoryStrategy),
        //ფაილების დაბეკაპების ნაბიჯების სია
        nameof(FilesBackupStepCruderListCliMenuCommandFactoryStrategy),
        //ფაილების დასინქრონიზების ნაბიჯების სია
        nameof(FilesSyncStepCruderListCliMenuCommandFactoryStrategy),
        //ფაილების გადაადგილების ნაბიჯების სია
        nameof(FilesMoveStepCruderListCliMenuCommandFactoryStrategy),
        //ფაილების გადაადგილების ნაბიჯების სია
        nameof(UnZipOnPlaceStepCruderListCliMenuCommandFactoryStrategy),
        //სტანდარტული ნაბიჯების დაგენერირება
        nameof(GenerateStandardDatabaseStepsCliMenuCommandFactoryStrategy),
        //ნაბიჯების გასუფთავება
        nameof(ClearStepsCliMenuCommandFactoryStrategy),
        //მთლიანი ფაილის გასუფთავება
        nameof(ClearAllCliMenuCommandFactoryStrategy)
    ];
}
