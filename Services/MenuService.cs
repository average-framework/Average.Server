using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using Average.Server.Menu;
using System.Collections.Generic;

namespace Average.Server.Services
{
    internal class MenuService : IService
    {
        private readonly EventManager _eventManager;
        private int _menuIndex;

        private readonly List<MenuContainer> _menus = new();

        public MenuService(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        internal void RegisterMenu(MenuContainer menu)
        {
            menu.Index = _menuIndex;
            _menus.Add(menu);
            _menuIndex++;
        }

        internal void UnregisterMenu(MenuContainer menu)
        {
            _menus.Remove(menu);
        }

        internal bool Exists(MenuContainer menu)
        {
            return _menus.Contains(menu);
        }

        internal void Open(Client client, MenuContainer menu)
        {
            _eventManager.EmitClient(client, "menu:open", menu.Index);
        }

        internal void Close(Client client)
        {
            _eventManager.EmitClient(client, "menu:close");
        }
    }
}
