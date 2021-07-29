﻿using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Events;
using SDK.Server.Managers;
using SDK.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Managers
{
    public class EventManager : IEventManager
    {
        Dictionary<string, Delegate> events;
        Logger logger;

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

        public EventManager(EventHandlerDictionary eventHandlers, Logger logger)
        {
            this.logger = logger;
            events = new Dictionary<string, Delegate>();

            eventHandlers["avg.internal.trigger_event"] += new Action<string, List<object>>(InternalTriggerEvent);
        }

        public void Emit(string eventName, params object[] args)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName].DynamicInvoke(args);
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
            {
                events.Add(eventName, action);
                logger.Debug($"Registering internal event: {eventName}");
            }
            else
            {
                logger.Error($"Unable to register internal event: {eventName}, an event have already been registered with this event name.");
            }
        }

        public void RegisterEvent(MethodInfo method, EventAttribute eventAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            if (!events.ContainsKey(eventAttr.Event))
            {
                var action = Action.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
                events.Add(eventAttr.Event, action);
                logger.Debug($"Registering [Event] attribute: {eventAttr.Event} on method: {method.Name}, args count: {methodParams.Count()}");
            }
            else
            {
                logger.Error($"Unable to register [Event] attribute: {eventAttr.Event} on method: {method.Name}, an event have already been registered with this event name.");
            }
        }

        #region Internal

        internal void InternalTriggerEvent(string eventName, List<object> args)
        {
            Emit(eventName, args.ToArray());
        }

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

        #endregion
    }
}
