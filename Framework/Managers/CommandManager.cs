using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using CitizenFX.Core.Native;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Average.Server.Framework.Managers
{
    internal class CommandManager
    {
        private readonly IContainer _container;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly List<ServerCommandAttribute> _serverCommands = new List<ServerCommandAttribute>();
        private readonly List<Command> _clientCommands = new List<Command>();

        internal class Command
        {
            public ClientCommandAttribute Attribute { get; }
            public ClientCommandAliasAttribute Alias { get; }

            public Command(ClientCommandAttribute attribute, ClientCommandAliasAttribute alias)
            {
                Attribute = attribute;
                Alias = alias;
            }
        }

        public CommandManager(IContainer container)
        {
            _container = container;

            Logger.Write("ServerCommandManager", "Initialized successfully");
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            // Register server commands
            foreach (var serviceType in types)
            {
                if (_container.IsRegistered(serviceType))
                {
                    // Continue if the service have the same type of this class
                    if (serviceType == GetType()) continue;

                    // Get service instance
                    var service = _container.GetService(serviceType);
                    var methods = serviceType.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ServerCommandAttribute>();
                        if (attr == null) continue;

                        RegisterInternalServerCommand(attr, service, method);
                    }
                }
            }

            // Register client commands
            foreach (var serviceType in types)
            {
                if (_container.IsRegistered(serviceType))
                {
                    // Continue if the service have the same type of this class
                    if (serviceType == GetType()) continue;

                    // Get service instance
                    var service = _container.GetService(serviceType);
                    var methods = serviceType.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ClientCommandAttribute>();
                        if (attr == null) continue;
                        var alisAttr = method.GetCustomAttribute<ClientCommandAliasAttribute>();

                        RegisterInternalClientCommand(attr, alisAttr, service, method);
                    }
                }
            }
        }

        internal void RegisterInternalServerCommand(ServerCommandAttribute cmdAttr, object classObj, MethodInfo method)
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
                        _serverCommands.Add(cmdAttr);
                    }
                    catch
                    {
                        Logger.Error($"Unable to convert server command arguments.");
                    }
                }
                else
                {
                    var usage = "";
                    methodParams.ToList().ForEach(x => usage += $"<[{x.ParameterType.Name}] {x.Name}> ");
                    Logger.Error($"Invalid server command usage: {cmdAttr.Command} {usage}.");
                }
            }), false);

            Logger.Debug($"Registering [ServerCommand]: {cmdAttr.Command} on method: {method.Name}");
        }

        internal void RegisterInternalClientCommand(ClientCommandAttribute commandAttr, ClientCommandAliasAttribute aliasAttr, object classObj, MethodInfo method)
        {
            _clientCommands.Add(new Command(commandAttr, aliasAttr));

            Logger.Debug($"Registering [Command]: {commandAttr.Command} on method: {method.Name} with alias: {(aliasAttr != null ? $"[{string.Join(", ", aliasAttr.Alias)}]" : "empty")}");
        }

        public IEnumerable<ServerCommandAttribute> GetServerCommands() => _serverCommands.AsEnumerable();
        public ServerCommandAttribute GetServerCommand(string command) => _serverCommands.Find(x => x.Command == command);
        public IEnumerable<Command> GetCommands() => _clientCommands.AsEnumerable();
        public Command GetCommand(string command) => _clientCommands.Find(x => x.Attribute.Command == command);
    }
}
