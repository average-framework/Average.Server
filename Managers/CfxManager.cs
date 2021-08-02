using CitizenFX.Core;
using SDK.Server.Diagnostics;
using System;

namespace Average.Managers
{
    internal class CfxManager
    {
        Logger logger;
        EventManager eventManager;

        public CfxManager(EventHandlerDictionary eventHandlers, Logger logger, EventManager eventManager)
        {
            this.logger = logger;
            this.eventManager = eventManager;

            eventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            eventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDisconnecting);
            eventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
            eventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            eventHandlers["onResourceListRefresh"] += new Action(OnResourceListRefresh);
            eventHandlers["onResourceStarting"] += new Action<string>(OnResourceStarting);
            eventHandlers["onServerResourceStart"] += new Action<string>(OnServerResourceStart);
            eventHandlers["onServerResourceStop"] += new Action<string>(OnServerResourceStop);
            eventHandlers["playerJoining"] += new Action<string, string>(OnPlayerJoining);
            eventHandlers["entityCreated"] += new Action<int>(OnEntityCreated);
            eventHandlers["entityCreating"] += new Action<int>(OnEntityCreating);
            eventHandlers["entityRemoved"] += new Action<int>(OnEntityRemoved);
            eventHandlers["playerEnteredScope"] += new Action<object>(OnPlayerEnteredScope);
            eventHandlers["playerLeftScope"] += new Action<object>(OnPlayerLeftScope);
        }

        #region Events

        protected void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            logger.Info($"{playerName} is connected to the server.");
            eventManager.OnPlayerConnecting(player, setKickReason, deferrals);
        }

        protected void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            eventManager.OnPlayerDisconnecting(player, reason);
        }

        protected void OnResourceStop(string resource)
        {
            eventManager.OnResourceStop(resource);
        }

        protected void OnResourceStart(string resource)
        {
            eventManager.OnResourceStart(resource);
        }

        protected void OnResourceListRefresh()
        {
            eventManager.OnResourceListRefresh();
        }

        protected void OnResourceStarting(string resource)
        {
            eventManager.OnResourceStarting(resource);
        }

        protected void OnServerResourceStart(string resource)
        {
            eventManager.OnServerResourceStart(resource);
        }

        protected void OnServerResourceStop(string resource)
        {
            eventManager.OnServerResourceStop(resource);
        }

        protected void OnPlayerJoining(string source, string oldId)
        {
            eventManager.OnPlayerJoining(source, oldId);
        }

        protected void OnEntityCreated(int handle)
        {
            eventManager.OnEntityCreated(handle);
        }

        protected void OnEntityCreating(int handle)
        {
            eventManager.OnEntityCreating(handle);
        }

        protected void OnEntityRemoved(int handle)
        {
            eventManager.OnEntityRemoved(handle);
        }

        protected void OnPlayerEnteredScope(object data)
        {
            eventManager.OnPlayerEnteredScope(data);
        }

        protected void OnPlayerLeftScope(object data)
        {
            eventManager.OnPlayerLeftScope(data);
        }

        #endregion
    }
}
