using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Ray;
using Average.Server.Services;
using Average.Shared.Attributes;
using System.Collections.Generic;
using static Average.Server.Services.RpcService;

namespace Average.Server.Handlers
{
    internal class RayHandler : IHandler
    {
        private readonly RayService _rayService;
        private readonly UIService _uiService;
        private readonly ClientService _clientService;

        public RayHandler(RayService rayService, UIService uiService, ClientService clientService)
        {
            _rayService = rayService;
            _uiService = uiService;
            _clientService = clientService;

            Logger.Error("Ray Handler");
        }

        [UICallback("window_ready")]
        private void OnWindowReady(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            _rayService.OnClientWindowInitialized(client);
        }

        [UICallback("ray/on_click")]
        private void OnClick(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var id = (string)args["id"];

            _rayService.OnClick(client, id);
        }

        [UICallback("ray/keydown")]
        private void OnKeydown(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var key = int.Parse(args["key"].ToString());

            // Touche Echap
            if (key == 27)
            {
                var isOpen = _clientService.GetData<bool>(client, "Ray:IsOpen");

                if (isOpen)
                {
                    var histories = _clientService.GetData<List<RayGroup>>(client, "Ray:Histories");

                    if (histories.Count > 0)
                    {
                        var currentGroup = _clientService.GetData<RayGroup>(client, "Ray:CurrentGroup");
                        if (currentGroup == null) return;

                        var currentGroupIndex = histories.FindIndex(x => x.Name == currentGroup.Name);

                        if (currentGroupIndex > 0)
                        {
                            var parent = histories[currentGroupIndex - 1];

                            _rayService.Open(client, parent);
                            histories.RemoveAt(currentGroupIndex);
                        }
                        else
                        {
                            _rayService.OnPrevious(client);
                            _uiService.Unfocus(client);
                        }
                    }
                    else
                    {
                        Logger.Error("Keydown 9");
                        _rayService.OnPrevious(client);
                        _uiService.Unfocus(client);
                    }
                }
            }
        }
    }
}
