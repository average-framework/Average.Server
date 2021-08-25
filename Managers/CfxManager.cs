using CitizenFX.Core;
using SDK.Server.Diagnostics;
using System;

namespace Average.Server.Managers
{
    internal class CfxManager
    {
        public CfxManager()
        {
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
        }

        #region Event

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            Log.Info($"{playerName} is connected to the server.");
             Main.eventManager.OnPlayerConnecting(player, setKickReason, deferrals);
        }

        private void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
             Main.eventManager.OnPlayerDisconnecting(player, reason);
        }

        private void OnResourceStop(string resource)
        {
             Main.eventManager.OnResourceStop(resource);
        }

        private void OnResourceStart(string resource)
        {
             Main.eventManager.OnResourceStart(resource);
        }

        private void OnResourceListRefresh()
        {
             Main.eventManager.OnResourceListRefresh();
        }

        private void OnResourceStarting(string resource)
        {
             Main.eventManager.OnResourceStarting(resource);
        }

        private void OnServerResourceStart(string resource)
        {
             Main.eventManager.OnServerResourceStart(resource);
        }

        private void OnServerResourceStop(string resource)
        {
             Main.eventManager.OnServerResourceStop(resource);
        }

        private void OnPlayerJoining(string source, string oldId)
        {
             Main.eventManager.OnPlayerJoining(source, oldId);
        }

        private void OnEntityCreated(int handle)
        {
             Main.eventManager.OnEntityCreated(handle);
        }

        private void OnEntityCreating(int handle)
        {
             Main.eventManager.OnEntityCreating(handle);
        }

        private void OnEntityRemoved(int handle)
        {
             Main.eventManager.OnEntityRemoved(handle);
        }

        private void OnPlayerEnteredScope(object data)
        {
             Main.eventManager.OnPlayerEnteredScope(data);
        }

        private void OnPlayerLeftScope(object data)
        {
             Main.eventManager.OnPlayerLeftScope(data);
        }

        #endregion
    }
}
