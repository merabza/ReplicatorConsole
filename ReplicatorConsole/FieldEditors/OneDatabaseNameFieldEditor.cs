using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
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

public sealed class OneDatabaseNameFieldEditor : FieldEditor<string>
{
    private readonly string _appName;
    private readonly string _databaseServerConnectionNamePropertyName;

    //private readonly string _databaseWebAgentNamePropertyName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public OneDatabaseNameFieldEditor(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        string propertyName, IParametersManager parametersManager, string databaseServerConnectionNamePropertyName,
        bool enterFieldDataOnCreate = false) : base(propertyName, enterFieldDataOnCreate)
    {
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _parametersManager = parametersManager;
        _databaseServerConnectionNamePropertyName = databaseServerConnectionNamePropertyName;
    }

    public override ValueTask UpdateField(string? recordKey, object recordForUpdate,
        CancellationToken cancellationToken = default)
    {
        string? databaseServerConnectionName =
            GetValue<string>(recordForUpdate, _databaseServerConnectionNamePropertyName);

        List<DatabaseInfoModel> dbList = CreateDbList(databaseServerConnectionName);

        string? currentDatabaseName = GetValue(recordForUpdate);

        var listSet = new CliMenuSet();
        foreach (string listItem in dbList.Select(s => s.Name))
        {
            listSet.AddMenuItem(new CliMenuCommand(listItem));
        }

        int selectedId = MenuInputer.InputIdFromMenuList(PropertyName.Pluralize(), listSet, currentDatabaseName);
        if (selectedId >= 0 && selectedId < dbList.Count)
        {
            SetValue(recordForUpdate, dbList[selectedId].Name);
        }

        return ValueTask.CompletedTask;
    }

    private List<DatabaseInfoModel> CreateDbList(string? databaseServerConnectionName)
    {
        var dbList = new List<DatabaseInfoModel>();

        if (_parametersManager.Parameters is not IParametersWithDatabaseServerConnectionsAndApiClients parameters)
        {
            Console.WriteLine("Parameters is invalid");
            return dbList;
        }

        OneOf<IDatabaseManager, Error[]> createDatabaseManagerResult = DatabaseManagersFactory
            .CreateDatabaseManager(_appName, _logger, true, databaseServerConnectionName,
                new DatabaseServerConnections(parameters.DatabaseServerConnections), null, _httpClientFactory, null,
                null, CancellationToken.None).Result;

        if (createDatabaseManagerResult.IsT1)
        {
            StShared.WriteErrorLine(
                $"DatabaseManagementClient does not created for webAgent {databaseServerConnectionName}", true,
                _logger);
            dbList = [];
        }
        else
        {
            var databasesListCreator = new DatabasesListCreator(EDatabaseSet.AllDatabases,
                createDatabaseManagerResult.AsT0, EBackupType.Full);
            dbList = databasesListCreator.LoadDatabaseNames(CancellationToken.None).Result;
        }

        return dbList;
    }
}
