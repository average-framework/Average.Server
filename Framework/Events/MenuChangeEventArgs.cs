using Average.Server.Framework.Model;
using Average.Server.Menu;
using System;

namespace Average.Server.Framework.Events
{
    internal class MenuChangeEventArgs : EventArgs
    {
        public Client Client { get; set; }
        public MenuContainer OldMenu { get; }
        public MenuContainer CurrentMenu { get; }

        public MenuChangeEventArgs(Client client, MenuContainer oldMenu, MenuContainer currentMenu)
        {
            Client = client;
            OldMenu = oldMenu;
            CurrentMenu = currentMenu;
        }
    }
}
