//using AppCliTools.CliMenu;
//using Microsoft.Extensions.DependencyInjection;
//using ReplicatorConsole.Menu.ReplicatorParametersEdit;
//using ReplicatorShared.Data.Models;
//using SystemTools.BackgroundTasks;
//using SystemTools.SystemToolsShared;

//namespace ReplicatorConsole;

//public sealed class ReplicatorServicesCreator : ServicesCreator
//{
//    // ReSharper disable once ConvertToPrimaryConstructor
//    public ReplicatorServicesCreator(ReplicatorParameters par) : base(par.LogFolder, null, "Replicator")
//    {
//    }

//    protected override void ConfigureServices(IServiceCollection services)
//    {
//        base.ConfigureServices(services);
//        services.AddHttpClient();

//        services.AddTransientAllMenuCommandFactoryStrategies(typeof(ParametersEditorListCliMenuCommandFactoryStrategy)
//            .Assembly);

//        services.AddSingleton<IApplication, ReplicatorApplication>();
//        services.AddSingleton<IProcesses, Processes>();
//    }
//}


