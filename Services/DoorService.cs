using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace Average.Server.Services
{
    internal class DoorService : IService
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;
        private readonly RpcService _rpcService;

        private readonly List<DoorInfo> _infos = new();
        private readonly List<DoorModel> _doors = new();

        public class DoorModel
        {
            public Vector3 Position { get; set; }
            public float Range { get; set; }
            public bool IsLocked { get; set; }

            public DoorModel(Vector3 position, float range, bool isLocked)
            {
                Position = position;
                Range = range;
                IsLocked = isLocked;
            }
        }

        public DoorService(EventService eventService, ClientService clientService, RpcService rpcService)
        {
            _eventService = eventService;
            _clientService = clientService;
            _rpcService = rpcService;

            // Get list of doors
            _infos = Configuration.Parse<List<DoorInfo>>("utilities/doors.json");

            // Doors
            Add(new DoorModel(new Vector3(-276.02f, 802.59f, 118.41f), 2f, true));

            Logger.Write("DoorService", "Initialized successfully");
        }

        internal void OnClientInitialized(Client client)
        {
            _eventService.EmitClient(client, "door:init", _doors);
        }

        internal void Add(DoorModel door)
        {
            _doors.Add(door);
        }

        internal void Remove(DoorModel door)
        {
            _doors.Remove(door);
        }

        private DoorModel GetDoor(Vector3 position) => _doors.Find(x =>
               Math.Round(x.Position.X) == Math.Round(position.X) &&
               Math.Round(x.Position.Y) == Math.Round(position.Y) &&
               Math.Round(x.Position.Z) == Math.Round(position.Z));

        private DoorInfo GetDoorInfo(Vector3 position) => _infos.Find(x =>
                Math.Round(x.Position.X) == Math.Round(position.X) &&
                Math.Round(x.Position.Y) == Math.Round(position.Y) &&
                Math.Round(x.Position.Z) == Math.Round(position.Z));

        internal void OnSetDoorState(Client client, Vector3 doorPosition)
        {
            Logger.Error($"Client: {client.Name} try to open door: " + doorPosition);

            var door = GetDoor(doorPosition);

            if (door != null)
            {
                var info = GetDoorInfo(doorPosition);
                if (info == null) return;

                door.IsLocked = !door.IsLocked;
                _eventService.EmitClients("door:set_state", info.Hash, door.IsLocked ? 1 : 0);
            }
            else
            {
                Logger.Debug($"Unable to find door at position: {door.Position}.");
            }
        }
    }
}
