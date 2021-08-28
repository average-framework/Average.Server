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
    public class CommandManager : InternalPlugin, ICommandManager
    {
        private static List<ServerCommandAttribute> _commands = new List<ServerCommandAttribute>();

        internal static void RegisterInternalCommand(ServerCommandAttribute cmdAttr, object classObj, MethodInfo method)
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
                        Log.Error($"Unable to convert command arguments.");
                    }
                }
                else
                {
                    var usage = "";
                    methodParams.ToList().ForEach(x => usage += $"<[{x.ParameterType.Name}] {x.Name}> ");
                    Log.Error($"Invalid command usage: {cmdAttr.Command} {usage}.");
                }
            }), false);
            
            Log.Debug($"Registering [Command] attribute: {cmdAttr.Command} on method: {method.Name}");
        }

        public IEnumerable<ServerCommandAttribute> GetCommands() => _commands.AsEnumerable();

        public ServerCommandAttribute GetCommand(string command) => _commands.Find(x => x.Command == command);

        public int Count() => _commands.Count();
    }
}
