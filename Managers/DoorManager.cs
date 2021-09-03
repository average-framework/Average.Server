using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SDK.Server;

namespace Average.Server.Managers
{
    public class DoorManager : InternalPlugin, IDoorManager
    {
        private List<Door> _doors;

        public override void OnInitialized()
        {
            _doors = SDK.Server.Configuration.Parse<List<Door>>("configs/custom_doors.json");

            #region Rpc

            Rpc.Event("Door.GetAll").On((message, callback) => callback(_doors));

            #endregion
        }

        public Door? Get(Vector3 position)
        {
            return _doors.Find(x =>
                Math.Round(x.Position.X) == Math.Round(position.X) &&
                Math.Round(x.Position.Y) == Math.Round(position.Y) &&
                Math.Round(x.Position.Z) == Math.Round(position.Z));
        }

        public void SetDoorState(Door door)
        {
            door.IsLocked = !door.IsLocked;
            Event.EmitClients("Door.SetDoorState", door.Position, door.IsLocked);
        }

        #region Event

        [ServerEvent("Door.SetDoorState")]
        private void SetDoorStateEvent(int player, Vector3 position)
        {
            var door = Get(position);

            if (door == null)
            {
                Log.Debug("[Door] Any door exist at position: " + position);
            }
            else
            {
                SetDoorState(door);
            }
        }

        #endregion
    }
}
