using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;
using static Average.Shared.SharedAPI;

namespace Average.Server.Models
{
    public class StorageContextItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Emoji { get; set; }
        public string EventName { get; set; }

        [JsonIgnore]
        public Action<StorageItemData, RaycastHit> Action { get; set; }

        public StorageContextItem() => Id = RandomString();
    }
}