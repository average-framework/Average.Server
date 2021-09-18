using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Extensions;
using SDK.Server.Interfaces;

namespace Average.Server.Managers
{
    internal class PermissionService : IPermissionManager, IService
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
