using Average.Server.Data;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Rpc;
using SDK.Shared.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class PermissionManager : IPermissionManager
    {
        public List<PermissionData> Permissions { get; private set; }

        public PermissionManager(Logger logger, RpcRequest rpc, SQL sql)
        {
            #region Events

            rpc.Event("Permission.GetAll").On((message, callback) =>
            {
                callback(Permissions);
            });

            #endregion

            Task.Factory.StartNew(async () =>
            {
                Permissions = await sql.GetAllAsync<PermissionData>("permissions");
                logger.Info("[Permission] loaded");
            });
        }
    }
}
