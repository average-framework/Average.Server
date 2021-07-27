using CitizenFX.Core;
using SDK.Server;
using SDK.Server.Diagnostics;
using System;

namespace Average.Internal
{
    internal class CfxManager
    {
        EventHandlerDictionary eventHandlers;
        Logger logger;
        Framework framework;

        public CfxManager(EventHandlerDictionary eventHandlers, Logger logger, Framework framework)
        {
            this.eventHandlers = eventHandlers;
            this.logger = logger;
            this.framework = framework;

            eventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            eventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDisconnecting);
        }

        #region Events

        protected async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            //deferrals.defer();
            logger.Info($"{playerName} is connected to the server.");
            framework.Event.OnPlayerConnecting(player, setKickReason, deferrals);
        }

        protected async void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            framework.Event.OnPlayerDisconnecting(player, reason);
        }

        #endregion
    }
}
