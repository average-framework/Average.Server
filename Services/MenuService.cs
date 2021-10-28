using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Menu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class MenuService : IService
    {
        private readonly UIService _uiService;
        private readonly ClientService _clientService;

        public MenuService(UIService uiService, ClientService clientService)
        {
            _uiService = uiService;
            _clientService = clientService;

            //var topContainer = new TopContainer();
            //var bottomContainer = new BottomContainer();
            //var menuInfoContainer = new StatsMenuInfo();

            //topContainer.AddItem(new ButtonItem("Enculer de merde", (btn) => 
            //{
            //    Logger.Error("Je mange des moules tout les matins !");
            //}));

            //bottomContainer.AddItem(new BottomButton("Je suis en bas", (btn) =>
            //{
            //    Logger.Error("Je ne mange plus :(");
            //}));

            //menuInfoContainer.AddItem(new StatsItem("Banane", StatsBarType.Five, 0, 100, 50));

            //var testMenu = new MenuContainer(topContainer, bottomContainer, menuInfoContainer);
            //testMenu.BannerTitle = "Debug Menu";
        }

        public event EventHandler<MenuChangeEventArgs> MenuChanged;
        public event EventHandler<MenuCloseEventArgs> MenuClosed;

        private void OnMenuChanged(Client client, MenuContainer oldMenu, MenuContainer currentMenu)
        {
            MenuChanged?.Invoke(this, new MenuChangeEventArgs(client, oldMenu, currentMenu));
        }

        private void OnMenuClosed(Client client, MenuContainer currentMenu)
        {
            MenuClosed?.Invoke(this, new MenuCloseEventArgs(client, currentMenu));
        }

        private void OnRender(Client client, MenuContainer menuContainer) => _uiService.SendMessage(client, "menu", "render", new
        {
            topContainer = menuContainer.TopContainer.OnRender(),
            bottomContainer = menuContainer.BottomContainer.OnRender(),
            middleContainer = menuContainer.MiddleContainer.OnRender()
        });

        internal void Open(Client client, MenuContainer menu)
        {
            _clientService.OnShareData(client, "Menu:IsOpen", true);

            var oldMenu = _clientService.GetData<MenuContainer>(client, "Menu:OldMenu");
            var currentMenu = _clientService.GetData<MenuContainer>(client, "Menu:CurrentMenu");

            if (oldMenu != currentMenu)
            {
                _clientService.OnShareData(client, "Menu:OldMenu", currentMenu);
            }

            _clientService.OnShareData(client, "Menu:CurrentMenu", menu);

            OnRender(client, currentMenu);

            _uiService.SendMessage(client, "menu", "open", new
            {
                id = currentMenu.Id,
                bannerTitle = currentMenu.BannerTitle
            });

            OnMenuChanged(client, oldMenu, currentMenu);
        }

        internal void Close(Client client)
        {
            var isOpen = _clientService.GetData<bool>(client, "Menu:IsOpen");

            if (isOpen)
            {
                var currentMenu = _clientService.GetData<MenuContainer>(client, "Menu:CurrentMenu");

                _clientService.OnShareData(client, "Menu:IsOpen", false);
                ClearHistory(client);
                OnMenuClosed(client, currentMenu);

                _uiService.SendMessage(client, "menu", "close");
            }
        }

        internal void ClearHistory(Client client)
        {
            var histories = _clientService.GetData<List<MenuContainer>>(client, "Menu:Histories");
            histories.Clear();
        }

        internal void AddHistory(Client client, MenuContainer menuContainer)
        {
            var histories = _clientService.GetData<List<MenuContainer>>(client, "Menu:Histories");

            if (!histories.Exists(x => x.Id == menuContainer.Id))
            {
                histories.Add(menuContainer);
            }
        }

        internal void RemoveHistory(Client client, MenuContainer menuContainer)
        {
            var histories = _clientService.GetData<List<MenuContainer>>(client, "Menu:Histories");

            if (histories.Exists(x => x.Id == menuContainer.Id))
            {
                histories.Remove(menuContainer);
            }
        }
    }
}