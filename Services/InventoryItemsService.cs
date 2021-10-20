using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Models;
using static Average.Server.Models.StorageItemInfo;

namespace Average.Server.Services
{
    internal class InventoryItemsService : IService
    {
        private readonly InventoryService _inventoryService;

        public InventoryItemsService(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;

            RegisterItems();
        }

        private void RegisterItems()
        {
            _inventoryService.RegisterItem(GetMoneyItemInfo());
            _inventoryService.RegisterItem(GetAppleItemInfo());
        }

        #region Registered Items

        private StorageItemInfo GetMoneyItemInfo() => new()
        {
            Name = "money",
            Text = "Argent",
            Img = "money_moneystack",
            Title = "Du fric",
            Description = "Un petit peu de flouz",
            Weight = 1.0,
            CanBeStacked = true,
            SplitValueType = typeof(decimal),
            DefaultData = new()
            {
                { "cash", 0m }
            },
            OnStacking = (lastItem, targetItem) =>
            {
                var cash = decimal.Parse(lastItem.Data["cash"].ToString()) + decimal.Parse(targetItem.Data["cash"].ToString());
                targetItem.Data["cash"] = cash;
                return targetItem;
            },
            OnRenderStacking = (item) =>
            {
                return "$" + item.Data["cash"];
            },
            OnSplit = (item, splitValue, splitType) =>
            {
                var cash = (decimal)item.Data["cash"];

                switch (splitType)
                {
                    case SplitType.BaseItem:
                        cash -= (decimal)splitValue;
                        item.Data["cash"] = cash;
                        break;
                    case SplitType.TargetItem:
                        item.Data["cash"] = splitValue;
                        break;
                }

                return item;
            },
            ContextMenu = GetMoneyContextMenu()
        };

        private StorageItemInfo GetAppleItemInfo() => new()
        {
            Name = "apple",
            Text = "Pomme",
            Img = "consumable_apple",
            Title = "Une grosse pomme",
            Description = "Une petite pomme",
            Weight = 1.0,
            CanBeStacked = true,
            SplitValueType = typeof(int),
            ContextMenu = GetAppleContextMenu()
        };

        #endregion

        #region Context Menu

        internal StorageContextMenu GetMoneyContextMenu() => new(new StorageContextItem
        {
            EventName = "drop",
            Emoji = "",
            Text = "Jeter",
            Action = (client, itemData, raycast) =>
            {
                Logger.Debug("item: " + itemData.Name + ", " + raycast.EntityHit);
            }
        },
        GetMoneySplitContextItem());

        internal StorageContextMenu GetAppleContextMenu() => new(new StorageContextItem
        {
            EventName = "drop",
            Emoji = "",
            Text = "Jeter",
            Action = (client, itemData, raycast) =>
            {
                Logger.Debug("item: " + itemData.Name + ", " + raycast.EntityHit);
            }
        },
        GetDefaultSplitContextItem());

        #endregion

        #region Context Items

        internal StorageContextItem GetMoneySplitContextItem() => new()
        {
            EventName = "split",
            Emoji = "✂️",
            Text = "Séparer",
            Action = (client, itemData, raycast) =>
            {
                Logger.Debug("Split decimal item: " + itemData.Name);

                var info = _inventoryService.GetItemInfo(itemData.Name);
                var minValue = 1m;
                var maxValue = decimal.Parse(itemData.Data["cash"].ToString());

                _inventoryService.ShowSplitMenu(client, info, itemData.SlotId, minValue, maxValue, minValue);
            }
        };

        internal StorageContextItem GetDefaultSplitContextItem() => new()
        {
            EventName = "split",
            Emoji = "✂️",
            Text = "Séparer",
            Action = (client, itemData, raycast) =>
            {
                Logger.Debug("Split int item: " + itemData.Name);

                var info = _inventoryService.GetItemInfo(itemData.Name);
                var minValue = 1;
                var maxValue = itemData.Count;

                _inventoryService.ShowSplitMenu(client, info, itemData.SlotId, minValue, maxValue, minValue);
            }
        };

        #endregion
    }
}
