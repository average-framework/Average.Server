using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using static Average.Server.Services.RpcService;

namespace Average.Server.Handlers
{
    internal class CommandHandler : IHandler
    {
        private readonly EventService _eventManager;
        private readonly CommandService _commandManager;
        private readonly ClientService _clientService;
        private readonly RpcService _rpc;

        public CommandHandler(EventService eventManager, CommandService commandManager, ClientService clientService, RpcService rpc)
        {
            _eventManager = eventManager;
            _commandManager = commandManager;
            _clientService = clientService;
            _rpc = rpc;

            _rpc.OnRequest<string, List<object>>("command:execute", OnClientExecuteCommand);
        }

        internal void OnClientInitialized(Client client)
        {
            var commands = _commandManager.GetCommands().ToList();
            var newCommands = new List<CommandModel>();

            commands.ForEach(x => newCommands.Add(new CommandModel(x.Attribute.Command, x.Alias != null ? x.Alias.Alias : new string[] { })));
            _eventManager.EmitClient(client, "command:register_commands", newCommands.ToJson());
        }

        private void OnClientExecuteCommand(Client client, RpcCallback callback, string commandName, List<object> args)
        {
            try
            {
                _commandManager.ExecuteClientCommand(client, commandName, args);

                // Send empty response
                callback(true, "");
            }
            catch
            {
                var command = _commandManager.GetCommand(commandName);
                var usage = "";
                command.Action.Method.GetParameters().Skip(1).ToList().ForEach(x => usage += $"<[{x.ParameterType.Name}] {x.Name}> ");

                // Need to send response for this command error
                callback(false, $"Invalid command usage: {commandName} {usage.TrimEnd(' ')}.");
            }
        }
    }
}
