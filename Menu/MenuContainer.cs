using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Average.Shared.SharedAPI;

namespace Average.Server.Menu
{
    interface IPrimaryMenuItem
    {
        string Id { get; set; }
        bool Visible { get; set; }
        object OnRender();
    }

    interface ISecondaryMenuItem
    {
        string Id { get; set; }
        bool Visible { get; set; }
        object OnRender();
    }

    internal class BottomButton : ISecondaryMenuItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Visible { get; set; } = true;

        [JsonIgnore]
        public MenuContainer Parent { get; set; }

        [JsonIgnore]
        public Action<BottomButton> OnClick { get; }

        public BottomButton(string text, Action<BottomButton> onClick, bool visible = true)
        {
            Id = RandomString();
            Text = text;
            OnClick = onClick;
            Visible = visible;
        }

        public object OnRender() => new
        {
            type = GetType().Name,
            id = Id,
            text = Text,
            visible = Visible
        };
    }

    internal class TopContainer
    {
        private readonly List<IPrimaryMenuItem> _items = new();

        public bool ItemExists(IPrimaryMenuItem menuItem)
        {
            return _items.Exists(x => x.Id == menuItem.Id);
        }

        public void AddItem(IPrimaryMenuItem menuItem)
        {
            if (!ItemExists(menuItem))
            {
                _items.Add(menuItem);
            }
        }

        public void RemoveItem(IPrimaryMenuItem menuItem)
        {
            if (ItemExists(menuItem))
            {
                _items.Remove(menuItem);
            }
        }

        public IEnumerable<IPrimaryMenuItem> GetItems()
        {
            return _items;
        }

        public T GetItem<T>(string id)
        {
            return (T)_items.Find(x => x.Id == id);
        }

        public object OnRender()
        {
            var items = new List<object>();

            foreach (var item in _items)
            {
                switch (item)
                {
                    case ButtonItem menuItem:
                        items.Add(menuItem.OnRender());
                        break;
                }
            }

            return items;
        }
    }

    internal class BottomContainer
    {
        private readonly List<ISecondaryMenuItem> _items = new();

        public bool ItemExists(ISecondaryMenuItem menuItem)
        {
            return _items.Exists(x => x.Id == menuItem.Id);
        }

        public void AddItem(ISecondaryMenuItem menuItem)
        {
            if (!ItemExists(menuItem))
            {
                _items.Add(menuItem);
            }
        }

        public void RemoveItem(ISecondaryMenuItem menuItem)
        {
            if (ItemExists(menuItem))
            {
                _items.Remove(menuItem);
            }
        }

        public IEnumerable<ISecondaryMenuItem> GetItems()
        {
            return _items;
        }

        public T GetItem<T>(string id)
        {
            return (T)_items.Find(x => x.Id == id);
        }

        public object OnRender()
        {
            var items = new List<object>();

            foreach (var item in _items)
            {
                switch (item)
                {
                    case BottomButton menuItem:
                        items.Add(menuItem.OnRender());
                        break;
                }
            }

            return items;
        }
    }

    interface IMenuInfo
    {
        string Id { get; set; }
        object OnRender();
    }

    internal class StoreMenuInfo : IMenuInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public decimal SellablePrice { get; set; }

        public StoreMenuInfo()
        {

        }

        public object OnRender() => new
        {
            type = GetType().Name,
            id = Id,
            title = Title,
            description = Description,
            weight = Weight,
            sellablePrice = SellablePrice,
        };
    }

    internal enum StatsBarType
    {
        Four, Five
    }

    internal class StatsItem
    {
        public string Id { get; set; } = RandomString();
        public string Label { get; set; }
        public StatsBarType StatsType { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }

        public StatsItem(string label, StatsBarType statsBarType, int min, int max, int value)
        {
            Id = RandomString();
            Label = label;
            StatsType = statsBarType;
            Min = min;
            Max = max;
            Value = value;
        }

        public object OnRender() => new
        {
            type = GetType().Name,
            id = Id,
            label = Label,
            statsType = StatsType,
            min = Min,
            max = Max,
            value = Value
        };
    }

    internal class StatsMenuInfo : IMenuInfo
    {
        public string Id { get; set; } = RandomString();

        private readonly List<StatsItem> _items = new();

        public bool ItemExists(StatsItem statsItem)
        {
            return _items.Exists(x => x.Id == statsItem.Id);
        }

        public void AddItem(StatsItem statsItem)
        {
            if (!ItemExists(statsItem))
            {
                _items.Add(statsItem);
            }
        }

        public void RemoveItem(StatsItem statsItem)
        {
            if (ItemExists(statsItem))
            {
                _items.Remove(statsItem);
            }
        }

        public IEnumerable<StatsItem> GetItems()
        {
            return _items;
        }

        public object OnRender()
        {
            var items = new List<object>();

            foreach (var item in _items)
            {
                items.Add(item.OnRender());
            }

            return items;
        }
    }

    internal class MenuContainer
    {
        public TopContainer TopContainer { get; private set; }
        public BottomContainer BottomContainer { get; private set; }
        public IMenuInfo MiddleContainer { get; private set; }

        public string Id { get; set; } = RandomString();
        public string BannerTitle { get; set; }

        public MenuContainer(TopContainer topContainer, BottomContainer bottomContainer, IMenuInfo menuInfo)
        {
            TopContainer = topContainer;
            BottomContainer = bottomContainer;
            MiddleContainer = menuInfo;
        }

        internal IPrimaryMenuItem GetPrimaryItem(string id)
        {
            return TopContainer.GetItem<IPrimaryMenuItem>(id);
        }

        internal ISecondaryMenuItem GetSecondaryItem(string id)
        {
            return BottomContainer.GetItem<ISecondaryMenuItem>(id);
        }
    }
}
