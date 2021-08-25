using CitizenFX.Core.Native;
using SDK.Server;
using SDK.Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class CommandManager : ICommandManager
    {
        private List<ServerCommandAttribute> _commands = new List<ServerCommandAttribute>();

        private void RegisterCommandInternal(string command, object classObj, MethodInfo method,
            ServerCommandAttribute commandAttr)
        {
            var methodParams = method.GetParameters();

            API.RegisterCommand(command, new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var newArgs = new List<object>();

                if (args.Count == methodParams.Length)
                {
                    try
                    {
                        args.ForEach(x =>
                            newArgs.Add(Convert.ChangeType(x,
                                methodParams[args.FindIndex(p => p == x)].ParameterType)));
                        method.Invoke(classObj, newArgs.ToArray());
                    }
                    catch
                    {
                        Log.Error($"Unable to convert command arguments.");
                    }
                }
                else
                {
                    var usage = "";
                    methodParams.ToList().ForEach(x => usage += $"<[{x.ParameterType.Name}] {x.Name}> ");
                    Log.Error($"Invalid command usage: {command} {usage}.");
                }
            }), false);

            Log.Debug($"Registering [Command] attribute: {command} on method: {method.Name}");
        }

        public void RegisterCommand(ServerCommandAttribute commandAttr, MethodInfo method, object classObj)
        {
            if (commandAttr == null) return;

            RegisterCommandInternal(commandAttr.Command, classObj, method, commandAttr);
            _commands.Add(commandAttr);
        }

        public IEnumerable<ServerCommandAttribute> GetCommands() => _commands.AsEnumerable();

        public ServerCommandAttribute GetCommand(string command) => _commands.Find(x => x.Command == command);

        public int Count() => _commands.Count();
    }
}
