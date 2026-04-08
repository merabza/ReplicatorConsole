using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.MenuCommands;

public sealed class RunAllStepsNowCommand : CliMenuCommand
{
    private readonly string _appName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _jobScheduleName;
    private readonly ILogger _logger;
    private readonly string? _parametersFileName;
    private readonly IParametersManager _parametersManager;
    private readonly IProcesses _processes;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RunAllStepsNowCommand(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager, string jobScheduleName,
        string? parametersFileName) : base("Run All steps from this schedule now...", EMenuAction.Reload)
    {
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _processes = processes;
        _parametersManager = parametersManager;
        _jobScheduleName = jobScheduleName;
        _parametersFileName = parametersFileName;
    }

    protected override ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        string? procLogFilesFolder =
            parameters.CountLocalPath(parameters.ProcLogFilesFolder, _parametersFileName, "ProcLogFiles");

        if (!string.IsNullOrWhiteSpace(procLogFilesFolder))
        {
            return ValueTask.FromResult(parameters.RunAllSteps(_appName, _logger, _httpClientFactory, true,
                _jobScheduleName, _processes, procLogFilesFolder));
        }

        StShared.WriteErrorLine("procLogFilesFolder does not counted. cannot run steps", true, _logger);
        return ValueTask.FromResult(false);
    }
}
