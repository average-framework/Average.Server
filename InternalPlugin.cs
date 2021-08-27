using System;
using Average.Server.Data;
using Average.Server.Managers;
using CitizenFX.Core;
using SDK.Server.Rpc;

namespace Average.Server
{
    public abstract class InternalPlugin
    {
        public string Name { get; }
        
        public PlayerList Players { get; set; }
        public RpcRequest Rpc { get; set; }
        public ThreadManager Thread { get; set; }
        public CharacterManager Character { get; set; }
        public CommandManager Command { get; set; }
        public EventManager Event { get; set; }
        public ExportManager Export { get; set; }
        public PermissionManager Permission { get; set; }
        public SaveManager Save { get; set; }
        public SyncManager Sync { get; set; }
        public UserManager User { get; set; }
        public RequestManager Request { get; set; }
        public RequestInternalManager RequestInternal { get; set; }
        public JobManager Job { get; set; }
        public DoorManager Door { get; set; }

        public InternalPlugin()
        {
            Name = GetType().Name;
        }
        
        public virtual void OnInitialized()
        {
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
