using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Menu;
using Average.Server.Services;

namespace Average.Server.Framework.Commands
{
    internal class MenuCommand : ICommand
    {
        private readonly MenuService _menuService;

        private readonly TopContainer topContainer = new();
        private readonly BottomContainer bottomContainer = new();
        private readonly StatsMenuInfo menuInfoContainer = new();
        private readonly MenuContainer testMenu;

        public MenuCommand(MenuService menuService)
        {
            _menuService = menuService;

            testMenu = new MenuContainer(topContainer, bottomContainer, menuInfoContainer);
            testMenu.BannerTitle = "Debug Menu";

            topContainer.AddItem(new ButtonItem("Enculer de merde", (btn) =>
            {
                Logger.Error("Je mange des moules tout les matins !");
            }));

            bottomContainer.AddItem(new BottomButton("Je suis en bas", (btn) =>
            {
                Logger.Error("Je ne mange plus :(");
            }));

            menuInfoContainer.AddItem(new StatsItem("Banane", StatsBarType.Five, 0, 100, 50));
        }

        [ClientCommand("menu.open")]
        private void OpenMenuCommand(Client client)
        {
            Logger.Error($"Client: {client.Name} try to open menu: {testMenu.BannerTitle} -> {testMenu.Id}");
            _menuService.Open(client, testMenu);
        }
    }
}
