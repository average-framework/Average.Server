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
            eventHandlers["avg.internal.trigger_event"] += new Action<string, List<object>>(InternalTriggerEvent);
        }

        public void Emit(string eventName, params object[] args)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName].ForEach(x => x.DynamicInvoke(args));
            }
            else
            {
                logger.Debug($"Calling internal event: {eventName}.");
                BaseScript.TriggerEvent(eventName, args);
            }
        }

        public void EmitClients(string eventName, params object[] args)
        {
            BaseScript.TriggerClientEvent("avg.internal.trigger_event", eventName, args);
        }

        public void EmitClient(Player player, string eventName, object[] args)
        {
            player.TriggerEvent("avg.internal.trigger_event", eventName, args);
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

        internal void InternalTriggerEvent(string eventName, List<object> args) => Emit(eventName, args.ToArray());

        #endregion

        #region Events

        public void OnPlayerConnecting(Player player, dynamic kick, dynamic deferrals)
        {
            if (PlayerConnecting != null)
            {
                PlayerConnecting(null, new PlayerConnectingEventArgs(player, kick, deferrals));
            }
        }

        public void OnPlayerDisconnecting(Player player, string reason)
        {
            if (PlayerDisconnecting != null)
            {
                PlayerDisconnecting(null, new PlayerDisconnectingEventArgs(player, reason));
            }
        }

        public void OnResourceStop(string resource)
        {
            if (ResourceStop != null)
            {
                ResourceStop(null, new ResourceStopEventArgs(resource));
            }
        }

        public void OnResourceStart(string resource)
        {
            if (ResourceStart != null)
            {
                ResourceStart(null, new ResourceStartEventArgs(resource));
            }
        }

        public void OnResourceListRefresh()
        {
            if (ResourceListRefresh != null)
            {
                ResourceListRefresh(null, new EventArgs());
            }
        }

        public void OnResourceStarting(string resource)
        {
            if (ResourceStarting != null)
            {
                ResourceStarting(null, new ResourceStartingEventArgs(resource));
            }
        }

        public void OnServerResourceStart(string resource)
        {
            if (ServerResourceStart != null)
            {
                ServerResourceStart(null, new ServerResourceStartEventArgs(resource));
            }
        }

        public void OnServerResourceStop(string resource)
        {
            if (ServerResourceStop != null)
            {
                ServerResourceStop(null, new ServerResourceStopEventArgs(resource));
            }
        }

        public void OnPlayerJoining(string source, string oldId)
        {
            if (PlayerJoining != null)
            {
                PlayerJoining(null, new PlayerJoiningEventArgs(source, oldId));
            }
        }

        public void OnEntityCreated(int handle)
        {
            if (EntityCreated != null)
            {
                EntityCreated(null, new EntityCreatedEventArgs(handle));
            }
        }

        public void OnEntityCreating(int handle)
        {
            if (EntityCreating != null)
            {
                EntityCreating(null, new EntityCreatingEventArgs(handle));
            }
        }

        public void OnEntityRemoved(int handle)
        {
            if (EntityRemoved != null)
            {
                EntityRemoved(null, new EntityRemovedEventArgs(handle));
            }
        }

        public void OnPlayerEnteredScope(object data)
        {
            if (PlayerEnteredScope != null)
            {
                PlayerEnteredScope(null, new PlayerEnteredScopeEventArgs(data));
            }
        }

        public void OnPlayerLeftScope(object data)
        {
            if (PlayerLeftScope != null)
            {
                PlayerLeftScope(null, new PlayerLeftScopeEventArgs(data));
            }
        }

        public void OnHttpResponse(int token, int status, string text, dynamic header)
        {
            if (HttpResponse != null)
            {
                HttpResponse(null, new HttpResponseEventArgs(token, status, text, header));
            }
        }

        #endregion
    }
}
