using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using CitizenFX.Core.Native;
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

        private readonly Dictionary<string, List<Delegate>> _events = new Dictionary<string, List<Delegate>>();

        public EventManager(IContainer container, EventHandlerDictionary eventHandlers)
        {
            _container = container;
            _eventHandlers = eventHandlers;

            #region Events

            // Internal events
            _eventHandlers["__cfx_internal:httpResponse"] += new Action<int, int, string, dynamic>(OnHttpResponse);

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

            Logger.Write("EventManager", "Initialized successfully");
        }

        #region Event Handlers

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

        #endregion

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

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

                        RegisterInternalEvent(attr, _service, method);
                    }
                }
            }
        }

        public void Emit(string eventName, params object[] args)
        {
            //BaseScript.TriggerEvent(eventName, args);

            if (_events.ContainsKey(eventName))
            {
                _events[eventName].ForEach(x => x.DynamicInvoke(args));
            }
            //else
            //{
            //    BaseScript.TriggerEvent(eventName, args);
            //}
        }

        public void EmitClients(string eventName, params object[] args)
        {
            BaseScript.TriggerClientEvent(eventName, args);
        }

        public void EmitClient(Player player, string eventName, params object[] args)
        {
            player.TriggerEvent(eventName, args);
        }

        public void RegisterEvent(string eventName, Delegate action)
        {
            //_eventHandlers[eventName] += action;

            if (!_events.ContainsKey(eventName))
            {
                _events.Add(eventName, new List<Delegate> { action });
            }
            else
            {
                _events[eventName].Add(action);
            }

            Logger.Debug($"Registering [ServerEvent]: {eventName} on method: {action.Method.Name}.");
        }

        public void UnregisterEvent(string eventName, Delegate action)
        {
            _eventHandlers[eventName] -= action;

            Logger.Debug($"Unregistering [ServerEvent]: {eventName} on method: {action.Method.Name}.");
        }

        internal void RegisterInternalEvent(ServerEventAttribute eventAttr, object classObj, MethodInfo method)
        {
            RegisterEvent(eventAttr.Event, Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method));
        }

        #region Internal Events

        internal void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic kick, dynamic deferrals)
        {
            PlayerConnecting?.Invoke(this, new PlayerConnectingEventArgs(player, kick, deferrals));
            Emit("PlayerConnecting", new PlayerConnectingEventArgs(player, kick, deferrals));
        }

        internal void OnPlayerDisconnecting([FromSource] Player player, string reason)
        {
            PlayerDisconnecting?.Invoke(this, new PlayerDisconnectingEventArgs(player, reason));
            Emit("PlayerDisconnecting", new PlayerDisconnectingEventArgs(player, reason));
        }

        internal void OnResourceStop(string resource)
        {
            ResourceStop?.Invoke(this, new ResourceStopEventArgs(resource));
            Emit("ResourceStop", new ResourceStopEventArgs(resource));
        }

        internal void OnResourceStart(string resource)
        {
            ResourceStart?.Invoke(this, new ResourceStartEventArgs(resource));
            Emit("ResourceStart", new ResourceStartEventArgs(resource));
        }

        internal void OnResourceListRefresh()
        {
            ResourceListRefresh?.Invoke(this, new EventArgs());
            Emit("ResourceListRefresh");
        }

        internal void OnResourceStarting(string resource)
        {
            ResourceStarting?.Invoke(this, new ResourceStartingEventArgs(resource));
            Emit("ResourceStarting", new ResourceStartingEventArgs(resource));
        }

        internal void OnServerResourceStart(string resource)
        {
            ServerResourceStart?.Invoke(this, new ServerResourceStartEventArgs(resource));
            Emit("ServerResourceStart", new ServerResourceStartEventArgs(resource));
        }

        internal void OnServerResourceStop(string resource)
        {
            ServerResourceStop?.Invoke(this, new ServerResourceStopEventArgs(resource));
            Emit("ServerResourceStop", new ServerResourceStopEventArgs(resource));
        }

        internal void OnPlayerJoining(string source, string oldId)
        {
            PlayerJoining?.Invoke(this, new PlayerJoiningEventArgs(source, oldId));
            Emit("PlayerJoining", new PlayerJoiningEventArgs(source, oldId));
        }

        internal void OnEntityCreated(int handle)
        {
            EntityCreated?.Invoke(this, new EntityCreatedEventArgs(handle));
            Emit("EntityCreated", new EntityCreatedEventArgs(handle));
        }

        internal void OnEntityCreating(int handle)
        {
            EntityCreating?.Invoke(this, new EntityCreatingEventArgs(handle));
            Emit("EntityCreating", new EntityCreatingEventArgs(handle));
        }

        internal void OnEntityRemoved(int handle)
        {
            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(handle));
            Emit("EntityRemoved", new EntityRemovedEventArgs(handle));
        }

        internal void OnPlayerEnteredScope(object data)
        {
            PlayerEnteredScope?.Invoke(this, new PlayerEnteredScopeEventArgs(data));
            Emit("PlayerEnteredScope", new PlayerEnteredScopeEventArgs(data));
        }

        internal void OnPlayerLeftScope(object data)
        {
            PlayerLeftScope?.Invoke(this, new PlayerLeftScopeEventArgs(data));
            Emit("PlayerLeftScope", new PlayerLeftScopeEventArgs(data));
        }

        internal void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            HttpResponse?.Invoke(this, new HttpResponseEventArgs(token, status, text, header));
            Emit("HttpResponse", new HttpResponseEventArgs(token, status, text, header));
        }

        #endregion
    }
}
