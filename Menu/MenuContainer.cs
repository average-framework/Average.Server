using Average.Server.Interfaces;
using System.Collections.Generic;
using static Average.Shared.SharedAPI;

namespace Average.Server.Menu
{
    internal class MenuContainer
    {
        private readonly List<IMenuItem> _items = new();

        public string Name { get; }
        public string Title { get; }
        public string Description { get; }
        public int Index { get; set; }

        public MenuContainer(string title, string description)
        {
            Name = RandomString();
            Title = title;
            Description = description;
        }

        public bool ItemExists(IMenuItem menuItem) => _items.Contains(menuItem);

        public void AddItem(IMenuItem menuItem)
        {
            if (!ItemExists(menuItem))
            {
                menuItem.Parent = this;
                _items.Add(menuItem);
            }
        }

        public void RemoveItem(IMenuItem menuItem)
        {
            if (ItemExists(menuItem)) _items.Remove(menuItem);
        }

        public IMenuItem GetItem(string name) => _items.Find(x => x.Name == name);
    }
}
