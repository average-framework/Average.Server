using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using CitizenFX.Core;

namespace Average.Server.Handlers
{
    internal class DoorHandler : IHandler
    {
        private readonly DoorService _doorService;

        public DoorHandler(DoorService doorService)
        {
            _doorService = doorService;
        }

        [ServerEvent("door:set_state")]
        private void SetDoorStateEvent(Client client, Vector3 position)
        {
            _doorService.OnSetDoorState(client, position);
        }
    }
}
