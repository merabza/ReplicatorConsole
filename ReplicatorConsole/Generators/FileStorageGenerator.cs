using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.Generators;

public sealed class FileStorageGenerator
{
    private readonly IParametersManager _parametersManager;

    public FileStorageGenerator(IParametersManager parametersManager)
    {
        _parametersManager = parametersManager;
    }

    public void GenerateForLocalPath(string fileStorageName, string fileStoragePath)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        var fileStorage = new FileStorageData { FileStoragePath = fileStoragePath };

        parameters.FileStorages.TryAdd(fileStorageName, fileStorage);
    }
}
