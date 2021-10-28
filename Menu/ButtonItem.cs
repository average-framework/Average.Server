using Newtonsoft.Json;
using System;
using static Average.Shared.SharedAPI;

namespace Average.Server.Menu
{
    internal class ButtonItem : IPrimaryMenuItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Visible { get; set; }

        [JsonIgnore]
        public MenuContainer Parent { get; set; }

        [JsonIgnore]
        public Action<ButtonItem> OnClick { get; }

        public ButtonItem(string text, Action<ButtonItem> onClick, bool visible = true)
        {
            Id = RandomString();
            Text = text;
            OnClick = onClick;
            Visible = visible;
        }

        public object OnRender() => new
        {
            type = GetType().Name,
            id = Id,
            text = Text,
            visible = Visible
        };
    }
}
