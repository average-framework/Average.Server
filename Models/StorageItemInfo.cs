using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Average.Server.Models
{
    internal class StorageItemInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public double Weight { get; set; }
        public bool CanBeStacked { get; set; }
        public bool RemoveOnGive { get; set; } = true;
        public Type SplitValueType { get; set; } = typeof(int);
        public Dictionary<string, object> DefaultData { get; set; }

        [JsonIgnore]
        public StorageContextMenu ContextMenu { get; set; }

        [JsonIgnore]
        public Action<StorageItemData, StorageItemData> OnStacking { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, object> OnRenderStacking { get; set; }

        [JsonIgnore]
        public Action<StorageItemData, StorageItemData> OnStackCombine { get; set; }

        internal enum SplitType
        {
            BaseItem, TargetItem
        }

        [JsonIgnore]
        public Action<StorageItemData, object, SplitType> OnSplit { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, bool> SplitCondition { get; set; }

        [JsonIgnore]
        public Func<StorageItemData, object> OnRenderSplit { get; set; }

        [JsonIgnore]
        public Action OnInventoryLoading { get; set; }

        [JsonIgnore]
        public Action OnChestLoading { get; set; }
    }
}