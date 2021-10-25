using Average.Server.Framework.Model;
using Average.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using static Average.Shared.SharedAPI;

namespace Average.Server.Ray
{
    internal class RayItem
    {
        public string Id { get; } = RandomString();
        public string Text { get; }
        public string Emoji { get; }
        public bool IsVisible { get; set; }

        [JsonIgnore]
        public bool CloseMenuOnAction { get; }

        [JsonIgnore]
        public Func<Client, RaycastHit, Task<bool>> Condition { get; }

        [JsonIgnore]
        public Action<Client, RaycastHit> Action { get; }

        public RayItem(string text, string emoji, bool closeMenuOnAction, Action<Client, RaycastHit> action, Func<Client, RaycastHit, Task<bool>> condition)
        {
            Text = text;
            Emoji = emoji;
            CloseMenuOnAction = closeMenuOnAction;
            Action = action;
            Condition = condition;
        }
    }
}
