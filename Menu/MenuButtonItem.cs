using Average.Server.Framework.Model;
using Average.Server.Interfaces;
using Average.Server.Services;
using System;
using static Average.Server.Framework.ServerAPI;

namespace Average.Server.Menu
{
    internal class MenuButtonItem : IMenuItem
    {
        public string Name { get; }
        public string Text { get; set; }
        public bool Visible { get; set; }
        public MenuContainer Parent { get; set; }
        public Action<MenuButtonItem> Action { get; }

        public MenuButtonItem(string text, Action<MenuButtonItem> action, bool visible = true)
        {
            Name = RandomString();
            Text = text;
            Action = action;
            Visible = visible;
        }

        public void OnRender(Client client, UIService uiService) => uiService.SendNui(client, "menu", "render_item", new
        {
            type = "button",
            name = Name,
            text = Text,
            visible = Visible
        });
    }
}
