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

    public ParametersEditorListCliMenuCommandFactoryStrategy(
        ILogger<ParametersEditorListCliMenuCommandFactoryStrategy> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public string MenuCommandName => ReplicatorParametersEditor.MenuCommandName;

    public CliMenuCommand CreateMenuCommand(IParametersManager parametersManager)
    {
        var parameters = (ReplicatorParameters)parametersManager.Parameters;

        var replicatorParametersEditor =
            new ReplicatorParametersEditor(_logger, _httpClientFactory, parameters, parametersManager);
        return new ParametersEditorListCliMenuCommand(replicatorParametersEditor);
    }
}
