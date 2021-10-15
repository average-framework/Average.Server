using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;

namespace Average.Server.Services
{
    internal class GameService : IService
    {
        private readonly RpcService _rpcService;

        public GameService(RpcService rpcService)
        {
            _rpcService = rpcService;

            Logger.Write("GameService", "Initialized successfully");
        }

        internal void Init(Client client)
        {
            _rpcService.NativeCall(client, 0x4B8F743A4A6D2FF8, true);
        }
    }
}
