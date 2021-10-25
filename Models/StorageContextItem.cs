using Average.Server.Framework.Model;
using Average.Shared.DataModels;
using Average.Shared.Models;
using Newtonsoft.Json;
using System;

namespace Average.Server.Models
{
    internal class StorageContextItem
    {
        public string Text { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public string EventName { get; set; }

        [JsonIgnore]
        public Action<Client, StorageData, StorageItemData, RaycastHit> Action { get; set; }
    }
}