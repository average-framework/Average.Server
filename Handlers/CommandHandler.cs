﻿using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Handlers
{
    internal class CommandHandler : IHandler
    {
        private readonly EventManager _eventManager;
        private readonly CommandManager _commandManager;

        public CommandHandler(EventManager eventManager, CommandManager commandManager)
        {
            _eventManager = eventManager;
            _commandManager = commandManager;
        }

        internal void OnClientInitialized(Client client)
        {
            var commands = _commandManager.GetCommands().ToList();
            var newCommands = new List<object>();

            commands.ForEach(x => newCommands.Add(new
            {
                Command = x.Attribute.Command,
                Alias = x.Alias != null ? x.Alias.Alias : new string[] { }
            }));

            _eventManager.EmitClient(client, "command:register_commands", newCommands.ToJson());
        }
    }
}
