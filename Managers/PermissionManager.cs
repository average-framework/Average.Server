//using SDK.Server.Diagnostics;
//using SDK.Server.Interfaces;
//using System;

//namespace Average.Server.Managers
//{
//    public class PermissionManager : InternalPlugin, IPermissionManager
//    {
//        public override void OnInitialized()
//        {
//            #region Event

//            Main.eventHandlers["Permission.Set"] += new Action<int, int>(SetPermissionEvent);

//            #endregion
//        }

//        #region Event

//        private void SetPermissionEvent(int target, int permissionLevel)
//        {
//            Players[target].TriggerEvent("Permission.Set", permissionLevel);
//            Log.Info($"Set permission: [{permissionLevel}] to player: {Players[target].Identifiers["license"]}");
//        }

//        #endregion
//    }
//}
