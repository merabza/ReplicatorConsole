//using AppCliTools.CliMenu;
//using AppCliTools.CliMenu.CliMenuCommands;
//using AppCliTools.CliParameters.CliMenuCommands;
//using AppCliTools.CliTools;
//using AppCliTools.CliTools.CliMenuCommands;
//using AppCliTools.LibDataInput;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using ParametersManagement.LibParameters;
//using ReplicatorConsole.Menu;
//using ReplicatorConsole.MenuCommands;
//using ReplicatorConsole.StepCruders;
//using ReplicatorShared.Data.Models;
//using SystemTools.BackgroundTasks;

//namespace ReplicatorConsole;

//public sealed class ReplicatorCliAppLoop : CliAppLoop
//{
//    private readonly string _appName;
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly ILogger _logger;
//    private readonly ParametersManager _parametersManager;
//    private readonly IProcesses _processes;
//    private readonly ServiceProvider _serviceProvider;

//    // ReSharper disable once ConvertToPrimaryConstructor
//    public ReplicatorCliAppLoop(ServiceProvider serviceProvider, string appName, ILogger logger,
//        IHttpClientFactory httpClientFactory, ParametersManager parametersManager, IProcesses processes) : base(appName,
//        null, null, processes)
//    {
//        _appName = appName;
//        _logger = logger;
//        _httpClientFactory = httpClientFactory;
//        _parametersManager = parametersManager;
//        _processes = processes;
//        _serviceProvider = serviceProvider;
//    }

//    public override CliMenuSet BuildMainMenu()
//    {
//        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

//        CliMenuSet mainMenuSet = CliMenuSetFactory.CreateMenuSet("Main Menu", MenuData.MenuCommandNames,
//            _serviceProvider, _parametersManager);

//        //მონაცემთა ბაზების ბექაპირების ნაბიჯების სია
//        var databaseBackupStepCommand = new CruderListCliMenuCommand(new DatabaseBackupStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.DatabaseBackupSteps));
//        mainMenuSet.AddMenuItem(databaseBackupStepCommand);

//        //რამდენიმე ბაზის დამუშავების ბრძანებების სია
//        var multiDatabaseProcessStepCommand = new CruderListCliMenuCommand(new MultiDatabaseProcessStepCruder(_appName,
//            _logger, _httpClientFactory, _processes, _parametersManager, parameters.MultiDatabaseProcessSteps));
//        mainMenuSet.AddMenuItem(multiDatabaseProcessStepCommand);

//        //პროგრამის გაშვების ნაბიჯების სია
//        var runProgramStepCommand = new CruderListCliMenuCommand(new RunProgramStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.RunProgramSteps));
//        mainMenuSet.AddMenuItem(runProgramStepCommand);

//        //მონაცემთა ბაზების მხარეს გასაშვები ბრძანებების ნაბიჯების სია
//        var executeSqlCommandStepCommand = new CruderListCliMenuCommand(new ExecuteSqlCommandStepCruder(_appName,
//            _logger, _httpClientFactory, _processes, _parametersManager, parameters.ExecuteSqlCommandSteps));
//        mainMenuSet.AddMenuItem(executeSqlCommandStepCommand);

//        //ფაილების დაბეკაპების ნაბიჯების სია
//        var filesBackupStepCommand = new CruderListCliMenuCommand(new FilesBackupStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.FilesBackupSteps));
//        mainMenuSet.AddMenuItem(filesBackupStepCommand);

//        //ფაილების დასინქრონიზების ნაბიჯების სია
//        var filesSyncStepCommand = new CruderListCliMenuCommand(new FilesSyncStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.FilesSyncSteps));
//        mainMenuSet.AddMenuItem(filesSyncStepCommand);

//        //ფაილების გადაადგილების ნაბიჯების სია
//        var filesMoveStepCommand = new CruderListCliMenuCommand(new FilesMoveStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.FilesMoveSteps));
//        mainMenuSet.AddMenuItem(filesMoveStepCommand);

//        //ფაილების გადაადგილების ნაბიჯების სია
//        var unZipOnPlaceStepCommand = new CruderListCliMenuCommand(new UnZipOnPlaceStepCruder(_appName, _logger,
//            _httpClientFactory, _processes, _parametersManager, parameters.UnZipOnPlaceSteps));
//        mainMenuSet.AddMenuItem(unZipOnPlaceStepCommand);

//        //ნაბიჯების გასუფთავება
//        var generateStandardDatabaseStepsCliMenuCommand =
//            new GenerateStandardDatabaseStepsCliMenuCommand(_appName, _logger, _parametersManager);
//        mainMenuSet.AddMenuItem(generateStandardDatabaseStepsCliMenuCommand);

//        //ნაბიჯების გასუფთავება
//        var clearStepsCliMenuCommand = new ClearStepsCliMenuCommand(_parametersManager);
//        mainMenuSet.AddMenuItem(clearStepsCliMenuCommand);

//        //მთლიანი ფაილის გასუფთავება
//        var clearAllCliMenuCommand = new ClearAllCliMenuCommand(_parametersManager);
//        mainMenuSet.AddMenuItem(clearAllCliMenuCommand);

//        //გასასვლელი
//        string key = ConsoleKey.Escape.Value().ToUpperInvariant();
//        mainMenuSet.AddMenuItem(key, new ExitCliMenuCommand(), key.Length);

//        return mainMenuSet;
//    }
//}


