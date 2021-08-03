using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class PermissionManager : IPermissionManager
    {
        public List<PermissionData> Permissions { get; private set; }

        public PermissionManager(Framework framework)
        {
            Task.Factory.StartNew(async () =>
            {
                await framework.IsReadyAsync();

                Permissions = await framework.Sql.GetAllAsync<PermissionData>("permissions");
                framework.Logger.Info("[Permission] loaded");

                #region Events

                framework.Rpc.Event("Permission.GetAll").On((message, callback) =>
                {
                    callback(Permissions);
                });

                #endregion
            });
        }
    }
}
