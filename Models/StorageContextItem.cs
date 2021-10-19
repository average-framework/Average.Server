using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;

namespace Average.Server.Models
{
    public class StorageContextItem
    {
        public string Text { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public string EventName { get; set; }

        [JsonIgnore]
        public Action<StorageItemData, RaycastHit> Action { get; set; }
    }
}