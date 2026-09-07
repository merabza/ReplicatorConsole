using System.Threading;
using DatabaseTools.DbTools.Models;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using SystemTools.SharedKernel;
using SystemTools.SystemToolsShared;
using ToolsManagement.DatabasesManagement;

namespace ReplicatorConsole.Counters;

public sealed class StepNamePrefixCounter
{
    private readonly string _appName;
    private readonly string _databaseServerConnectionName;
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public StepNamePrefixCounter(string appName, ILogger logger, IParametersManager parametersManager,
        string databaseServerConnectionName)
    {
        _appName = appName;
        _logger = logger;
        _parametersManager = parametersManager;
        _databaseServerConnectionName = databaseServerConnectionName;
    }

    public string Count()
    {
        var parameters = (IParametersWithDatabaseServerConnections)_parametersManager.Parameters;

        Result<IDatabaseManager> createDatabaseManagerResult = DatabaseManagersFactory.CreateDatabaseManager(_appName,
            _logger, true, _databaseServerConnectionName,
            new DatabaseServerConnections(parameters.DatabaseServerConnections), CancellationToken.None).Result;

        if (createDatabaseManagerResult.IsFailure)
        {
            createDatabaseManagerResult.Error.PrintErrorsOnConsole();
        }

        Result<DbServerInfo> getDatabaseServerInfoResult =
            createDatabaseManagerResult.Value.GetDatabaseServerInfo(CancellationToken.None).Result;
        if (getDatabaseServerInfoResult.IsSuccess)
        {
            return getDatabaseServerInfoResult.Value.ServerName ?? string.Empty;
        }

        return string.Empty;
    }
}
