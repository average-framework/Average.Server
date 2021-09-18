﻿using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using CitizenFX.Core;

namespace Average.Server.Managers
{
    internal class PermissionService : IService
    {
        public PermissionService()
        {
            Logger.Write("PermissionManager", "Initialized successfully");
        }

        public void SetPermission(Player player, int permissionLevel)
        {
            player.TriggerEvent("permission:set_permission", permissionLevel);
            Logger.Info($"Set permission: [{permissionLevel}] to player: {player.License()}");
        }
    }
}
