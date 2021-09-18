using Average.Server.Framework.Extensions;
using Average.Shared.Rpc;
using CitizenFX.Core;
using System;
using System.Linq;

namespace Average.Server.Framework.Rpc
{
    internal class RpcRequest
    {
        private RpcMessage _message;
        private readonly RpcHandler _handler;
        private readonly RpcTrigger _trigger;
        private readonly RpcSerializer _serializer;

        public delegate object RpcCallback(params object[] args);

        public RpcRequest(RpcHandler handler, RpcTrigger trigger, RpcSerializer serializer)
        {
            _message = new RpcMessage();
            _handler = handler;
            _trigger = trigger;
            _serializer = serializer;
        }

        public RpcRequest Event(string eventName)
        {
            _message.Event = eventName;
            return this;
        }

        public void Emit(RpcMessage message)
        {
            _trigger.Trigger(message);
        }

        public void Emit(params object[] args)
        {
            foreach (var arg in args)
                _message.Args.Add(arg);

            _trigger.Trigger(_message);
        }

        public void On(Action action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1>(Action<T1> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2>(Action<T1, T2> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
        {
            _handler.Attach(_message.Event, action);
        }

        public void On(Action<RpcMessage, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                callback(_message, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        #region On<T>

        public void On<T1>(Action<T1, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();

                callback(arg1, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2>(Action<T1, T2, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();

                callback(arg1, arg2, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3>(Action<T1, T2, T3, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();

                callback(arg1, arg2, arg3, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4>(Action<T1, T2, T3, T4, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();

                callback(arg1, arg2, arg3, arg4, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();

                callback(arg1, arg2, arg3, arg4, arg5, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();
                var arg11 = _message.Args[10].Convert<T11>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();
                var arg11 = _message.Args[10].Convert<T11>();
                var arg12 = _message.Args[11].Convert<T12>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();
                var arg11 = _message.Args[10].Convert<T11>();
                var arg12 = _message.Args[11].Convert<T12>();
                var arg13 = _message.Args[12].Convert<T13>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();
                var arg11 = _message.Args[10].Convert<T11>();
                var arg12 = _message.Args[11].Convert<T12>();
                var arg13 = _message.Args[12].Convert<T13>();
                var arg14 = _message.Args[13].Convert<T14>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        public void On<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, RpcCallback> callback)
        {
            var action = new Action<string>(request =>
            {
                _message = _serializer.Deserialize<RpcMessage>(request);

                var arg1 = _message.Args[0].Convert<T1>();
                var arg2 = _message.Args[1].Convert<T2>();
                var arg3 = _message.Args[2].Convert<T3>();
                var arg4 = _message.Args[3].Convert<T4>();
                var arg5 = _message.Args[4].Convert<T5>();
                var arg6 = _message.Args[5].Convert<T6>();
                var arg7 = _message.Args[6].Convert<T7>();
                var arg8 = _message.Args[7].Convert<T8>();
                var arg9 = _message.Args[8].Convert<T9>();
                var arg10 = _message.Args[9].Convert<T10>();
                var arg11 = _message.Args[10].Convert<T11>();
                var arg12 = _message.Args[11].Convert<T12>();
                var arg13 = _message.Args[12].Convert<T13>();
                var arg14 = _message.Args[13].Convert<T14>();
                var arg15 = _message.Args[14].Convert<T15>();

                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args =>
                {
                    _message.Args = args.ToList();
                    _trigger.Trigger(_message);
                    return args;
                });
            });

            _handler.Attach(_message.Event, action);
        }

        #endregion

        public RpcRequest Target(Player player)
        {
            _message.Target = int.Parse(player.Handle);
            return this;
        }
    }
}
