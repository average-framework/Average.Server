using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.DataModels;
using System;

namespace Average.Server.Handlers
{
    internal class InventoryHandler : IHandler
    {
        private readonly InventoryService _inventoryService;
        private readonly RpcService _rpcService;
        private readonly UserService _userService;

        public InventoryHandler(InventoryService inventoryService, RpcService rpcService, UserService userService)
        {
            _inventoryService = inventoryService;
            _rpcService = rpcService;
            _userService = userService;

            _rpcService.OnRequest<string>("inventory:get", (client, cb, storageId) =>
            {
                var storageData = _inventoryService.Get(storageId);

                if(storageData == null)
                {
                    Logger.Error($"Unable to get inventory for client {client.Name}: {storageId}");
                    return;
                }

                cb(storageData);
            });
        }

        [ServerEvent("character:character_created")]
        private async void OnCharacterCreated(Client client, string characterId)
        {
            if (!await _inventoryService.Exists(characterId))
            {
                var storage = new StorageData
                {
                    StorageId = characterId,
                    MaxWeight = InventoryService.DefaultMaxInventoryWeight,
                    Type = StorageDataType.Player,
                    Items = new()
                };

                var userData = await _userService.Get(client);
                userData.SelectedCharacterId = characterId;

                _userService.Update(userData);
                _inventoryService.Create(storage);

                Logger.Write("Inventory", $"Inventory created for {client.License} with storage id: %{characterId}% of type %{storage.Type}%",
                    new Logger.TextColor(foreground: ConsoleColor.DarkYellow),
                    new Logger.TextColor(foreground: ConsoleColor.Cyan));
            }
        }

        [ServerEvent("inventory:update")]
        private async void OnUpdate(Client client, string storageDataJson)
        {
            var storageData = storageDataJson.Convert<StorageData>();
            Logger.Error("OnUpdate: " + client.Name + ", " + (storageData == null));
            var result = await _inventoryService.SaveInventory(storageData);

            if (!result)
            {
                Logger.Error($"Unable to save inventory for client {client.Name}: {storageData.StorageId}");
            }
        }
    }
}
