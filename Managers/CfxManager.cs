using CitizenFX.Core;
using System;

namespace Average.Server.Managers
{
    public class CfxManager : InternalPlugin
    {
        public override void OnInitialized()
        {
            #region Event

            Main.eventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            Main.eventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDisconnecting);
            Main.eventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
            Main.eventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            Main.eventHandlers["onResourceListRefresh"] += new Action(OnResourceListRefresh);
            Main.eventHandlers["onResourceStarting"] += new Action<string>(OnResourceStarting);
            Main.eventHandlers["onServerResourceStart"] += new Action<string>(OnServerResourceStart);
            Main.eventHandlers["onServerResourceStop"] += new Action<string>(OnServerResourceStop);
            Main.eventHandlers["playerJoining"] += new Action<string, string>(OnPlayerJoining);
            Main.eventHandlers["entityCreated"] += new Action<int>(OnEntityCreated);
            Main.eventHandlers["entityCreating"] += new Action<int>(OnEntityCreating);
            Main.eventHandlers["entityRemoved"] += new Action<int>(OnEntityRemoved);
            Main.eventHandlers["playerEnteredScope"] += new Action<object>(OnPlayerEnteredScope);
            Main.eventHandlers["playerLeftScope"] += new Action<object>(OnPlayerLeftScope);

            #endregion
        }

        #region Event

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            Event.OnPlayerConnecting(player, setKickReason, deferrals);
        }

        private void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            Event.OnPlayerDisconnecting(player, reason);
        }

        private void OnResourceStop(string resource)
        {
            Event.OnResourceStop(resource);
        }

        private void OnResourceStart(string resource)
        {
            Event.OnResourceStart(resource);
        }

        private void OnResourceListRefresh()
        {
            Event.OnResourceListRefresh();
        }

        private void OnResourceStarting(string resource)
        {
            Event.OnResourceStarting(resource);
        }

        private void OnServerResourceStart(string resource)
        {
            Event.OnServerResourceStart(resource);
        }

        private void OnServerResourceStop(string resource)
        {
            Event.OnServerResourceStop(resource);
        }

        private void OnPlayerJoining(string source, string oldId)
        {
            Event.OnPlayerJoining(source, oldId);
        }

        private void OnEntityCreated(int handle)
        {
            Event.OnEntityCreated(handle);
        }

        private void OnEntityCreating(int handle)
        {
            Event.OnEntityCreating(handle);
        }

        private void OnEntityRemoved(int handle)
        {
            Event.OnEntityRemoved(handle);
        }

        private void OnPlayerEnteredScope(object data)
        {
            Event.OnPlayerEnteredScope(data);
        }

        private void OnPlayerLeftScope(object data)
        {
            Event.OnPlayerLeftScope(data);
        }

        #endregion
    }
}