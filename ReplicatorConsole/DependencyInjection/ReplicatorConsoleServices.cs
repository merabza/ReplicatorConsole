using AppCliTools.CliMenu;
using AppCliTools.CliMenu.DependencyInjection;
using AppCliTools.CliTools.App;
using AppCliTools.CliTools.DependencyInjection;
using AppCliTools.CliTools.Menu.RecentCommandsList;
using AppCliTools.CliTools.Services.MenuBuilder;
using AppCliTools.CliTools.Services.RecentCommands;
using AppCliTools.CliTools.Services.RecentCommands.Models;
using Microsoft.Extensions.DependencyInjection;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Menu.ReplicatorParametersEdit;
using ReplicatorShared.Data.Models;
using Serilog.Events;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.DependencyInjection;

public static class ReplicatorConsoleServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, string appName,
        ReplicatorParameters par, string parametersFileName)
    {
        // @formatter:off
        services
            .AddSerilogLoggerService(LogEventLevel.Information, appName, par.LogFolder)
            .AddHttpClient()
            //.AddMemoryCache()
            //.AddSingleton<MenuParameters>()
            //.AddTransientAllStrategies<IMenuCommandListFactoryStrategy>(
            //    typeof(ProjectGroupsListFactoryStrategy).Assembly,
            //    typeof(RecentCommandsListFactoryStrategy).Assembly)
            .AddSingleton<IApplication, Application>()
            .AddSingleton<IProcesses, Processes>()
            .AddSingleton<IMenuBuilder, ReplicatorConsoleMenuBuilder>()
            .AddTransientAllStrategies<IMenuCommandFactoryStrategy>(
                typeof(ParametersEditorListCliMenuCommandFactoryStrategy).Assembly)
            //.AddTransientAllStrategies<IToolCommandFactoryStrategy>(
            //    typeof(CorrectNewDatabaseToolCommandFactoryStrategy).Assembly,
            //    typeof(JetBrainsCleanupCodeRunnerToolCommandFactoryStrategy).Assembly,
            //    typeof(JsonFromProjectDbProjectGetterFactoryStrategy).Assembly,
            //    typeof(GenerateApiRoutesToolCommandFactoryStrategy).Assembly,
            //    typeof(ApplicationSettingsEncoderToolCommandFactoryStrategy).Assembly)
            .AddApplication(x =>
            {
                x.AppName = appName;
            })
            .AddMainParametersManager(x =>
            {
                x.ParametersFileName = parametersFileName;
                x.Par = par;
            });

        // @formatter:on
        //services.AddRecentCommandsService(x =>
        //{
        //    x.RecentCommandsFileName = par.RecentCommandsFileName;
        //    x.RecentCommandsCount = par.RecentCommandsCount;
        //});

        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services,
        Action<ApplicationOptions> setupAction)
    {
        services.AddSingleton<IApplication, Application>();
        services.Configure(setupAction);
        return services;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IServiceCollection AddMainParametersManager(this IServiceCollection services,
        Action<MainParametersManagerOptions> setupAction)
    {
        services.AddSingleton<IParametersManager, ParametersManager>();
        services.Configure(setupAction);
        return services;
    }

    //private static IServiceCollection AddRecentCommandsService(this IServiceCollection services,
    //    Action<RecentCommandOptions> setupAction)
    //{
    //    services.AddSingleton<IRecentCommandsService, RecentCommandsService>();
    //    services.Configure(setupAction);
    //    return services;
    //}
}
