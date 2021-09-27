using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using CitizenFX.Core.Native;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Services
{
    internal class CommandService : IService
    {
        private readonly IContainer _container;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly List<Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>> _serverCommands = new List<Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>>();
        private readonly List<Command> _clientCommands = new List<Command>();

        internal class Command
        {
            public ClientCommandAttribute Attribute { get; }
            public CommandAliasAttribute Alias { get; }
            public Delegate Action { get; }

            public Command(ClientCommandAttribute attribute, CommandAliasAttribute alias, Delegate action)
            {
                Attribute = attribute;
                Alias = alias;
                Action = action;
            }
        }

        public CommandService(IContainer container)
        {
            _container = container;

            Logger.Write("CommandManager", "Initialized successfully");
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
                        var aliasAttr = method.GetCustomAttribute<CommandAliasAttribute>();

                        RegisterInternalServerCommand(attr, aliasAttr, service, method);
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
                        var aliasAttr = method.GetCustomAttribute<CommandAliasAttribute>();

                        RegisterInternalClientCommand(attr, aliasAttr, service, method);
                    }
                }
            }
        }

        private void RegisterServerCommand(string commandName, Delegate action)
        {
            API.RegisterCommand(commandName, new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var newArgs = new List<object>();
                var methodParams = action.Method.GetParameters();

                if (args.Count == methodParams.Length)
                {
                    try
                    {
                        args.ForEach(x => newArgs.Add(Convert.ChangeType(x, methodParams[args.FindIndex(p => p == x)].ParameterType)));
                        action.DynamicInvoke(newArgs.ToArray());
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
                    Logger.Error($"Invalid server command usage: {commandName} {usage}.");
                }
            }), false);

            Logger.Debug($"Registering [ServerCommand]: {commandName} on method: {action.Method.Name}");
        }

        internal void RegisterInternalServerCommand(ServerCommandAttribute cmdAttr, CommandAliasAttribute aliasAttr, object classObj, MethodInfo method)
        {
            var methodParams = method.GetParameters();
            var action = Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);

            RegisterServerCommand(cmdAttr.Command, action);

            if (aliasAttr != null)
            {
                foreach (var alias in aliasAttr.Alias)
                {
                    RegisterServerCommand(alias, action);
                }
            }

            _serverCommands.Add(new Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>(cmdAttr, aliasAttr, action));

            Logger.Debug($"Registering [ServerCommand]: {cmdAttr.Command} on method: {method.Name}");
        }

        internal void RegisterInternalClientCommand(ClientCommandAttribute commandAttr, CommandAliasAttribute aliasAttr, object classObj, MethodInfo method)
        {
            var action = Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
            _clientCommands.Add(new Command(commandAttr, aliasAttr, action));

            Logger.Debug($"Registering [Command]: {commandAttr.Command} on method: {method.Name} with alias: {(aliasAttr != null ? $"[{string.Join(", ", aliasAttr.Alias)}]" : "empty")}, params [{string.Join(", ", method.GetParameters().Select(x => x.ParameterType))}]");
        }

        internal void ExecuteClientCommand(Client client, string commandName, List<object> args)
        {
            var command = GetCommand(commandName);

            if (command != null)
            {
                // Need to cast args to the paramaters type of command.Action
                var newArgs = new List<object> { client };

                for (int i = 0; i < args.Count; i++)
                {
                    // Need to skip client arg "Skip(1)" for convert args correctly
                    newArgs.Add(Convert.ChangeType(args[i], command.Action.Method.GetParameters().Skip(1).ToList()[i].ParameterType));
                }

                command.Action.DynamicInvoke(newArgs.ToArray());
            }
        }

        public IEnumerable<Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>> GetServerCommands() => _serverCommands.AsEnumerable();
        public Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate> GetServerCommand(string command) => _serverCommands.Find(x => x.Item1.Command == command);
        public IEnumerable<Command> GetCommands() => _clientCommands.AsEnumerable();
        public Command GetCommand(string command) => _clientCommands.Find(x => x.Attribute.Command == command);
    }
}
