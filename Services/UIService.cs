using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Attributes;
using DryIoc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Average.Server.Services.RpcService;

namespace Average.Server.Services
{
    internal class UIService : IService
    {
        private readonly IContainer _container;
        private readonly RpcService _rpcService;
        private readonly EventService _eventService;

        private readonly List<string> _nuiEvents = new();
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public UIService(IContainer container, RpcService rpcService, EventService eventService)
        {
            _container = container;
            _rpcService = rpcService;
            _eventService = eventService;
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            // Register client nui events
            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.Resolve(service);
                    var methods = service.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<UICallbackAttribute>();
                        if (attr == null) continue;

                        RegisterInternalUICallbackEvent(attr, _service, method);
                    }
                }
            }
        }

        internal void OnClientInitialized(Client client)
        {
            _eventService.EmitClient(client, "ui:register_nui_events", _nuiEvents);
        }

        private void RegisterInternalUICallbackEvent(UICallbackAttribute eventAttr, object classObj, MethodInfo method)
        {
            var redirectName = eventAttr.Name;

            _rpcService.OnRequest<Dictionary<string, object>>(redirectName, (client, cb, dict) =>
            {
                Logger.Debug("Receive ui callback from client: " + client.Name);

                foreach (var val in dict)
                {
                    Logger.Debug("Dict: " + val.Key + ", " + val.Value);
                }

                method.Invoke(classObj, new object[] { dict, new RpcCallback(args =>
                {
                    cb(args.ToList());
                    return cb;
                })});
            });

            if (!_nuiEvents.Contains(redirectName))
            {
                _nuiEvents.Add(redirectName);
            }

            Logger.Debug($"Registering [UICallback]: {redirectName} on method: {method.Name}.");
        }
    }
}
