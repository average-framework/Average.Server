using CitizenFX.Core.Native;
using DryIoc;
using SDK.Server;
using SDK.Server.Commands;
using SDK.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Average.Server.Managers
{
    public class CommandManager : ICommandManager
    {
        private readonly IContainer _container;
        private readonly List<ServerCommandAttribute> _commands = new List<ServerCommandAttribute>();
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public CommandManager(IContainer container)
        {
            _container = container;

            Logger.Write("CommandManager", "Initialized successfully");
        }

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
                        var cmdAttr = method.GetCustomAttribute<ServerCommandAttribute>();
                        if (cmdAttr == null) continue;

                        RegisterInternalCommand(cmdAttr, _service, method);
                    }
                }
            }
        }

        internal void RegisterInternalCommand(ServerCommandAttribute cmdAttr, object classObj, MethodInfo method)
        {
            var methodParams = method.GetParameters();

            API.RegisterCommand(cmdAttr.Command, new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var newArgs = new List<object>();

                if (args.Count == methodParams.Length)
                {
                    try
                    {
                        args.ForEach(x => newArgs.Add(Convert.ChangeType(x, methodParams[args.FindIndex(p => p == x)].ParameterType)));
                        method.Invoke(classObj, newArgs.ToArray());
                        _commands.Add(cmdAttr);
                    }
                    catch
                    {
                        Logger.Error($"Unable to convert command arguments.");
                    }
                }
                else
                {
                    var usage = "";
                    methodParams.ToList().ForEach(x => usage += $"<[{x.ParameterType.Name}] {x.Name}> ");
                    Logger.Error($"Invalid command usage: {cmdAttr.Command} {usage}.");
                }
            }), false);

            Logger.Debug($"Registering [Command] attribute: {cmdAttr.Command} on method: {method.Name}");
        }

        public IEnumerable<ServerCommandAttribute> GetCommands() => _commands.AsEnumerable();

        public ServerCommandAttribute GetCommand(string command) => _commands.Find(x => x.Command == command);

        public int Count() => _commands.Count();
    }
}
