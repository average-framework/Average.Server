using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Enums;
using CitizenFX.Core;
using SDK.Server.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Average.Server.Services.InputService;

namespace Average.Server.Services
{
    internal class DoorService : IService
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;
        private readonly InputService _inputService;
        private readonly RpcService _rpcService;

        private readonly List<DoorInfo> _infos = new();
        private readonly DoorList _doors = new();

        internal class DoorList : IEnumerable<DoorModel>
        {
            private readonly List<DoorModel> _doors = new();

            internal DoorModel GetDoor(Vector3 doorPosition) => _doors.Find(x =>
            Math.Round(x.Position.X) == Math.Round(doorPosition.X) &&
            Math.Round(x.Position.Y) == Math.Round(doorPosition.Y) &&
            Math.Round(x.Position.Z) == Math.Round(doorPosition.Z));

            public DoorModel this[int index] => _doors[index];

            public DoorList AddDoor(DoorModel door)
            {
                _doors.Add(door);
                return this;
            }

            internal DoorList RemoveDoor(DoorModel door)
            {
                _doors.Remove(door);
                return this;
            }

            public IEnumerator<DoorModel> GetEnumerator()
            {
                for (int i = 0; i < _doors.Count; i++)
                {
                    yield return _doors[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class DoorModel
        {
            public Vector3 Position { get; set; }
            public float Range { get; set; }
            public bool IsLocked { get; set; }
            public Action<Client, DoorModel> OpenAction { get; set; }
            public Action<Client, DoorModel, bool> NearAction { get; set; }

            public DoorModel(Vector3 position, float range, bool isLocked, Action<Client, DoorModel, bool> nearAction, Action<Client, DoorModel> openAction)
            {
                Position = position;
                Range = range;
                IsLocked = isLocked;
                NearAction = nearAction;
                OpenAction = openAction;
            }
        }

        public DoorService(EventService eventService, ClientService clientService, InputService inputService, RpcService rpcService)
        {
            _eventService = eventService;
            _clientService = clientService;
            _inputService = inputService;
            _rpcService = rpcService;

            // Get list of doors
            _infos = Configuration.Parse<List<DoorInfo>>("utilities/doors.json");

            // Doors
            Add(new DoorModel(new Vector3(-276.02f, 802.59f, 118.41f), 2f, true, nearAction: (client, door, isNear) =>
            {
                Logger.Debug($"Player {client.Name} is near of door ?? " + isNear);
            },
            openAction: (client, door) =>
            {
                Logger.Debug($"Player {client.Name} open the door at position: " + door.Position);
            }));

            // Inputs
            _inputService.RegisterKey(new Input((Control)0x8CC9CD42,
            condition: (client) =>
            {
                var exists = _doors.ToList().Exists(x => Vector3.Distance(client.Player.Character.Position, x.Position) < x.Range);
                return exists;
            },
            onStateChanged: (client, state) =>
            {
                Logger.Debug($"Client {client.Name} can {(state ? "open" : "not open")}/close the door");
            },
            onKeyReleased: (client) =>
            {
                var door = _doors.ToList().Find(x => Vector3.Distance(client.Player.Character.Position, x.Position) < x.Range);
                OnSetDoorState(client, ref door);
                Logger.Debug($"Client {client.Name} can open/close door: " + (door != null));
            }));

            Logger.Write("DoorService", "Initialized successfully");
        }

        internal DoorList Add(DoorModel door)
        {
            return _doors.AddDoor(door);
        }

        internal DoorList Remove(DoorModel door)
        {
            return _doors.RemoveDoor(door);
        }

        private DoorInfo GetDoorInfo(Vector3 position) => _infos.Find(x =>
                Math.Round(x.Position.X) == Math.Round(position.X) &&
                Math.Round(x.Position.Y) == Math.Round(position.Y) &&
                Math.Round(x.Position.Z) == Math.Round(position.Z));

        internal void OnSetDoorState(Client client, ref DoorModel door)
        {
            if (door == null) return;

            var info = GetDoorInfo(door.Position);
            door.IsLocked = !door.IsLocked;

            _rpcService.NativeCall(client, 0xD99229FE93B46286, info.Hash, 1, 0, 0, 0, 0, 0);
            _rpcService.NativeCall(client, 0x6BAB9442830C7F53, (dynamic)info.Hash, door.IsLocked ? 1 : 0);
        }

        internal void OnSetDoorState(Vector3 doorPosition)
        {
            var door = _doors.GetDoor(doorPosition);

            if (door != null)
            {
                door.IsLocked = !door.IsLocked;
                _eventService.EmitClients("door:set_state", door.Position, door.IsLocked);
            }
            else
            {
                Logger.Debug($"Unable to find door at position: {door.Position}.");
            }
        }
    }
}
