using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Model;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static Average.Server.Framework.ServerAPI;
using static CitizenFX.Core.Native.API;

namespace Average.Server.Services
{
    internal class AIZombieScript
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;

        public AIZombieScript(EventService eventService, ClientService clientService)
        {
            _eventService = eventService;
            _clientService = clientService;

            _eventService.EntityCreated += _eventService_EntityCreated;
            _eventService.EntityRemoved += _eventService_EntityRemoved;
        }

        private void _eventService_EntityRemoved(object sender, Framework.Events.EntityRemovedEventArgs e)
        {
            //Logger.Debug("Delete entity: " + e.Handle);
        }

        private void _eventService_EntityCreated(object sender, Framework.Events.EntityCreatedEventArgs e)
        {
            //Logger.Debug("Spawn entity: " + e.Handle);
        }

        [ServerEvent("apply")]
        private void Apply(Client client, int netId, bool isRusher)
        {
            for(int i = 0; i < _clientService.Clients.Count; i++)
            {
                var target = _clientService.Clients[i];

                //Logger.Error("Apply from: " + client.Name + " to: " + target.Name + ", netId: " + netId + ", other: " + client.Player.Handle + ", " + target.Player.Handle);
                
                if(client.Player.Handle == target.Player.Handle) continue;

                var entity = NetworkGetEntityFromNetworkId(netId);

                var clientCoords = client.Player.Character.Position;
                var targetCoords = target.Player.Character.Position;
                var entityCoords = GetEntityCoords(entity);

                var clientDistance = Vector3.Distance(clientCoords, entityCoords);
                var targetDistance = Vector3.Distance(targetCoords, entityCoords);

                //if(distance <= 150)
                //{

                //}

                // Synchronise l'entité vers la cible uniquement si la distance entre l'entité et la cible est inférieur à 150feet
                // pour éviter que des évennements de synchronisation soit appeler pour rien
                if(targetDistance <= 150f)
                {
                    Logger.Debug("Apply for target: " + target.Name + ", " + clientDistance + ", " + targetDistance);
                    _eventService.EmitClient(target, "apply", netId, isRusher);
                }
                else
                {
                    Logger.Error("Ignore apply for target: " + target.Name);
                }
            }
        }
    }
}
