using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Events;
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
                logger.Debug($"Register internal event: {eventName}");
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
                logger.Debug($"Register event: {eventAttr.Event} on method: {method.Name}, args count: {methodParams.Count()}");
            }
            else
            {
                logger.Error($"Unable to register event: {eventAttr.Event} on method: {method.Name}, an event have already been registered with this event name.");
            }
        }

        #region Internal

        internal void InternalTriggerEvent(string eventName, List<object> args)
        {
            Emit(eventName, args.ToArray());
        }

        #endregion

        #region Events

        public event EventHandler<PlayerConnectingEventArgs> PlayerConnecting;
        public event EventHandler<PlayerDisconnectingEventArgs> PlayerDisconnecting;

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

        #endregion
    }
}
