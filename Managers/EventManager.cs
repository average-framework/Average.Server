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
    public class EventManager : InternalPlugin, IEventManager
    {
        private static Dictionary<string, List<Delegate>> _events = new Dictionary<string, List<Delegate>>();

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

        public override void OnInitialized()
        {
            #region Event

            Main.eventHandlers["__cfx_internal:httpResponse"] += new Action<int, int, string, dynamic>(OnHttpResponse);
            Main.eventHandlers["avg.internal.trigger_event"] += new Action<Player, string, List<object>>(TriggerInternalEvent);

            #endregion
        }

        public void Emit(string eventName, params object[] args)
        {
            if (_events.ContainsKey(eventName))
            {
                // Log.Debug($"Calling event: {eventName}.");
                _events[eventName].ForEach(x => x.DynamicInvoke(args));
            }
            else
            {
                // Log.Debug($"Calling external event: {eventName}.");
                BaseScript.TriggerEvent(eventName, args);
            }
        }

        public void EmitClients(string eventName, params object[] args)
        {
            BaseScript.TriggerClientEvent("avg.internal.trigger_event", eventName, args);
        }

        public void EmitClient([FromSource] Player player, string eventName, params object[] args)
        {
            player.TriggerEvent("avg.internal.trigger_event", eventName, args);
        }

        internal static void RegisterInternalEvent(string eventName, Delegate action)
        {
            if (!_events.ContainsKey(eventName))
                _events.Add(eventName, new List<Delegate>() { action });
            else
                _events[eventName].Add(action);

            // Log.Debug($"Registering internal event: {eventName}");
        }

        internal void UnregisterInternalEvent(string eventName)
        {
            if (_events.ContainsKey(eventName))
            {
                _events.Remove(eventName);
                Log.Debug($"Unregister event: {eventName}");
            }
            else
            {
                Log.Error($"Unable to unregister event: {eventName}.");
            }
        }

        internal void UnregisterInternalEventAction(string eventName, Delegate action)
        {
            if (_events.ContainsKey(eventName) && _events[eventName].Contains(action))
            {
                _events[eventName].Remove(action);
                Log.Debug($"Unregister event action: {eventName}");
            }
            else
            {
                Log.Error($"Unable to unregister event action: {eventName}.");
            }
        }

        internal static void RegisterInternalEvent(MethodInfo method, ServerEventAttribute eventAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            var action = Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
            RegisterInternalEvent(eventAttr.Event, action);

            Log.Debug($"Registering [Event] attribute: {eventAttr.Event} on method: {method.Name}, args count: {methodParams.Count()}, [{string.Join(", ", methodParams.Select(x => x.ParameterType))}]");
        }

        internal void TriggerInternalEvent([FromSource] Player player, string eventName, List<object> args)
        {
            var newArgs = new List<object> {int.Parse(player.Handle)};
            args.ForEach(x => newArgs.Add(x));
            Emit(eventName, newArgs.ToArray());
        }
        
        #region Event

        internal async void OnPlayerConnecting(Player player, dynamic kick, dynamic deferrals)
        {
            await Main.loader.IsReady();
            PlayerConnecting?.Invoke(this, new PlayerConnectingEventArgs(player, kick, deferrals));
            Emit("PlayerConnecting", int.Parse(player.Handle), kick, deferrals);
        }
        
        internal async void OnPlayerDisconnecting(Player player, string reason)
        {
            await Main.loader.IsReady();
            PlayerDisconnecting?.Invoke(this, new PlayerDisconnectingEventArgs(player, reason));
            Emit("PlayerDisconnecting", int.Parse(player.Handle), reason);
        }

        internal async void OnResourceStop(string resource)
        {
            await Main.loader.IsReady();
            ResourceStop?.Invoke(this, new ResourceStopEventArgs(resource));
            Emit("ResourceStop", resource);
        }

        internal async void OnResourceStart(string resource)
        {
            await Main.loader.IsReady();
            ResourceStart?.Invoke(this, new ResourceStartEventArgs(resource));
            Emit("ResourceStart", resource);
        }

        internal async void OnResourceListRefresh()
        {
            await Main.loader.IsReady();
            ResourceListRefresh?.Invoke(this, new EventArgs());
            Emit("ResourceListRefresh");
        }

        internal async void OnResourceStarting(string resource)
        {
            await Main.loader.IsReady();
            ResourceStarting?.Invoke(this, new ResourceStartingEventArgs(resource));
            Emit("ResourceStarting", resource);
        }

        internal async void OnServerResourceStart(string resource)
        {
            await Main.loader.IsReady();
            ServerResourceStart?.Invoke(this, new ServerResourceStartEventArgs(resource));
            Emit("ServerResourceStart", resource);
        }

        internal async void OnServerResourceStop(string resource)
        {
            await Main.loader.IsReady();
            ServerResourceStop?.Invoke(this, new ServerResourceStopEventArgs(resource));
            Emit("ServerResourceStop", resource);
        }

        internal async void OnPlayerJoining(string source, string oldId)
        {
            await Main.loader.IsReady();
            PlayerJoining?.Invoke(this, new PlayerJoiningEventArgs(source, oldId));
            Emit("PlayerJoining", source, oldId);
        }

        internal async void OnEntityCreated(int handle)
        {
            await Main.loader.IsReady();
            EntityCreated?.Invoke(this, new EntityCreatedEventArgs(handle));
            Emit("EntityCreated", handle);
        }

        internal async void OnEntityCreating(int handle)
        {
            await Main.loader.IsReady();
            EntityCreating?.Invoke(this, new EntityCreatingEventArgs(handle));
            Emit("EntityCreating", handle);
        }

        internal async void OnEntityRemoved(int handle)
        {
            await Main.loader.IsReady();
            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(handle));
            Emit("EntityRemoved", handle);
        }

        internal async void OnPlayerEnteredScope(object data)
        {
            await Main.loader.IsReady();
            PlayerEnteredScope?.Invoke(this, new PlayerEnteredScopeEventArgs(data));
            Emit("PlayerEnteredScope", data);
        }

        internal async void OnPlayerLeftScope(object data)
        {
            await Main.loader.IsReady();
            PlayerLeftScope?.Invoke(this, new PlayerLeftScopeEventArgs(data));
            Emit("PlayerLeftScope", data);
        }

        internal async void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            await Main.loader.IsReady();
            HttpResponse?.Invoke(this, new HttpResponseEventArgs(token, status, text, header));
            Emit("HttpResponse", token, status, text, header);
        }

        #endregion
    }
}
