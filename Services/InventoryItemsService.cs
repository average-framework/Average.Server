using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Models;

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
        }

        #region Registered Items

        private StorageItemInfo GetMoneyItemInfo() => new()
        {
            Name = "money",
            Img = "money_moneystack",
            Title = "Du fric",
            Description = "Un petit peu de flouz",
            Weight = 1.0,
            CanBeStacked = true,
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
                return item.Data["cash"];
            },
            ContextMenu = GetMoneyContextMenu()
        };

        #endregion

        #region Context Menu

        internal StorageContextMenu GetMoneyContextMenu() => new(new StorageContextItem
        {
            EventName = "drop",
            Emoji = "",
            Text = "Jeter",
            Action = (itemData, raycast) =>
            {
                Logger.Debug("item: " + itemData.Name + ", " + raycast.EntityHit);
                //_inventoryService.Remove
            }
        }, GetSplitContextItem());

        #endregion

        #region Context Items

        internal StorageContextItem GetSplitContextItem() => new()
        {
            EventName = "split",
            Emoji = "✂️",
            Text = "Séparer",
            Action = (itemData, raycast) =>
            {
                Logger.Debug("Split item: " + itemData.Name);
            }
        };

        #endregion
    }
}
