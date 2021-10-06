using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Rpc;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Average.Server.Services
{
    internal class RpcService : IService
    {
        private readonly EventService _eventService;

        internal delegate object RpcCallback(params object[] args);

        private readonly Dictionary<string, Delegate> _events = new();

        public RpcService(EventService eventService)
        {
            _eventService = eventService;

            OnRequest<int>("rpc:test2", (client, cb, age) =>
            {
                Logger.Debug("Age is: " + age);
                cb(age > 20);
            });

            OnRequest("player_count", (client, cb) =>
            {
                Logger.Debug("Send player count: " + 128);
                cb(128);
            });

            Logger.Write("RpcService", "Initialized successfully");
        }

        [ClientCommand("rpc:test")]
        private async void RpcTestCommand(Client client)
        {
            Logger.Error("Exec command: rpc:test");
            OnResponse<string>("rpc:test", ageResult =>
            {
                Logger.Error("rpc:test response: " + ageResult);
            }).Emit(client, "rpc:test", 24);

            Logger.Error("Getting value..");
            var result = await Request<float>(client, "GetPedScale");
            Logger.Error("Ped scale: " + result.Item1);
        }

        internal void TriggerResponse(string @event, string response)
        {
            if (_events.ContainsKey(@event))
            {
                _events[@event].DynamicInvoke(response);
            }
        }

        internal void OnInternalRequest(Client client, string @event, string request)
        {
            var message = request.Convert<RpcMessage>();

            if (_events.ContainsKey(@event))
            {
                var newArgs = new List<object>();

                newArgs.Add(client);
                newArgs.Add(new RpcCallback(args =>
                {
                    var response = new RpcMessage();
                    response.Event = @event;
                    response.Args = args.ToList();

                    _eventService.EmitClient(client, "rpc:send_response", @event, response.ToJson());
                    return args;
                }));

                // Need to skip two first args (Client & RpcCallback) args
                var methodParams = _events[@event].Method.GetParameters().Skip(2).ToList();

                for (int i = 0; i < methodParams.Count; i++)
                {
                    var arg = message.Args[i];
                    var param = methodParams[i];

                    if (arg.GetType() != param.ParameterType)
                    {
                        if (arg.GetType() == typeof(JArray))
                        {
                            // Need to convert arg or type JArray to param type if is it not the same
                            var array = arg as JArray;
                            var newArg = array.ToObject(param.ParameterType);
                            newArgs.Add(newArg);
                        }
                        else
                        {
                            // Need to convert arg type to param type if is it not the same
                            var newArg = Convert.ChangeType(arg, param.ParameterType);
                            newArgs.Add(newArg);
                        }
                    }
                    else
                    {
                        newArgs.Add(arg);
                    }
                }

                _events[@event].DynamicInvoke(newArgs.ToArray());
            }
        }

        internal void OnInternalResponse(string @event, Delegate callback)
        {
            Action<string> action = null;
            action = response =>
            {
                Unregister(@event);

                var message = response.Convert<RpcMessage>();
                var @params = callback.Method.GetParameters().ToList();
                var newArgs = new List<object>();

                for (int i = 0; i < message.Args.Count; i++)
                {
                    var arg = message.Args[i];
                    var param = @params[i];

                    if (arg.GetType() != param.ParameterType)
                    {
                        if (arg.GetType() == typeof(JArray))
                        {
                            // Need to convert arg or type JArray to param type if is it not the same
                            var array = arg as JArray;
                            var newArg = array.ToObject(param.ParameterType);
                            newArgs.Add(newArg);
                        }
                        else
                        {
                            // Need to convert arg type to param type if is it not the same
                            var newArg = Convert.ChangeType(arg, param.ParameterType);
                            newArgs.Add(newArg);
                        }
                    }
                    else
                    {
                        newArgs.Add(arg);
                    }
                }

                callback.DynamicInvoke(newArgs.ToArray());
            };

            Register(@event, action);
        }

        private void Register(string @event, Delegate callback)
        {
            if (!_events.ContainsKey(@event))
            {
                _events.Add(@event, callback);
            }
        }

        private void Unregister(string @event)
        {
            if (_events.ContainsKey(@event))
            {
                _events.Remove(@event);
            }
        }

        internal void Emit(Client client, string @event, params object[] args)
        {
            var message = new RpcMessage();
            message.Event = @event;
            message.Args = args.ToList();

            _eventService.EmitClient(client, "rpc:trigger_event", message.Event, message.ToJson());
        }

        #region OnRequest<,>

        internal void OnRequest(string @event, Action<Client, RpcCallback> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1>(string @event, Action<Client, RpcCallback, T1> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2>(string @event, Action<Client, RpcCallback, T1, T2> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3>(string @event, Action<Client, RpcCallback, T1, T2, T3> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5, T6>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5, T6> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5, T6, T7> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5, T6, T7, T8>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5, T6, T7, T8> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
        {
            Register(@event, callback);
        }

        internal void OnRequest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string @event, Action<Client, RpcCallback, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
        {
            Register(@event, callback);
        }

        #endregion

        #region OnResponse<,>

        public RpcService OnResponse(string @event, Action callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1>(string @event, Action<T1> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2>(string @event, Action<T1, T2> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3>(string @event, Action<T1, T2, T3> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4>(string @event, Action<T1, T2, T3, T4> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5>(string @event, Action<T1, T2, T3, T4, T5> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5, T6>(string @event, Action<T1, T2, T3, T4, T5, T6> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<T1, T2, T3, T4, T5, T6, T7> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5, T6, T7, T8>(string @event, Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string @event, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        public RpcService OnResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string @event, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
        {
            OnInternalResponse(@event, callback);
            return this;
        }

        #endregion

        public async Task<Tuple<T1>> Request<T1>(Client client, string @event, params object[] args)
        {
            object result = null;

            OnResponse<T1>(@event, arg0 =>
            {
                result = Tuple.Create(arg0);
            }).Emit(client, @event, args);

            while (result == null) await BaseScript.Delay(1);

            return result as Tuple<T1>;
        }

        #region Native Game Call

        internal void NativeCall(Client client, long native, params object[] args)
        {
            _eventService.EmitClient(client, "rpc:native_call", native, args);
        }

        internal void NativeCall(Client client, ulong native, params object[] args)
        {
            _eventService.EmitClient(client, "rpc:native_call", native, args);
        }

        internal void NativeCall(Client client, Hash native, params object[] args)
        {
            _eventService.EmitClient(client, "rpc:native_call", (long)native, args);
        }

        internal void NativeCall(Client client, string native, params object[] args)
        {
            _eventService.EmitClient(client, "rpc:native_call", (long)GetHashKey(native), args);
        }

        internal void GlobalNativeCall(long native, params object[] args)
        {
            _eventService.EmitClients("rpc:native_call", native, args);
        }

        internal void GlobalNativeCall(ulong native, params object[] args)
        {
            _eventService.EmitClients("rpc:native_call", native, args);
        }

        internal void GlobalNativeCall(Hash native, params object[] args)
        {
            _eventService.EmitClients("rpc:native_call", (long)native, args);
        }

        internal void GlobalNativeCall(string native, params object[] args)
        {
            _eventService.EmitClients("rpc:native_call", (long)GetHashKey(native), args);
        }

        #endregion
    }
}
