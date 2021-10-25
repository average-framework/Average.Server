using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Attributes;
using DryIoc;
using System;
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

        private readonly Dictionary<string, List<Tuple<object, MethodInfo>>> _nuiEvents = new();
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public UIService(IContainer container, RpcService rpcService, EventService eventService)
        {
            _container = container;
            _rpcService = rpcService;
            _eventService = eventService;

            Logger.Write("UIService", "Initialized successfully");
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
            var events = new List<string>();
            
            foreach(var @event in _nuiEvents)
            {
                events.Add(@event.Key);
            }

            _eventService.EmitClient(client, "ui:register_nui_events", events);
        }

        private void RegisterInternalUICallbackEvent(UICallbackAttribute eventAttr, object classObj, MethodInfo method)
        {
            var redirectName = eventAttr.Name;

            if (!_nuiEvents.ContainsKey(redirectName))
            {
                _nuiEvents.Add(redirectName, new List<Tuple<object, MethodInfo>> { new Tuple<object, MethodInfo>(classObj, method) });
            }
            else
            {
                _nuiEvents[redirectName].Add(new Tuple<object, MethodInfo>(classObj, method));
            }

            _rpcService.OnRequest<Dictionary<string, object>>(redirectName, (client, cb, dict) =>
            {
                if (!_nuiEvents.ContainsKey(redirectName)) return;

                var events = _nuiEvents[redirectName];

                foreach(var @event in events)
                {
                    Logger.Debug("Receive ui callback from client: " + client.Name + ", method: " + (@event.Item2.DeclaringType.Name + ":" + @event.Item2.Name) + ", event name: " + redirectName + ", dict: " + dict.ToJson());

                    @event.Item2.Invoke(@event.Item1, new object[] { client, dict, new RpcCallback(args =>
                    {
                        cb(args.ToList());
                        return cb;
                    })});
                }
            });

            Logger.Write("UI", $"Registering [UICallback]: %{redirectName}% on method: {method.DeclaringType.Name + ":" + method.Name} ({classObj.GetType()}).", new Logger.TextColor(foreground: ConsoleColor.DarkYellow));
        }

        internal async void Emit(Client client, object message)
        {
            _eventService.EmitClient(client, "ui:emit", message.ToJson());
        }

        internal void SendMessage(Client client, string frameName, string requestType, object message = null)
        {
            _eventService.EmitClient(client, "ui:send_message", frameName, requestType, message.ToJson());
        }

        internal void SendGlobalMessage(string frameName, string requestType, object message = null)
        {
            _eventService.EmitClients("ui:send_message", frameName, requestType, message.ToJson());
        }

        internal void FocusFrame(Client client, string frameName)
        {
            _eventService.EmitClient(client, "ui:frame_focus", frameName);
        }

        internal void Focus(Client client, bool showCursor = true)
        {
            _eventService.EmitClient(client, "ui:focus", showCursor);
        }

        internal void Unfocus(Client client)
        {
            _eventService.EmitClient(client, "ui:unfocus");
        }

        internal void LoadFrame(Client client, string frameName)
        {
            _eventService.EmitClient(client, "ui:load_frame", frameName);
        }

        internal void DestroyFrame(Client client, string frameName)
        {
            _eventService.EmitClient(client, "ui:destroy_frame", frameName);
        }

        internal void Show(Client client, string frameName)
        {
            _eventService.EmitClient(client, "ui:show", frameName);
        }

        internal void Hide(Client client, string frameName)
        {
            _eventService.EmitClient(client, "ui:hide", frameName);
        }

        internal void FadeIn(Client client, string frameName, int fadeDuration = 100)
        {
            _eventService.EmitClient(client, "ui:fadein", frameName, fadeDuration);
        }

        internal void FadeOut(Client client, string frameName, int fadeDuration = 100)
        {
            _eventService.EmitClient(client, "ui:fadeout", frameName, fadeDuration);
        }

        internal void SetZIndex(Client client, string frameName, int zIndex)
        {
            _eventService.EmitClient(client, "ui:zindex", frameName, zIndex);
        }
    }
}
