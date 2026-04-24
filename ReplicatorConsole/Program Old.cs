//using AppCliTools.CliParameters;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using ParametersManagement.LibParameters;
//using ReplicatorConsole;
//using ReplicatorShared.Data.Models;
//using Serilog;
//using Serilog.Events;
//using SystemTools.BackgroundTasks;
//using SystemTools.SystemToolsShared;

//ILogger<Program>? logger = null;
//try
//{
//    Console.WriteLine("Loading...");

//    const string appName = "Replicator Console";

//    ////პროგრამის ატრიბუტების დაყენება 
//    //ProgramAttributes.Instance.AppName = appName;

//    //string key = StringExtension.AppAgentAppKey + Environment.MachineName.Capitalize();

//    var argParser = new ArgumentsParser<ReplicatorParameters>(args, appName, null);
//    EParseResult parseResult = argParser.Analysis();

//    if (parseResult != EParseResult.Ok)
//    {
//        return 1;
//    }

//    var par = (ReplicatorParameters?)argParser.Par;
//    if (par is null)
//    {
//        StShared.WriteErrorLine("ReplicatorParameters is null", true);
//        return 3;
//    }

//    string? parametersFileName = argParser.ParametersFileName;
//    var replicatorServicesCreator = new ReplicatorServicesCreator(par);

//    // ReSharper disable once using
//    await using ServiceProvider? serviceProvider =
//        replicatorServicesCreator.CreateServiceProvider(LogEventLevel.Information);

//    if (serviceProvider == null)
//    {
//        Console.WriteLine("Logger not created");
//        return 8;
//    }

//    logger = serviceProvider.GetService<ILogger<Program>>();
//    if (logger is null)
//    {
//        StShared.WriteErrorLine("logger is null", true);
//        return 3;
//    }

//    var processesLogger = serviceProvider.GetService<ILogger<Processes>>();
//    if (processesLogger is null)
//    {
//        StShared.WriteErrorLine("processesLogger is null", true);
//        return 3;
//    }

//    var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
//    if (httpClientFactory is null)
//    {
//        StShared.WriteErrorLine("httpClientFactory is null", true);
//        return 6;
//    }

//    var processes = serviceProvider.GetService<IProcesses>();
//    if (processes is null)
//    {
//        StShared.WriteErrorLine("processes is null", true);
//        return 6;
//    }

//    var replicator = new ReplicatorCliAppLoop(serviceProvider, appName, logger, httpClientFactory,
//        new ParametersManager(parametersFileName, par), processes);
//    return await replicator.Run() ? 0 : 1;
//}
//catch (Exception e)
//{
//    StShared.WriteException(e, true, logger);
//    return 7;
//}
//finally
//{
//    await Log.CloseAndFlushAsync();
//}
