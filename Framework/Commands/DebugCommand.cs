using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using CitizenFX.Core;

namespace Average.Server.Framework.Commands
{
    internal class DebugCommand : ICommand
    {
        private readonly RpcService _rpcService;
        private readonly CharacterService _characterService;

        public DebugCommand(RpcService rpcService, CharacterService characterService)
        {
            _rpcService = rpcService;
            _characterService = characterService;
        }

        [ClientCommand("debug.gotow")]
        private async void OnGotow(Client client)
        {
            var coords = await _rpcService.NativeCall<Vector3>(client, 0x29B30D07C3F7873B);
            _characterService.OnTeleport(client, coords);
        }
    }
}
