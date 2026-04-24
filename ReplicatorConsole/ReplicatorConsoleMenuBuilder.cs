using AppCliTools.CliMenu;
using AppCliTools.CliTools.Services.MenuBuilder;
using ReplicatorConsole.Menu;

namespace ReplicatorConsole;

public sealed class ReplicatorConsoleMenuBuilder : IMenuBuilder
{
    private readonly IServiceProvider _serviceProvider;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ReplicatorConsoleMenuBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CliMenuSet BuildMainMenu()
    {
        //მთავარი მენიუს ჩატვირთვა
        return CliMenuSetFactory.CreateMenuSet("Main Menu", MenuData.MainMenuCommandFactoryStrategyNames,
            _serviceProvider, true);
    }
}
