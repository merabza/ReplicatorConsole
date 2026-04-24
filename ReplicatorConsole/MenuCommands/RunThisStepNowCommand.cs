using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.MenuCommands;

public sealed class RunThisStepNowCommand : CliMenuCommand
{
    private readonly string _appName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JobStep _jobStep;
    private readonly ILogger _logger;
    private readonly string? _parametersFileName;
    private readonly IParametersManager _parametersManager;
    private readonly IProcesses _processes;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RunThisStepNowCommand(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IProcesses processes, IParametersManager parametersManager, JobStep jobStep, string? parametersFileName) : base(
        "Run this step now...", EMenuAction.Reload)
    {
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _processes = processes;
        _parametersManager = parametersManager;
        _jobStep = jobStep;
        _parametersFileName = parametersFileName;
    }

    protected override ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        string? procLogFilesFolder =
            parameters.CountLocalPath(parameters.ProcLogFilesFolder, _parametersFileName, "ProcLogFiles");

        if (string.IsNullOrWhiteSpace(procLogFilesFolder))
        {
            StShared.WriteErrorLine("procLogFilesFolder does not counted. step does not started", true, _logger);
            return new ValueTask<bool>(false);
        }

        // ReSharper disable once using
        using ProcessManager processManager = _processes.GetNewProcessManager();

        ProcessesToolAction? stepToolAction = _jobStep.GetToolAction(_appName, _logger, _httpClientFactory, true,
            processManager, parameters, procLogFilesFolder);

        if (stepToolAction is null)
        {
            StShared.WriteErrorLine("stepToolAction does not found. step does not started", true, _logger);
            return new ValueTask<bool>(false);
        }

        processManager.Run(stepToolAction);
        return new ValueTask<bool>(true);
    }
}
