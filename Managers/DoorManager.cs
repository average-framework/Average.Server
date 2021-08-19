using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Models;
using SDK.Server.Rpc;
using System;
using System.Collections.Generic;

namespace Average.Server.Managers
{
    public class DoorManager : IDoorManager
    {
        Logger logger;
        EventManager eventManager;

        List<Door> doors;

        public DoorManager(Logger logger, EventHandlerDictionary eventHandlers, EventManager eventManager, RpcRequest rpc)
        {
            this.logger = logger;
            this.eventManager = eventManager;

            doors = SDK.Server.Configuration.Parse<List<Door>>("configs/custom_doors.json");

            #region Event

            eventHandlers["Door.SetDoorState"] += new Action<Vector3>(SetDoorStateEvent);

            #endregion

            #region Rpc

            rpc.Event("Door.GetDoors").On((message, callback) => callback(doors.ToArray()));

            #endregion
        }

        #region Export

        public Door Exist(Vector3 position)
        {
            return doors.Find(x =>
                Math.Round(x.Position.X) == Math.Round(position.X) &&
                Math.Round(x.Position.Y) == Math.Round(position.Y) &&
                Math.Round(x.Position.Z) == Math.Round(position.Z));
        }

        public void SetDoorState(Door door)
        {
            door.IsLocked = !door.IsLocked;
            eventManager.EmitClients("Door.SetDoorState", door.Position, door.IsLocked);
        }

        #endregion

        #region Event

        protected void SetDoorStateEvent(Vector3 position)
        {
            var door = Exist(position);

            if (door == null)
            {
                logger.Debug("[Door] Any door exist at position: " + position);
                return;
            }
            else
            {
                SetDoorState(door);
            }
        }

        #endregion
    }
}
