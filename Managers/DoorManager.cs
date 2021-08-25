using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Models;
using System;
using System.Collections.Generic;

namespace Average.Server.Managers
{
    public class DoorManager : IDoorManager
    {
        private readonly List<Door> _doors;

        public DoorManager()
        {
            _doors = SDK.Server.Configuration.Parse<List<Door>>("configs/custom_doors.json");

            #region Event

            Main.eventHandlers["Door.SetDoorState"] += new Action<Vector3>(SetDoorStateEvent);

            #endregion

            #region Rpc

            Main.rpc.Event("Door.GetDoors").On((message, callback) => callback(_doors.ToArray()));

            #endregion
        }

        #region Export

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
            Main.eventManager.EmitClients("Door.SetDoorState", door.Position, door.IsLocked);
        }

        #endregion

        #region Event

        private void SetDoorStateEvent(Vector3 position)
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
