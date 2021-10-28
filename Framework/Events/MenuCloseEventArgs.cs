using Average.Server.Framework.Model;
using Average.Server.Menu;
using System;

namespace Average.Server.Framework.Events
{
    internal class MenuCloseEventArgs : EventArgs
    {
        public Client Client { get; set; }
        public MenuContainer CurrentMenu { get; }

        public MenuCloseEventArgs(Client client, MenuContainer currentMenu)
        {
            Client = client;
            CurrentMenu = currentMenu;
        }
    }
}
