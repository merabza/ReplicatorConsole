using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersDataEdit.Cruders;
using AppCliTools.CliParametersEdit.Cruders;
using AppCliTools.CliParametersExcludeSetsEdit.Cruders;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Cruders;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.Menu.ReplicatorParametersEdit;

public sealed class ReplicatorParametersEditor : ParametersEditor
{
    public ReplicatorParametersEditor(ILogger logger, IHttpClientFactory httpClientFactory, IParameters parameters,
        IParametersManager parametersManager) : base("Replicator Parameters Editor", parameters, parametersManager)
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(ReplicatorParameters.LogFolder)));
        FieldEditors.Add(new FolderPathFieldEditor(nameof(ReplicatorParameters.WorkFolder)));
        FieldEditors.Add(new FolderPathFieldEditor(nameof(ReplicatorParameters.ProcLogFilesFolder)));
        FieldEditors.Add(
            new FilePathFieldEditor(nameof(ReplicatorParameters
                .ReplicatorParametersFileNameForLocalReplicatorService)));

        FieldEditors.Add(new TextFieldEditor(nameof(ReplicatorParameters.UploadFileTempExtension),
            ReplicatorParameters.DefaultUploadFileTempExtension));
        FieldEditors.Add(new TextFieldEditor(nameof(ReplicatorParameters.DownloadFileTempExtension),
            ReplicatorParameters.DefaultDownloadFileTempExtension));
        FieldEditors.Add(new TextFieldEditor(nameof(ReplicatorParameters.ArchivingFileTempExtension),
            ReplicatorParameters.DefaultArchivingFileTempExtension));
        FieldEditors.Add(new TextFieldEditor(nameof(ReplicatorParameters.DateMask),
            ReplicatorParameters.DefaultDateMask));

        //FieldEditors.Add(new DatabaseServerConnectionsFieldEditor(logger, httpClientFactory, parametersManager,
        //    nameof(ReplicatorParameters.DatabaseServerConnections)));
        FieldEditors.Add(new DictionaryFieldEditor<DatabaseServerConnectionCruder, DatabaseServerConnectionData>(
            nameof(ReplicatorParameters.DatabaseServerConnections), logger, httpClientFactory, parametersManager));

        //FieldEditors.Add(new ApiClientsFieldEditor(logger, httpClientFactory, nameof(ReplicatorParameters.ApiClients),
        //    parametersManager));
        //FieldEditors.Add(new DictionaryFieldEditor<ApiClientCruder, ApiClientSettings>(
        //    nameof(ReplicatorParameters.ApiClients), logger, httpClientFactory, parametersManager));

        //FieldEditors.Add(new FileStoragesFieldEditor(logger, nameof(ReplicatorParameters.FileStorages),
        //    parametersManager));
        FieldEditors.Add(
            new DictionaryFieldEditor<FileStorageCruder, FileStorageData>(nameof(ReplicatorParameters.FileStorages),
                logger, parametersManager));

        //FieldEditors.Add(new ExcludeSetsFieldEditor(nameof(ReplicatorParameters.ExcludeSets), parametersManager));
        FieldEditors.Add(
            new DictionaryFieldEditor<ExcludeSetCruder, ExcludeSet>(nameof(ReplicatorParameters.ExcludeSets),
                parametersManager));

        //FieldEditors.Add(new ReplacePairsSetFieldEditor(nameof(ReplicatorParameters.ReplacePairsSets), parametersManager));
        FieldEditors.Add(new DictionaryFieldEditor<ReplacePairsSetCruder, ReplacePairsSet>(
            nameof(ReplicatorParameters.ReplacePairsSets), logger, httpClientFactory, parametersManager));

        //FieldEditors.Add(new SmartSchemasFieldEditor(nameof(ReplicatorParameters.SmartSchemas), parametersManager));
        FieldEditors.Add(
            new DictionaryFieldEditor<SmartSchemaCruder, SmartSchema>(nameof(ReplicatorParameters.SmartSchemas),
                parametersManager));

        //FieldEditors.Add(new ArchiversFieldEditor(nameof(ReplicatorParameters.Archivers), parametersManager));
        FieldEditors.Add(new DictionaryFieldEditor<ArchiverCruder, ArchiverData>(nameof(ReplicatorParameters.Archivers),
            parametersManager));
    }
}
