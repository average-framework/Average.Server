using Average.Server.Framework.Interfaces;
using Average.Server.Models;

namespace Average.Server.Framework.Services
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
            }
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
            SplitValueType = typeof(int)
        };

        #endregion
    }
}
