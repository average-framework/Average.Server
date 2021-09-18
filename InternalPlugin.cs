//using Average.Server.Data;
//using Average.Server.Managers;
//using CitizenFX.Core;
//using SDK.Server;
//using SDK.Server.Rpc;
//using System;

//namespace Average.Server
//{
//    public abstract class InternalPlugin : IPlugin
//    {
//        public string Name { get; }

//        protected SQL Sql { get; private set; }
//        protected PlayerList Players { get; private set; }
//        protected RpcRequest Rpc { get; private set; }
//        protected ThreadManager Thread { get; private set; }
//        protected CommandManager Command { get; private set; }
//        protected EventManager Event { get; private set; }
//        protected ExportManager Export { get; private set; }
//        protected PermissionManager Permission { get; private set; }
//        protected SaveManager Save { get; private set; }
//        protected SyncManager Sync { get; private set; }
//        protected RequestManager Request { get; private set; }
//        protected RequestInternalManager RequestInternal { get; private set; }
//        protected JobManager Job { get; private set; }
//        protected DoorManager Door { get; private set; }
//        protected StorageManager Storage { get; private set; }
//        protected EnterpriseManager Enterprise { get; private set; }

//        public InternalPlugin()
//        {
//            Name = GetType().Name;
//        }

//        public void SetDependencies(SQL sql, PlayerList players, RpcRequest rpc, ThreadManager thread,
//            CommandManager command, EventManager @event, ExportManager export,
//            PermissionManager permission, SaveManager save, SyncManager sync, RequestManager request,
//            RequestInternalManager requestInternal, JobManager job, DoorManager door, StorageManager storage, EnterpriseManager enterprise)
//        {
//            Sql = sql;
//            Players = players;
//            Rpc = rpc;
//            Thread = thread;
//            Command = command;
//            Event = @event;
//            Export = export;
//            Permission = permission;
//            Save = save;
//            Sync = sync;
//            Request = request;
//            RequestInternal = requestInternal;
//            Job = job;
//            Door = door;
//            Storage = storage;
//            Enterprise = enterprise;
//        }

//        public virtual void OnInitialized()
//        {

//        }

//        public void Dispose()
//        {
//            GC.SuppressFinalize(this);
//        }
//    }
//}