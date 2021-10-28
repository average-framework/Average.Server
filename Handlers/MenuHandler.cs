using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Menu;
using Average.Server.Services;
using Average.Shared.Attributes;
using System.Collections.Generic;
using static Average.Server.Services.RpcService;

namespace Average.Server.Handlers
{
    internal class MenuHandler : IHandler
    {
        private readonly UIService _uiService;
        private readonly ClientService _clientService;
        private readonly MenuService _menuService;

        public MenuHandler(UIService uiService, ClientService clientService, MenuService menuService)
        {
            _uiService = uiService;
            _clientService = clientService;
            _menuService = menuService;
        }

        [UICallback("menu/on_click")]
        private void OnClick(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var id = (string)args["id"];
            var type = (string)args["type"];

            var currentMenu = _clientService.GetData<MenuContainer>(client, "Menu:CurrentMenu");

            switch (type)
            {
                case "primary":
                    var primaryItem = currentMenu.GetPrimaryItem(id);

                    switch (primaryItem)
                    {
                        case ButtonItem menuItem:
                            menuItem.OnClick?.Invoke(menuItem);
                            break;
                    }
                    break;
                case "secondary":
                    var secondaryItem = currentMenu.GetSecondaryItem(id);

                    switch (secondaryItem)
                    {
                        case BottomButton menuItem:
                            menuItem.OnClick?.Invoke(menuItem);
                            break;
                    }
                    break;
                    //case "info":
                    //    break;
            }
        }

        [UICallback("menu/keydown")]
        private void OnKeydown(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var key = int.Parse(args["key"].ToString());

            var histories = _clientService.GetData<List<MenuContainer>>(client, "Menu:Histories");
            var isOpen = _clientService.GetData<bool>(client, "Menu:IsOpen");
            var canCloseMenu = _clientService.GetData<bool>(client, "Menu:CanCloseMenu");

            if (isOpen && key == 27)
            {
                if (histories.Count > 0)
                {
                    var containerIndex = histories.Count - 1;
                    var parent = histories[containerIndex];

                    _menuService.Open(client, parent);

                    histories.RemoveAt(containerIndex);
                }
                else
                {
                    if (canCloseMenu)
                    {
                        _menuService.Close(client);
                        _uiService.Unfocus(client);
                    }
                }
            }
        }
    }
}
