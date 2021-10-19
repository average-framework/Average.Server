using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Average.Server.Models
{
    public class StorageItemInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public double Weight { get; set; }
        public bool CanBeStacked { get; set; }
        public bool RemoveOnGive { get; set; } = true;
        public Dictionary<string, object> DefaultData { get; set; }
        public StorageContextMenu ContextMenu { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, StorageItemData, StorageItemData> OnStacking { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, object> OnRenderStacking { get; set; }

        [JsonIgnore]
        public Action OnInventoryLoading { get; set; }

        [JsonIgnore]
        public Action OnChestLoading { get; set; }
    }
}