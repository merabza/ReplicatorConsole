using System;
using ParametersManagement.LibParameters;

namespace ReplicatorConsole.Counters;

public sealed class SmartSchemaNameCounter : SCounter
{
    private readonly string? _databaseFileStorageName;
    private readonly string? _uploadFileStorageName;

    public SmartSchemaNameCounter(IParametersManager parametersManager, string? databaseFileStorageName,
        string? uploadFileStorageName) : base(parametersManager)
    {
        _databaseFileStorageName = databaseFileStorageName;
        _uploadFileStorageName = uploadFileStorageName;
    }

    public string Count(ESmartSchemaCase smartSchemaCase, string mainSmartSchemaName, string reduceSmartSchemaName)
    {
        if (mainSmartSchemaName == reduceSmartSchemaName)
        {
            return mainSmartSchemaName;
        }

        return smartSchemaCase switch
        {
            ESmartSchemaCase.DatabaseServerSide => _databaseFileStorageName is not null &&
                                                   _uploadFileStorageName is not null &&
                                                   (!IsFileStorageLocal(_databaseFileStorageName) ||
                                                    !IsFileStorageLocal(_uploadFileStorageName))
                ? reduceSmartSchemaName
                : mainSmartSchemaName,
            ESmartSchemaCase.Local => _uploadFileStorageName is not null && !IsFileStorageLocal(_uploadFileStorageName)
                ? reduceSmartSchemaName
                : mainSmartSchemaName,
            ESmartSchemaCase.UploadServerSide => mainSmartSchemaName,
            _ => throw new ArgumentOutOfRangeException(nameof(smartSchemaCase), smartSchemaCase, null)
        };
    }
}
