using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using static Average.Server.Framework.ServerAPI;

namespace Average.Server.Services
{
    internal class GameService : IService
    {
        private readonly RpcService _rpcService;

        public GameService(RpcService rpcService)
        {
            _rpcService = rpcService;
        }

        internal void Init(Client client)
        {
            _rpcService.NativeCall(client, 0x4B8F743A4A6D2FF8, true);
        }
    }
}
