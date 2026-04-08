using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.LibMenuInput;
using DatabaseTools.DbTools;
using DatabaseTools.DbTools.Models;
using Microsoft.Extensions.Logging;
using OneOf;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data;
using ReplicatorShared.Data.Models;
using SystemTools.SystemToolsShared;
using SystemTools.SystemToolsShared.Errors;
using ToolsManagement.DatabasesManagement;

namespace ReplicatorConsole.FieldEditors;

public sealed class DatabaseNamesFieldEditor : FieldEditor<List<string>>
{
    private readonly string _appName;
    private readonly string _databaseServerConnectionNamePropertyName;
    private readonly string _databaseSetPropertyName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DatabaseNamesFieldEditor(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        string propertyName, IParametersManager parametersManager, string databaseServerConnectionNamePropertyName,
        string databaseSetPropertyName) : base(propertyName)
    {
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _parametersManager = parametersManager;
        _databaseServerConnectionNamePropertyName = databaseServerConnectionNamePropertyName;
        _databaseSetPropertyName = databaseSetPropertyName;
    }

    public override async ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        string? databaseServerConnectionName =
            GetValue<string>(recordForUpdate, _databaseServerConnectionNamePropertyName);

        var databaseSet = GetValue<EDatabaseSet>(recordForUpdate, _databaseSetPropertyName);

        const EBackupType backupType = EBackupType.Full;

        if (_parametersManager.Parameters is not IParametersWithDatabaseServerConnections parameters)
        {
            StShared.WriteErrorLine("Parameters is invalid", true);
            return;
        }

        List<DatabaseInfoModel> dbList;

        OneOf<IDatabaseManager, Error[]> createDatabaseManagerResult =
            await DatabaseManagersFactory.CreateDatabaseManager(_appName, _logger, true, databaseServerConnectionName,
                new DatabaseServerConnections(parameters.DatabaseServerConnections), null, _httpClientFactory, null,
                null, cancellationToken);

        if (createDatabaseManagerResult.IsT1)
        {
            Error.PrintErrorsOnConsole(createDatabaseManagerResult.AsT1);
            StShared.WriteErrorLine(
                $"DatabaseManagementClient does not created for database Server Connection {databaseServerConnectionName}",
                true, _logger);
            dbList = [];
        }
        else
        {
            var databasesListCreator =
                new DatabasesListCreator(databaseSet, createDatabaseManagerResult.AsT0, backupType);
            dbList = await databasesListCreator.LoadDatabaseNames(cancellationToken);
        }

        if (databaseSet != EDatabaseSet.DatabasesBySelection)
        {
            Console.WriteLine("Databases list is:");
            int i = 0;
            foreach (DatabaseInfoModel databaseInfoModel in dbList.OrderBy(o => o.IsSystemDatabase)
                         .ThenBy(tb => tb.Name))
            {
                i++;
                Console.WriteLine($"{i}. {databaseInfoModel.Name}");
            }
        }
        else
        {
            List<string> oldDatabaseNames = GetValue(recordForUpdate, []) ?? [];
            Dictionary<string, bool> oldDatabaseChecks = dbList.ToDictionary(
                databaseInfoModel => databaseInfoModel.Name,
                databaseInfoModel => oldDatabaseNames.Contains(databaseInfoModel.Name));
            SetValue(recordForUpdate, MenuInputer.MultipleInputFromList(FieldName, oldDatabaseChecks));
        }
    }

    public override string GetValueStatus(object? record)
    {
        List<string>? val = GetValue(record);
        return val is null ? string.Empty : string.Join(",", val);
    }
}
