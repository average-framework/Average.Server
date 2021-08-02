using Average.Plugins;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Commands;
using SDK.Server.Diagnostics;
using SDK.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Average.Managers
{
    public class CommandManager : ICommandManager
    {
        Logger Logger { get; }
        List<Tuple<ServerCommandAttribute, ClientCommandAliasAttribute>> Commands { get; }

        public CommandManager(Logger logger)
        {
            Logger = logger;
            Commands = new List<Tuple<ServerCommandAttribute, ClientCommandAliasAttribute>>();
        }

        public void RegisterCommand(ServerCommandAttribute commandAttr, ClientCommandAliasAttribute aliasAttr, MethodInfo method, object classObj)
        {
            if (commandAttr == null) return;

            var methodParams = method.GetParameters();

            if (methodParams.Count() == 3)
            {
                if (methodParams[0].ParameterType == typeof(int) && methodParams[1].ParameterType == typeof(List<object>) && methodParams[2].ParameterType == typeof(string))
                {
                    // source, args, raw
                    API.RegisterCommand(commandAttr.Command, new Action<int, List<object>, string>((source, args, raw) =>
                    {
                        method.Invoke(classObj, new object[] { source, args, raw });
                    }), false);

                    if (aliasAttr != null)
                    {
                        foreach (var alias in aliasAttr.Alias)
                        {
                            API.RegisterCommand(alias, new Action<int, List<object>, string>((source, args, raw) =>
                            {
                                method.Invoke(classObj, new object[] { source, args, raw });
                            }), false);
                        }

                        Logger.Debug($"Registering {aliasAttr.Alias.Length} alias for command: {commandAttr.Command} [{string.Join(", ", aliasAttr.Alias)}]");
                    }

                    Commands.Add(new Tuple<ServerCommandAttribute, ClientCommandAliasAttribute>(commandAttr, aliasAttr));
                    Logger.Debug($"Regisering [Command] attribute: {commandAttr.Command} on method: {method.Name}");
                }
                else
                {
                    Logger.Warn($"Unable to register [Command] attribute: {commandAttr.Command}, arguments does not match with the framework command format.");
                }
            }
            else if (methodParams.Count() == 0)
            {
                // empty args
                API.RegisterCommand(commandAttr.Command, new Action(() =>
                {
                    method.Invoke(classObj, new object[] { });
                }), false);

                if (aliasAttr != null)
                {
                    foreach (var alias in aliasAttr.Alias)
                    {
                        API.RegisterCommand(alias, new Action(() =>
                        {
                            method.Invoke(classObj, new object[] { });
                        }), false);
                    }

                    Commands.Add(new Tuple<ServerCommandAttribute, ClientCommandAliasAttribute>(commandAttr, aliasAttr));
                    Logger.Debug($"Registering {aliasAttr.Alias.Length} alias for command: {commandAttr.Command} [{string.Join(", ", aliasAttr.Alias)}]");
                }

                Logger.Debug($"Regisering [Command] attribute: {commandAttr.Command} on method: {method.Name}");
            }
            else
            {
                Logger.Warn($"Unable to register [Command] attribute: {commandAttr.Command}, arguments does not match with the framework command format.");
            }
        }

        public IEnumerable<Tuple<ServerCommandAttribute, ClientCommandAliasAttribute>> GetCommands() => Commands.AsEnumerable();

        public ServerCommandAttribute GetCommand(string command) => Commands.Find(x => x.Item1.Command == command).Item1;

        public int Count() => Commands.Count();
    }
}
