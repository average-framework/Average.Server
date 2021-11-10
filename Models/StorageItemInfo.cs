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
        public bool IsSellable { get; set; }
        public Dictionary<string, object> DefaultData { get; set; } = new();

        [JsonIgnore]
        public Type SplitValueType { get; set; } = typeof(int);
    }
}