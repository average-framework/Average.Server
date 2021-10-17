using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;

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
        public StorageContextMenu ContextMenu { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, StorageItemData, StorageItemData> OnStacking { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, object> OnRenderStack { get; set; }

        [JsonIgnore]
        public Action OnInventoryLoading { get; set; }

        [JsonIgnore]
        public Action OnChestLoading { get; set; }

        public StorageItemInfo()
        {
            
        }

        [JsonConstructor]
        public StorageItemInfo(string name, string title, string description, string img, double weight)
        {
            Name = name;
            Title = title;
            Description = Description;
            Img = img;
            Weight = weight;
        }
    }
}