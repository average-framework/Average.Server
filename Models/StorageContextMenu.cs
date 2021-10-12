using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Models
{
    public class StorageContextMenu
    {
        public StorageContextMenu(params StorageContextItem[] items) => Items = items.ToList();

        public List<StorageContextItem> Items { get; } = new List<StorageContextItem>();

        public void AddContext(StorageContextItem item) => Items.Add(item);
        public void RemoveContext(StorageContextItem item) => Items.Remove(item);
        public StorageContextItem? GetContext(string eventName) => Items.Find(x => x.EventName == eventName);
    }
}