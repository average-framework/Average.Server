using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Framework.Services;
using CitizenFX.Core;
using System;
using static CitizenFX.Core.Native.API;

namespace Average.Server.Scripts
{
    internal class AIZombieScript : IScript
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;

        public AIZombieScript(EventService eventService, ClientService clientService)
        {
            _eventService = eventService;
            _clientService = clientService;
        }

        internal void SyncEntityBetweenPlayers(Client client, int netId, bool isRusher)
        {
            for (int i = 0; i < _clientService.Clients.Count; i++)
            {
                var target = _clientService.Clients[i];

                //Logger.Debug("Apply from: " + client.Name + " to: " + target.Name + ", netId: " + netId + ", other: " + client.Player.Handle + ", " + target.Player.Handle);

                if (client.Player.Handle == target.Player.Handle) continue;

                var entity = NetworkGetEntityFromNetworkId(netId);

                var targetCoords = target.Player.Character.Position;
                var entityCoords = GetEntityCoords(entity);

                var targetDistance = Vector3.Distance(targetCoords, entityCoords);

                // Synchronise l'entité vers la cible uniquement si la distance entre l'entité et la cible est inférieur à 150feet
                // pour éviter que des évennements de synchronisation soit appeler pour rien
                if (targetDistance <= 150f)
                {
                    Logger.Debug("Apply for target: " + target.Name + ", " + targetDistance);
                    _eventService.EmitClient(target, "apply", netId, isRusher);
                }
                else
                {
                    Logger.Error("Ignore apply for target: " + target.Name);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
