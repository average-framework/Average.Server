using Average.Shared.Rpc;
using CitizenFX.Core;

namespace Average.Server.Framework.Rpc
{
    public class RpcTrigger : IRpcTrigger
    {
        private readonly PlayerList _players;

        public RpcTrigger(PlayerList players)
        {
            _players = players;
        }

        public void Trigger(RpcMessage message)
        {
            if (message.Target != -1)
            {
                _players[message.Target].TriggerEvent(message.Event, message.ToJson());
            }
            else
            {
                BaseScript.TriggerClientEvent(message.Event, message.ToJson());
            }
        }
    }
}
