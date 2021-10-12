using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;

namespace Average.Server.Models
{
    public class StorageItemInfo
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public string Img { get; private set; }
        public double Weight { get; private set; }
        public bool Stackable { get; private set; }
        public StorageContextMenu ContextMenu { get; set; }

        [JsonIgnore]
        public Action<StorageItemData> OnUpdateRender { get; set; }

        public StorageItemInfo()
        {

        }

        [JsonConstructor]
        public StorageItemInfo(string name, string text, string img, double weight, bool stackable)
        {
            Name = name;
            Text = text;
            Img = img;
            Weight = weight;
            Stackable = stackable;
        }
    }
}