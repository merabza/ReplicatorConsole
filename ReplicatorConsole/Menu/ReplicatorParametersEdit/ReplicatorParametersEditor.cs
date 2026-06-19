using System.Net.Http;
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
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.ReplicatorParametersEdit;

public sealed class ReplicatorParametersEditor : ParametersEditor
{
    public ReplicatorParametersEditor(IApplication application, ILogger logger, IHttpClientFactory httpClientFactory,
        IParameters parameters, IParametersManager parametersManager) : base("Replicator Parameters Editor", parameters,
        parametersManager)
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

        FieldEditors.Add(new DictionaryFieldEditor<DatabaseServerConnectionCruder, DatabaseServerConnectionData>(
            nameof(ReplicatorParameters.DatabaseServerConnections),
            x => new DatabaseServerConnectionCruder(application, logger, httpClientFactory, parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<FileStorageCruder, FileStorageData>(
            nameof(ReplicatorParameters.FileStorages), x => new FileStorageCruder(logger, parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<ExcludeSetCruder, ExcludeSet>(
            nameof(ReplicatorParameters.ExcludeSets), x => new ExcludeSetCruder(parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<ReplacePairsSetCruder, ReplacePairsSet>(
            nameof(ReplicatorParameters.ReplacePairsSets), x => new ReplacePairsSetCruder(parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<SmartSchemaCruder, SmartSchema>(
            nameof(ReplicatorParameters.SmartSchemas), x => new SmartSchemaCruder(parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<ArchiverCruder, ArchiverData>(nameof(ReplicatorParameters.Archivers),
            x => new ArchiverCruder(parametersManager, x)));

        FieldEditors.Add(new DictionaryFieldEditor<RetryStrategyParametersCruder, RetryStrategyParameters>(
            nameof(ReplicatorParameters.RetryStrategyParameters),
            x => new RetryStrategyParametersCruder(parametersManager, x)));
    }
}
