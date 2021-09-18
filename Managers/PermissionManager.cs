using CitizenFX.Core;
using DryIoc;
using SDK.Server.Diagnostics;
using SDK.Server.Extensions;
using SDK.Server.Interfaces;
using System;

namespace Average.Server.Managers
{
    public class PermissionManager : IPermissionManager
    {
        public PermissionManager()
        {
            Logger.Write("PermissionManager", "Initialized successfully");
        }

        #region Event

        public void SetPermission(Player player, int permissionLevel)
        {
            player.TriggerEvent("permission:set_permission", permissionLevel);
            Logger.Info($"Set permission: [{permissionLevel}] to player: {player.License()}");
        }

        #endregion
    }
}
