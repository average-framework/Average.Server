using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Commands
{
    internal class DebugCommand : ICommand
    {
        private readonly RpcService _rpcService;

        public DebugCommand(RpcService rpcService)
        {
            _rpcService = rpcService;
        }

        [ClientCommand("debug.gotow")]
        private async void OnGotow(Client client)
        {
            Logger.Debug("Gotow: " + client.Name);

            try
            {
                var coords = await _rpcService.NativeCall<Vector3>(client, 0x29B30D07C3F7873B);
                Logger.Debug("Gotow result: " + coords);
                _rpcService.NativeCall(client, 0x06843DA7060A026B, coords.X, coords.Y, coords.Z, true, true, true, false);
            }
            catch (Exception ex)
            {
                Logger.Error("Ex: " + ex.Message + ", " + ex.StackTrace);
            }
        }
    }
}
