using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using CitizenFX.Core;

namespace Average.Server.Framework.Managers
{
    internal class PermissionManager : IService
    {
        public PermissionManager()
        {
            Logger.Write("PermissionManager", "Initialized successfully");
        }

        public void SetPermission(Player player, int permissionLevel)
        {
            player.TriggerEvent("permission:set_permission", permissionLevel);
            Logger.Info($"Set permission: [{permissionLevel}] to player: {player.License()}");
        }

        public bool HasPermission(int needPermissionLevel, int permissionLevel)
        {
            return permissionLevel >= needPermissionLevel;
        }
    }
}
