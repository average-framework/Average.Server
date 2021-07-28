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

        #endregion
    }
}
