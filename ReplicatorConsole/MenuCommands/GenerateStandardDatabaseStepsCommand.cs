using AppCliTools.CliMenu;
using AppCliTools.CliParametersDataEdit.Cruders;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Generators;
using ReplicatorShared.Data.Models;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.MenuCommands;

public sealed class GenerateStandardDatabaseStepsCommand : CliMenuCommand
{
    private readonly string _appName;
    private readonly ILogger _logger;
    private readonly ParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public GenerateStandardDatabaseStepsCommand(string appName, ILogger logger, ParametersManager parametersManager) :
        base("Generate Standard Database Jobs...", EMenuAction.Reload)
    {
        _appName = appName;
        _parametersManager = parametersManager;
        _logger = logger;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var databaseServerConnectionCruder =
            DatabaseServerConnectionCruder.Create(_appName, _logger, null, _parametersManager);

        string? databaseConnectionName = await databaseServerConnectionCruder.GetNameWithPossibleNewName(
            "Select local connection for Generate standard database maintenance schema", null, null, false,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(databaseConnectionName))
        {
            StShared.WriteErrorLine("databaseConnectionName does not specified", true);
            return false;
        }

        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        var standardJobsSchemaGenerator = new StandardJobsSchemaGenerator(_appName, true, _logger, _parametersManager,
            databaseConnectionName, _parametersManager.ParametersFileName);
        await standardJobsSchemaGenerator.Generate(cancellationToken);

        //შენახვა
        await _parametersManager.Save(parameters, "Maintain schema generated success", null, cancellationToken);
        return true;
    }
}
