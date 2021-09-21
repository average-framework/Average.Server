using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Attributes;
using CitizenFX.Core;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Framework.Managers
{
    internal class EventManager
    {
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly Dictionary<string, List<Delegate>> _serverEvents = new Dictionary<string, List<Delegate>>();
        private readonly Dictionary<string, List<Delegate>> _clientEvents = new Dictionary<string, List<Delegate>>();

        public EventManager(IContainer container, EventHandlerDictionary eventHandlers)
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

            Logger.Write("ServerEventManager", "Initialized successfully");
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
            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.GetService(service);
                    var methods = service.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ServerEventAttribute>();
                        if (attr == null) continue;

                        RegisterInternalServerEvent(attr, _service, method);
                    }
                }
            }

            // Register client events
            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.GetService(service);
                    var methods = service.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ClientEventAttribute>();
                        if (attr == null) continue;

                        RegisterInternalClientEvent(attr, _service, method);
                    }
                }
            }
        }

        public void EmitLocalServer(string eventName, params object[] args)
        {
            if (_serverEvents.ContainsKey(eventName))
            {
                _serverEvents[eventName].ForEach(x => x.DynamicInvoke(args));
            }
        }

        internal void EmitLocalClient(string eventName, params object[] args)
        {
            if (_clientEvents.ContainsKey(eventName))
            {
                _clientEvents[eventName].ForEach(x => x.DynamicInvoke(args));
            }
        }

        public void EmitClient(Client client, string eventName, params object[] args)
        {
            client.Player.TriggerEvent("server-event:triggered", eventName, args);
        }

        public void EmitClients(string eventName, params object[] args)
        {
            BaseScript.TriggerClientEvent(eventName, args);
        }

        public void RegisterServerEvent(string eventName, Delegate action)
        {
            if (!_serverEvents.ContainsKey(eventName))
            {
                _serverEvents.Add(eventName, new List<Delegate> { action });
            }
            else
            {
                _serverEvents[eventName].Add(action);
            }

            Logger.Debug($"Registering [ServerEvent]: {eventName} on method: {action.Method.Name}.");
        }

        public void RegisterClientEvent(string eventName, Delegate action)
        {
            if (!_clientEvents.ContainsKey(eventName))
            {
                _clientEvents.Add(eventName, new List<Delegate> { action });
            }
            else
            {
                _clientEvents[eventName].Add(action);
            }

            Logger.Debug($"Registering [ClientEvent]: {eventName} on method: {action.Method.Name}.");
        }

        public void UnregisterServerEvent(string eventName)
        {
            if (_serverEvents.ContainsKey(eventName))
            {
                _serverEvents.Remove(eventName);

                Logger.Debug($"Removing [ServerEvent]: {eventName}");
            }
            else
            {
                Logger.Debug($"Unable to remove [ServerEvent]: {eventName}");
            }
        }

        public void UnregisterClientEvent(string eventName)
        {
            if (_clientEvents.ContainsKey(eventName))
            {
                _clientEvents.Remove(eventName);

                Logger.Debug($"Removing [ClientEvent]: {eventName}");
            }
            else
            {
                Logger.Debug($"Unable to remove [ClientEvent]: {eventName}");
            }
        }

        internal void RegisterInternalServerEvent(ServerEventAttribute eventAttr, object classObj, MethodInfo method)
        {
            RegisterServerEvent(eventAttr.Event, Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method));
        }

        internal void RegisterInternalClientEvent(ClientEventAttribute eventAttr, object classObj, MethodInfo method)
        {
            RegisterClientEvent(eventAttr.Event, Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method));
        }

        private void OnTriggerEvent([FromSource] Player player, string eventName, List<object> args)
        {
            Logger.Debug("Receive event from client: " + player.Name + ", " + eventName);

            var client = _container.Resolve<ClientService>().Get(player);
            var newArgs = new List<object> { client };
            args.ForEach(x => newArgs.Add(x));
            EmitLocalClient(eventName, newArgs.ToArray());
        }

        #region Internal Server Events

        internal void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic kick, dynamic deferrals)
        {
            PlayerConnecting?.Invoke(this, new PlayerConnectingEventArgs(player, kick, deferrals));
            EmitLocalServer("PlayerConnecting", new PlayerConnectingEventArgs(player, kick, deferrals));
        }

        internal void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            PlayerDisconnecting?.Invoke(this, new PlayerDisconnectingEventArgs(player, reason));
            EmitLocalServer("PlayerDisconnecting", new PlayerDisconnectingEventArgs(player, reason));
        }

        internal void OnResourceStop(string resource)
        {
            ResourceStop?.Invoke(this, new ResourceStopEventArgs(resource));
            EmitLocalServer("ResourceStop", new ResourceStopEventArgs(resource));
        }

        internal void OnResourceStart(string resource)
        {
            ResourceStart?.Invoke(this, new ResourceStartEventArgs(resource));
            EmitLocalServer("ResourceStart", new ResourceStartEventArgs(resource));
        }

        internal void OnResourceListRefresh()
        {
            ResourceListRefresh?.Invoke(this, new EventArgs());
            EmitLocalServer("ResourceListRefresh");
        }

        internal void OnResourceStarting(string resource)
        {
            ResourceStarting?.Invoke(this, new ResourceStartingEventArgs(resource));
            EmitLocalServer("ResourceStarting", new ResourceStartingEventArgs(resource));
        }

        internal void OnServerResourceStart(string resource)
        {
            ServerResourceStart?.Invoke(this, new ServerResourceStartEventArgs(resource));
            EmitLocalServer("ServerResourceStart", new ServerResourceStartEventArgs(resource));
        }

        internal void OnServerResourceStop(string resource)
        {
            ServerResourceStop?.Invoke(this, new ServerResourceStopEventArgs(resource));
            EmitLocalServer("ServerResourceStop", new ServerResourceStopEventArgs(resource));
        }

        internal void OnPlayerJoining(string source, string oldId)
        {
            PlayerJoining?.Invoke(this, new PlayerJoiningEventArgs(source, oldId));
            EmitLocalServer("PlayerJoining", new PlayerJoiningEventArgs(source, oldId));
        }

        internal void OnEntityCreated(int handle)
        {
            EntityCreated?.Invoke(this, new EntityCreatedEventArgs(handle));
            EmitLocalServer("EntityCreated", new EntityCreatedEventArgs(handle));
        }

        internal void OnEntityCreating(int handle)
        {
            EntityCreating?.Invoke(this, new EntityCreatingEventArgs(handle));
            EmitLocalServer("EntityCreating", new EntityCreatingEventArgs(handle));
        }

        internal void OnEntityRemoved(int handle)
        {
            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(handle));
            EmitLocalServer("EntityRemoved", new EntityRemovedEventArgs(handle));
        }

        internal void OnPlayerEnteredScope(object data)
        {
            PlayerEnteredScope?.Invoke(this, new PlayerEnteredScopeEventArgs(data));
            EmitLocalServer("PlayerEnteredScope", new PlayerEnteredScopeEventArgs(data));
        }

        internal void OnPlayerLeftScope(object data)
        {
            PlayerLeftScope?.Invoke(this, new PlayerLeftScopeEventArgs(data));
            EmitLocalServer("PlayerLeftScope", new PlayerLeftScopeEventArgs(data));
        }

        internal void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            HttpResponse?.Invoke(this, new HttpResponseEventArgs(token, status, text, header));
            EmitLocalServer("HttpResponse", new HttpResponseEventArgs(token, status, text, header));
        }

        #endregion
    }
}
