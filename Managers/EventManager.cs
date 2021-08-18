using CitizenFX.Core;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Events;
using SDK.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Managers
{
    public class EventManager : IEventManager
    {
        Logger logger;

        Dictionary<string, List<Delegate>> events;

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


        public EventManager(EventHandlerDictionary eventHandlers, Logger logger)
        {
            this.logger = logger;

            events = new Dictionary<string, List<Delegate>>();

            eventHandlers["__cfx_internal:httpResponse"] += new Action<int, int, string, dynamic>(OnHttpResponse);
            eventHandlers["avg.internal.trigger_event"] += new Action<string, Player, List<object>>(InternalTriggerEvent);
        }

        public void Emit(string eventName, params object[] args)
        {
            if (events.ContainsKey(eventName))
            {
                logger.Debug($"Calling event: {eventName}.");
                events[eventName].ForEach(x => x.DynamicInvoke(args));
            }
            else
            {
                logger.Debug($"Calling external event: {eventName}.");
                BaseScript.TriggerEvent(eventName, args);
            }
        }

        public void EmitClients(string eventName, params object[] args)
        {
            BaseScript.TriggerClientEvent("avg.internal.trigger_event", eventName, args);
        }

        public void EmitClient(Player player, string eventName, object[] args)
        {
            player.TriggerEvent("avg.internal.trigger_event", eventName, player, args);
        }

        public void RegisterInternalEvent(string eventName, Delegate action)
        {
            if (!events.ContainsKey(eventName))
                events.Add(eventName, new List<Delegate>() { action });
            else
                events[eventName].Add(action);

            logger.Debug($"Registering internal event: {eventName}");
        }

        public void UnregisterInternalEvent(string eventName)
        {
            if (events.ContainsKey(eventName))
            {
                events.Remove(eventName);
                logger.Debug($"Unregister event: {eventName}");
            }
            else
            {
                logger.Error($"Unable to unregister event: {eventName}.");
            }
        }

        public void UnregisterInternalEventAction(string eventName, Delegate action)
        {
            if (events.ContainsKey(eventName) && events[eventName].Contains(action))
            {
                events[eventName].Remove(action);
                logger.Debug($"Unregister event action: {eventName}");
            }
            else
            {
                logger.Error($"Unable to unregister event action: {eventName}.");
            }
        }

        public void RegisterEvent(MethodInfo method, ServerEventAttribute eventAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            var action = Action.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
            RegisterInternalEvent(eventAttr.Event, action);

            logger.Debug($"Registering [Event] attribute: {eventAttr.Event} on method: {method.Name}, args count: {methodParams.Count()}");
        }

        #region Internal

        internal void InternalTriggerEvent(string eventName, [FromSource] Player player, List<object> args)
        {
            var newArgs = new List<object>();
            newArgs.Add(int.Parse(player.Handle));

            foreach (var arg in args)
                newArgs.Add(arg);

            Emit(eventName, newArgs.ToArray());
        }

        #endregion

        #region Events

        public async void OnPlayerConnecting(Player player, dynamic kick, dynamic deferrals)
        {
            await Main.loader.IsReady();
            PlayerConnecting?.Invoke(this, new PlayerConnectingEventArgs(player, kick, deferrals));
            Emit("PlayerConnecting", int.Parse(player.Handle), kick, deferrals);
        }

        public async void OnPlayerDisconnecting(Player player, string reason)
        {
            await Main.loader.IsReady();
            PlayerDisconnecting?.Invoke(this, new PlayerDisconnectingEventArgs(player, reason));
            Emit("PlayerDisconnecting", int.Parse(player.Handle), reason);
        }

        public async void OnResourceStop(string resource)
        {
            await Main.loader.IsReady();
            ResourceStop?.Invoke(this, new ResourceStopEventArgs(resource));
            Emit("ResourceStop", resource);
        }

        public async void OnResourceStart(string resource)
        {
            await Main.loader.IsReady();
            ResourceStart?.Invoke(this, new ResourceStartEventArgs(resource));
            Emit("ResourceStart", resource);
        }

        public async void OnResourceListRefresh()
        {
            await Main.loader.IsReady();
            ResourceListRefresh?.Invoke(this, new EventArgs());
            Emit("ResourceListRefresh");
        }

        public async void OnResourceStarting(string resource)
        {
            await Main.loader.IsReady();
            ResourceStarting?.Invoke(this, new ResourceStartingEventArgs(resource));
            Emit("ResourceStarting", resource);
        }

        public async void OnServerResourceStart(string resource)
        {
            await Main.loader.IsReady();
            ServerResourceStart?.Invoke(this, new ServerResourceStartEventArgs(resource));
            Emit("ServerResourceStart", resource);
        }

        public async void OnServerResourceStop(string resource)
        {
            await Main.loader.IsReady();
            ServerResourceStop?.Invoke(this, new ServerResourceStopEventArgs(resource));
            Emit("ServerResourceStop", resource);
        }

        public async void OnPlayerJoining(string source, string oldId)
        {
            await Main.loader.IsReady();
            PlayerJoining?.Invoke(this, new PlayerJoiningEventArgs(source, oldId));
            Emit("PlayerJoining", source, oldId);
        }

        public async void OnEntityCreated(int handle)
        {
            await Main.loader.IsReady();
            EntityCreated?.Invoke(this, new EntityCreatedEventArgs(handle));
            Emit("EntityCreated", handle);
        }

        public async void OnEntityCreating(int handle)
        {
            await Main.loader.IsReady();
            EntityCreating?.Invoke(this, new EntityCreatingEventArgs(handle));
            Emit("EntityCreating", handle);
        }

        public async void  OnEntityRemoved(int handle)
        {
            await Main.loader.IsReady();
            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(handle));
            Emit("EntityRemoved", handle);
        }

        public async void OnPlayerEnteredScope(object data)
        {
            await Main.loader.IsReady();
            PlayerEnteredScope?.Invoke(this, new PlayerEnteredScopeEventArgs(data));
            Emit("PlayerEnteredScope", data);
        }

        public async void OnPlayerLeftScope(object data)
        {
            await Main.loader.IsReady();
            PlayerLeftScope?.Invoke(this, new PlayerLeftScopeEventArgs(data));
            Emit("PlayerLeftScope", data);
        }

        public async void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            await Main.loader.IsReady();
            HttpResponse?.Invoke(this, new HttpResponseEventArgs(token, status, text, header));
            Emit("HttpResponse", token, status, text, header);
        }

        #endregion
    }
}
