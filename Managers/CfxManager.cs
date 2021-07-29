using CitizenFX.Core;
using SDK.Server.Diagnostics;
using System;

namespace Average.Managers
{
    internal class CfxManager
    {
        EventHandlerDictionary eventHandlers;
        Logger logger;
        EventManager eventManager;

        public CfxManager(EventHandlerDictionary eventHandlers, Logger logger, EventManager eventManager)
        {
            this.eventHandlers = eventHandlers;
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
            eventHandlers["playerEnteredScope"] += new Action<object, string>(OnPlayerEnteredScope);
        }

        #region Events

        protected async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            //deferrals.defer();
            logger.Info($"{playerName} is connected to the server.");
            eventManager.OnPlayerConnecting(player, setKickReason, deferrals);
        }

        protected async void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            eventManager.OnPlayerDisconnecting(player, reason);
        }

        protected async void OnResourceStop(string resource)
        {
            eventManager.OnResourceStop(resource);
        }

        protected async void OnResourceStart(string resource)
        {
            eventManager.OnResourceStart(resource);
        }

        protected async void OnResourceListRefresh()
        {
            eventManager.OnResourceListRefresh();
        }

        protected async void OnResourceStarting(string resource)
        {
            eventManager.OnResourceStarting(resource);
        }

        protected async void OnServerResourceStart(string resource)
        {
            eventManager.OnServerResourceStart(resource);
        }

        protected async void OnServerResourceStop(string resource)
        {
            eventManager.OnServerResourceStop(resource);
        }

        protected async void OnPlayerJoining(string source, string oldId)
        {
            eventManager.OnPlayerJoining(source, oldId);
        }

        protected async void OnEntityCreated(int handle)
        {
            eventManager.OnEntityCreated(handle);
        }

        protected async void OnEntityCreating(int handle)
        {
            eventManager.OnEntityCreating(handle);
        }

        protected async void OnEntityRemoved(int handle)
        {
            eventManager.OnEntityRemoved(handle);
        }

        protected async void OnPlayerEnteredScope(object data, string player)
        {
            eventManager.OnPlayerEnteredScope(data, player);
        }

        #endregion
    }
}
