using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class DoorService : IService
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;
        private readonly List<DoorInfo> _infos = new();
        private readonly DoorCollection _doors = new();

        internal class DoorCollection : IEnumerable<DoorModel>
        {
            private readonly List<DoorModel> _doors = new();

            internal DoorModel GetDoor(Vector3 doorPosition) => _doors.Find(x =>
              Math.Round(x.Position.X) == Math.Round(doorPosition.X) &&
              Math.Round(x.Position.Y) == Math.Round(doorPosition.Y) &&
              Math.Round(x.Position.Z) == Math.Round(doorPosition.Z));

            public DoorModel this[int index] => _doors[index];

            public DoorCollection AddDoor(DoorModel door)
            {
                _doors.Add(door);
                return this;
            }

            internal DoorCollection RemoveDoor(DoorModel door)
            {
                _doors.Remove(door);
                return this;
            }

            public IEnumerator<DoorModel> GetEnumerator()
            {
                for(int i = 0; i < _doors.Count; i++)
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
            public bool IsLocked { get; set; }
            public Action<DoorModel> OpenAction { get; set; }
            public Action<DoorModel> NearAction { get; set; }

            public DoorModel(Vector3 position, bool isLocked, Action<DoorModel> nearAction, Action<DoorModel> openAction)
            {
                Position = position;
                IsLocked = isLocked;
                NearAction = nearAction;
                OpenAction = openAction;
            }
        }

        public DoorService(EventService eventService, ClientService clientService)
        {
            _eventService = eventService;
            _clientService = clientService;

            // Get list of doors
            _infos = Configuration.Parse<List<DoorInfo>>("utilities/doors.json");

            //Add(new DoorModel());
        }

        private Vector3 _position;

        [Thread]
        private async Task Update()
        {
            for (int i = 0; i < _clientService.ClientCount; i++)
            {
                var client = _clientService[i];

                if (client != null && client.Player != null && client.Player.Character != null)
                {
                    if(_position != client.Player.Character.Position)
                    {
                        _position = client.Player.Character.Position;

                        var door = _doors.ToList().Find(x => Vector3.Distance(client.Player.Character.Position, x.Position) < 5f);

                        if(door != null)
                        {
                            _eventService.EmitClient(client, "door:is_near", true);
                            Logger.Debug("Door is near !");
                        }

                        //for (int z = 0; z < _doors.Count; z++)
                        //{
                        //    var door = _doors[z];
                        //    var pedPos = client.Player.Character.Position;
                        //    var doorPos = door.Position;
                        //    var distance = Vector3.Distance(pedPos, doorPos);
                        //}

                        //_eventService.EmitClient(client, )
                        //Logger.Debug("Player position: " + client.Player.Character.Position);
                    }
                }
            }

            await BaseScript.Delay(1000);
        }

        internal DoorCollection Add(DoorModel door)
        {
            return _doors.AddDoor(door);
        }

        internal DoorCollection Remove(DoorModel door)
        {
            return _doors.RemoveDoor(door);
        }

        private DoorInfo GetDoorInfo(Vector3 position) => _infos.Find(x =>
                Math.Round(x.Position.X) == Math.Round(position.X) &&
                Math.Round(x.Position.Y) == Math.Round(position.Y) &&
                Math.Round(x.Position.Z) == Math.Round(position.Z));

        internal void OnSetDoorState(Vector3 doorPosition)
        {
            var door = _doors.GetDoor(doorPosition);

            if(door != null)
            {
                door.IsLocked = !door.IsLocked;
                _eventService.EmitClients("door:set_door_state", door.Position, door.IsLocked);
            }
            else
            {
                Logger.Debug($"Unable to find door at position: {door.Position}.");
            }
        }
    }
}
