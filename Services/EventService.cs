using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Rpc;
using CitizenFX.Core;
using DryIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Services
{
    internal class EventService : IService
    {
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;

        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly Dictionary<string, List<Delegate>> _events = new Dictionary<string, List<Delegate>>();

        public EventService(IContainer container, EventHandlerDictionary eventHandlers)
        {
            _container = container;
            _eventHandlers = eventHandlers;

            #region Events

            // Internal events
            _eventHandlers["__cfx_internal:httpResponse"] += new Action<int, int, string, dynamic>(OnHttpResponse);
            _eventHandlers["client-event:triggered"] += new Action<Player, string, List<object>>(OnTriggerEvent);

            // Custom events
            _eventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            _eventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDisconnecting);
            _eventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
            _eventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            _eventHandlers["onResourceListRefresh"] += new Action(OnResourceListRefresh);
            _eventHandlers["onResourceStarting"] += new Action<string>(OnResourceStarting);
            _eventHandlers["onServerResourceStart"] += new Action<string>(OnServerResourceStart);
            _eventHandlers["onServerResourceStop"] += new Action<string>(OnServerResourceStop);
            _eventHandlers["playerJoining"] += new Action<string, string>(OnPlayerJoining);
            _eventHandlers["entityCreated"] += new Action<int>(OnEntityCreated);
            _eventHandlers["entityCreating"] += new Action<int>(OnEntityCreating);
            _eventHandlers["entityRemoved"] += new Action<int>(OnEntityRemoved);
            _eventHandlers["playerEnteredScope"] += new Action<object>(OnPlayerEnteredScope);
            _eventHandlers["playerLeftScope"] += new Action<object>(OnPlayerLeftScope);

            #endregion

            Logger.Write("EventService", "Initialized successfully");
        }

        public event EventHandler<PlayerConnectingEventArgs> PlayerConnecting;
        public event EventHandler<PlayerDisconnectingEventArgs> PlayerDisconnecting;
        public event EventHandler<ResourceStopEventArgs> ResourceStop;
        public event EventHandler<ResourceStartEventArgs> ResourceStart;
        public event EventHandler<EventArgs> ResourceListRefresh;
        public event EventHandler<ResourceStartingEventArgs> ResourceStarting;
        public event EventHandler<ServerResourceStartEventArgs> ServerResourceStart;
        public event EventHandler<ServerResourceStopEventArgs> ServerResourceStop;
        public event EventHandler<PlayerJoiningEventArgs> PlayerJoining;
        public event EventHandler<EntityCreatedEventArgs> EntityCreated;
        public event EventHandler<EntityCreatingEventArgs> EntityCreating;
        public event EventHandler<EntityRemovedEventArgs> EntityRemoved;
        public event EventHandler<PlayerEnteredScopeEventArgs> PlayerEnteredScope;
        public event EventHandler<PlayerLeftScopeEventArgs> PlayerLeftScope;
        public event EventHandler<HttpResponseEventArgs> HttpResponse;

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            // Register server events
            foreach (var type in types)
            {
                if (_container.IsRegistered(type))
                {
                    // Continue if the service have the same type of this class
                    if (type == GetType()) continue;

                    // Get service instance
                    var service = _container.GetService(type);
                    var methods = type.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ServerEventAttribute>();
                        if (attr == null) continue;

                        RegisterInternalEvent(attr, service, method);
                    }
                }
            }
        }

        public void Emit(string eventName, params object[] args)
        {
            if (_events.ContainsKey(eventName))
            {
                _events[eventName].ForEach(x => x.DynamicInvoke(args));
            }
        }

        public void EmitClient(Client client, string eventName, params object[] args)
        {
            //Logger.Debug("trigger server event on client: " + client.Name + ", " + eventName + ", " + args.Count());
            client.Player.TriggerEvent("server-event:triggered", eventName, args);
        }

        public void EmitClients(string eventName, params object[] args)
        {
            //Logger.Debug("trigger server event on clients: " + eventName + ", " + args.Count());

            var clientService = _container.Resolve<ClientService>();

            for (int i = 0; i < clientService.Clients.Count; i++)
            {
                clientService.Clients[i].Player.TriggerEvent("server-event:triggered", eventName, args);
            }
        }

        public void RegisterEvent(string eventName, Delegate action)
        {
            if (!_events.ContainsKey(eventName))
            {
                _events.Add(eventName, new List<Delegate> { action });
            }
            else
            {
                _events[eventName].Add(action);
            }

            Logger.Write("Thread", $"Registering [ServerEvent]: %{eventName}% on method: {action.Method.Name}.", new Logger.TextColor(foreground: ConsoleColor.DarkYellow));
        }

        public void UnregisterEvent(string eventName)
        {
            if (_events.ContainsKey(eventName))
            {
                _events.Remove(eventName);

                Logger.Debug($"Removing [ServerEvent]: {eventName}");
            }
            else
            {
                Logger.Debug($"Unable to remove [ServerEvent]: {eventName}");
            }
        }

        internal void RegisterInternalEvent(ServerEventAttribute eventAttr, object classObj, MethodInfo method)
        {
            RegisterEvent(eventAttr.Event, Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method));
        }

        private void OnTriggerEvent([FromSource] Player player, string eventName, List<object> args)
        {
            Logger.Debug("Receive client event from client: " + player.Name + ", " + eventName + ", " + string.Join(", ", args));

            var client = _container.Resolve<ClientService>().Get(player);
            var newArgs = new List<object>();

            if (client == null)
            {
                // Create temporary client
                newArgs.Add(new Client(player));
            }
            else
            {
                newArgs.Add(client);
            }

            args.ForEach(x => newArgs.Add(x));
            Emit(eventName, newArgs.ToArray());
        }

        #region Internal Server Events

        internal void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic kick, dynamic deferrals)
        {
            PlayerConnecting?.Invoke(this, new PlayerConnectingEventArgs(player, kick, deferrals));
            Emit(ServerEvent.PlayerConnecting, new PlayerConnectingEventArgs(player, kick, deferrals));
        }

        internal void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            var p = player;

            PlayerDisconnecting?.Invoke(this, new PlayerDisconnectingEventArgs(p, reason));
            Emit(ServerEvent.PlayerDisconnecting, new PlayerDisconnectingEventArgs(p, reason));
        }

        internal void OnResourceStop(string resource)
        {
            ResourceStop?.Invoke(this, new ResourceStopEventArgs(resource));
            Emit(ServerEvent.ResourceStop, new ResourceStopEventArgs(resource));
        }

        internal void OnResourceStart(string resource)
        {
            ResourceStart?.Invoke(this, new ResourceStartEventArgs(resource));
            Emit(ServerEvent.ResourceStart, new ResourceStartEventArgs(resource));
        }

        internal void OnResourceListRefresh()
        {
            ResourceListRefresh?.Invoke(this, new EventArgs());
            Emit(ServerEvent.ResourceListRefresh);
        }

        internal void OnResourceStarting(string resource)
        {
            ResourceStarting?.Invoke(this, new ResourceStartingEventArgs(resource));
            Emit(ServerEvent.ResourceStarting, new ResourceStartingEventArgs(resource));
        }

        internal void OnServerResourceStart(string resource)
        {
            ServerResourceStart?.Invoke(this, new ServerResourceStartEventArgs(resource));
            Emit(ServerEvent.ServerResourceStart, new ServerResourceStartEventArgs(resource));
        }

        internal void OnServerResourceStop(string resource)
        {
            ServerResourceStop?.Invoke(this, new ServerResourceStopEventArgs(resource));
            Emit(ServerEvent.ServerResourceStop, new ServerResourceStopEventArgs(resource));
        }

        internal void OnPlayerJoining(string source, string oldId)
        {
            PlayerJoining?.Invoke(this, new PlayerJoiningEventArgs(source, oldId));
            Emit(ServerEvent.PlayerJoining, new PlayerJoiningEventArgs(source, oldId));
        }

        internal void OnEntityCreated(int handle)
        {
            EntityCreated?.Invoke(this, new EntityCreatedEventArgs(handle));
            Emit(ServerEvent.EntityCreated, new EntityCreatedEventArgs(handle));
        }

        internal void OnEntityCreating(int handle)
        {
            EntityCreating?.Invoke(this, new EntityCreatingEventArgs(handle));
            Emit(ServerEvent.EntityCreating, new EntityCreatingEventArgs(handle));
        }

        internal void OnEntityRemoved(int handle)
        {
            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(handle));
            Emit(ServerEvent.EntityRemoved, new EntityRemovedEventArgs(handle));
        }

        internal void OnPlayerEnteredScope(object data)
        {
            PlayerEnteredScope?.Invoke(this, new PlayerEnteredScopeEventArgs(data));
            Emit(ServerEvent.PlayerEnteredScope, new PlayerEnteredScopeEventArgs(data));
        }

        internal void OnPlayerLeftScope(object data)
        {
            PlayerLeftScope?.Invoke(this, new PlayerLeftScopeEventArgs(data));
            Emit(ServerEvent.PlayerLeftScope, new PlayerLeftScopeEventArgs(data));
        }

        internal void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            HttpResponse?.Invoke(this, new HttpResponseEventArgs(token, status, text, header));
            Emit("HttpResponse", new HttpResponseEventArgs(token, status, text, header));
        }

        #endregion
    }
}
