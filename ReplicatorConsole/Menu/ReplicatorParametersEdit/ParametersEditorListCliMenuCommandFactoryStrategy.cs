using System.Net.Http;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.Menu.ReplicatorParametersEdit;

public class ParametersEditorListCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ParametersEditorListCliMenuCommandFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;

    public ParametersEditorListCliMenuCommandFactoryStrategy(
        ILogger<ParametersEditorListCliMenuCommandFactoryStrategy> logger, IHttpClientFactory httpClientFactory, IParametersManager parametersManager)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        var replicatorParametersEditor =
            new ReplicatorParametersEditor(_logger, _httpClientFactory, parameters, _parametersManager);
        return new ParametersEditorListCliMenuCommand(replicatorParametersEditor);
    }
}
