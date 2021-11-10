using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Framework.Services;

namespace Average.Server.Framework.Handlers
{
    internal class RpcHandler : IHandler
    {
        private readonly RpcService _rpcService;

        public RpcHandler(RpcService rpcService)
        {
            _rpcService = rpcService;
        }

        [ServerEvent("rpc:send_response")]
        private void OnReceiveResponse(Client client, string @event, string response)
        {
            _rpcService.TriggerResponse(@event, response);
        }

        [ServerEvent("rpc:trigger_event")]
        private void OnTriggerEvent(Client client, string @event, string request)
        {
            _rpcService.OnInternalRequest(client, @event, request);
        }
    }
}
