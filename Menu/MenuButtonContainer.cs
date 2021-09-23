using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using Average.Server.Interfaces;
using Average.Server.Services;
using System;
using static Average.Shared.SharedAPI;

namespace Average.Server.Menu
{
    internal class MenuButtonContainer : IMenuItem
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Visible { get; set; }
        public MenuContainer Parent { get; set; }
        public MenuContainer Target { get; }
        public Action<MenuButtonContainer> Action { get; }

        public MenuButtonContainer(string text, MenuContainer target, Action<MenuButtonContainer> action, bool visible = true)
        {
            Name = RandomString();
            Text = text;
            Target = target;
            Action = action;
            Visible = visible;
        }

        public void OnRender(Client client, UIService uiService) => uiService.SendNui(client, "menu", "render_item", new
        {
            type = "button_container",
            name = Name,
            text = Text,
            visible = Visible
        });
    }
}
