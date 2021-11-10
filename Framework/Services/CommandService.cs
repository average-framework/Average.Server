﻿using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using CitizenFX.Core.Native;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Framework.Services
{
    internal class CommandService : IService
    {
        private readonly IContainer _container;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly List<Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>> _serverCommands = new List<Tuple<ServerCommandAttribute, CommandAliasAttribute, Delegate>>();

        public CommandService(IContainer container)
        {
            _container = container;

            Logger.Write("CommandService", "Initialized successfully");
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

            Logger.Write("Command", $"Registering [ServerCommand]: %{commandName}% on method: {action.Method.Name}.", new Logger.TextColor(foreground: ConsoleColor.DarkYellow));
        }

        private void RegisterInternalServerCommand(ServerCommandAttribute cmdAttr, CommandAliasAttribute aliasAttr, object classObj, MethodInfo method)
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
        }
    }
}