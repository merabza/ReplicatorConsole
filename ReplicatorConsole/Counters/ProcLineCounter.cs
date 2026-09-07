using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using OneOf;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using SystemTools.SharedKernel;
using SystemTools.SystemToolsShared;
using SystemTools.SystemToolsShared.Errors;
using ToolsManagement.DatabasesManagement;

namespace ReplicatorConsole.Counters;

public sealed class ProcLineCounter : SCounter
{
    private readonly string _appName;
    private readonly string _databaseServerConnectionName;
    private readonly string? _downloadFileStorageName;
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;
    private readonly string? _uploadFileStorageName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ProcLineCounter(string appName, ILogger logger, IParametersManager parametersManager,
        string databaseServerConnectionName, string? downloadFileStorageName,
        string? uploadFileStorageName) : base(parametersManager)
    {
        _appName = appName;
        _logger = logger;
        _parametersManager = parametersManager;
        _databaseServerConnectionName = databaseServerConnectionName;
        _downloadFileStorageName = downloadFileStorageName;
        _uploadFileStorageName = uploadFileStorageName;
    }

    private bool IsServerLocal()
    {
        if (string.IsNullOrWhiteSpace(_databaseServerConnectionName))
        {
            return false;
        }

        if (_parametersManager.Parameters is not IParametersWithDatabaseServerConnections parametersDsc)
        {
            return false;
        }

        Result<IDatabaseManager> createDatabaseManagerResult = DatabaseManagersFactory
            .CreateDatabaseManager(_appName, _logger, true, _databaseServerConnectionName,
                new DatabaseServerConnections(parametersDsc.DatabaseServerConnections), CancellationToken.None).Result;

        if (createDatabaseManagerResult.IsFailure)
        {
            createDatabaseManagerResult.Error.PrintErrorsOnConsole();
        }

        Result<bool> isServerLocalResult =
            createDatabaseManagerResult.Value.IsServerLocal(CancellationToken.None).Result;
        return isServerLocalResult is { IsSuccess: true, Value: true };
    }

    public int Count(EProcLineCase procLineCase)
    {
        bool isServerLocal = IsServerLocal();

        return procLineCase switch
        {
            EProcLineCase.Backup => 1,
            EProcLineCase.Download => isServerLocal || _downloadFileStorageName is null ||
                                      IsFileStorageLocal(_downloadFileStorageName)
                ? 1
                : 2,
            EProcLineCase.Archive => isServerLocal || _downloadFileStorageName is null ||
                                     IsFileStorageLocal(_downloadFileStorageName)
                ? 1
                : 3,
            EProcLineCase.Upload => _uploadFileStorageName is null || IsFileStorageLocal(_uploadFileStorageName)
                ? 1
                : 4,
            _ => throw new ArgumentOutOfRangeException(nameof(procLineCase), procLineCase, null)
        };
    }
}
