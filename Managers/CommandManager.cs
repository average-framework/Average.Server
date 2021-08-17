using CitizenFX.Core.Native;
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
        Logger logger;

        List<ServerCommandAttribute> Commands { get; }

        public CommandManager(Logger logger)
        {
            this.logger = logger;
            Commands = new List<ServerCommandAttribute>();
        }

        public void RegisterCommand(ServerCommandAttribute commandAttr, MethodInfo method, object classObj)
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

                    Commands.Add(commandAttr);
                    logger.Debug($"Registering [Command] attribute: {commandAttr.Command} on method: {method.Name}");
                }
                else
                {
                    logger.Warn($"Unable to register [Command] attribute: {commandAttr.Command}, arguments does not match with the framework command format.");
                }
            }
            else if (methodParams.Count() == 0)
            {
                // empty args
                API.RegisterCommand(commandAttr.Command, new Action(() =>
                {
                    method.Invoke(classObj, new object[] { });
                }), false);

                Commands.Add(commandAttr);
                logger.Debug($"Registering [Command] attribute: {commandAttr.Command} on method: {method.Name}");
            }
            else
            {
                logger.Warn($"Unable to register [Command] attribute: {commandAttr.Command}, arguments does not match with the framework command format.");
            }
        }

        public IEnumerable<ServerCommandAttribute> GetCommands() => Commands.AsEnumerable();

        public ServerCommandAttribute GetCommand(string command) => Commands.Find(x => x.Command == command);

        public int Count() => Commands.Count();
    }
}
