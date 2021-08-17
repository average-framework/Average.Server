using Average.Server.Data;
using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Rpc;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class PermissionManager : IPermissionManager
    {
        Logger logger;
        PlayerList players;

        public List<PermissionData> Permissions { get; private set; }

        public PermissionManager(Logger logger, RpcRequest rpc, SQL sql, EventHandlerDictionary eventHandler, PlayerList players)
        {
            this.logger = logger;
            this.players = players;


            #region Events

            eventHandler["Permission.Set"] += new Action<int, string, int>(SetPermissionEvent);
            
            rpc.Event("Permission.GetAll").On((message, callback) => callback(Permissions));

            #endregion

            Task.Factory.StartNew(async () =>
            {
                Permissions = await sql.GetAllAsync<PermissionData>("permissions");
                logger.Info("[Permission] loaded");
            });
        }

        #region Event

        protected void SetPermissionEvent(int target, string permissionName, int permissionLevel)
        {
            players[target].TriggerEvent("Permission.Set", permissionName, permissionLevel);
            logger.Info($"Set permission: [{permissionName}, {permissionLevel}] to player: {players[target].Identifiers["license"]}");
        }

        #endregion
    }
}
