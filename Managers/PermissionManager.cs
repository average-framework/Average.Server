using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class PermissionManager : IPermissionManager
    {
        private List<PermissionData> _permissions;

        public PermissionManager()
        {
            Task.Factory.StartNew(async () =>
            {
                _permissions = await Main.sql.GetAllAsync<PermissionData>("permissions");
                Log.Info("[Permission] loaded");
            });
            
            #region Event

            Main.eventHandlers["Permission.Set"] += new Action<int, string, int>(SetPermissionEvent);

            #endregion

            #region Rpc

            Main.rpc.Event("Permission.GetAll").On((message, callback) => callback(_permissions));

            #endregion
        }

        #region Event

        private void SetPermissionEvent(int target, string permissionName, int permissionLevel)
        {
            Main.players[target].TriggerEvent("Permission.Set", permissionName, permissionLevel);
            Log.Info($"Set permission: [{permissionName}, {permissionLevel}] to player: {Main.players[target].Identifiers["license"]}");
        }

        #endregion
    }
}
