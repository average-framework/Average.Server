using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Attributes;
using DryIoc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        internal void FocusFrame(Client client, string frame) => Emit(client, new
        {
            eventName = "ui:frame_focus",
            frame
        });

        internal void Focus(Client client, bool showCursor = true)
        {
            _eventService.EmitClient(client, "ui:focus", false, showCursor);
        }

        internal void Unfocus(Client client)
        {
            _eventService.EmitClient(client, "ui:unfocus", false, false);
        }
        internal async void Emit(Client client, object message)
        {
            _eventService.EmitClient(client, "ui:emit", message.ToJson());
        }

        internal void SendMessage(Client client, string frame, string requestType, object message = null)
        {
            _eventService.EmitClient(client, frame, requestType, message.ToJson());
        }

        internal void LoadFrame(Client client, string frame) => Emit(client, new
        {
            eventName = "ui:load_frame",
            frame
        });

        internal void DestroyFrame(Client client, string frame) => Emit(client, new
        {
            eventName = "ui:destroy_frame",
            frame
        });

        internal void Show(Client client, string frame) => Emit(client, new
        {
            eventName = "ui:show",
            frame
        });

        internal void Hide(Client client, string frame) => Emit(client, new
        {
            eventName = "ui:hide",
            frame
        });

        internal void FadeIn(Client client, string frame, int fadeDuration = 100) => Emit(client, new
        {
            eventName = "ui:fadein",
            frame,
            fade = fadeDuration
        });

        internal void FadeOut(Client client, string frame, int fadeDuration = 100) => Emit(client, new
        {
            eventName = "ui:fadeout",
            frame,
            fade = fadeDuration
        });

        internal void SetZIndex(Client client, string frame, int zIndex) => Emit(client, new
        {
            eventName = "ui:zindex",
            frame,
            zIndex
        });
    }
}
