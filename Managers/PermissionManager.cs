using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Average.Server.Data;

namespace Average.Server.Managers
{
    public class PermissionManager : InternalPlugin, IPermissionManager
    {
        private List<PermissionData> _permissions;

        public override void OnInitialized()
        {
            Task.Factory.StartNew(async () =>
            {
                _permissions = await SQL.GetAllAsync<PermissionData>("permissions");
                Log.Info("[Permission] loaded");
            });
            
            #region Event

            Main.eventHandlers["Permission.Set"] += new Action<int, string, int>(SetPermissionEvent);

            #endregion

            #region Rpc

            Rpc.Event("Permission.GetAll").On((message, callback) => callback(_permissions));

            #endregion
        }

        #region Event

        private void SetPermissionEvent(int target, string permissionName, int permissionLevel)
        {
            Players[target].TriggerEvent("Permission.Set", permissionName, permissionLevel);
            Log.Info($"Set permission: [{permissionName}, {permissionLevel}] to player: {Players[target].Identifiers["license"]}");
        }

        #endregion
    }
}
