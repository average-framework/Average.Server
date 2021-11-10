using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Framework.Services;
using System;

namespace Average.Server.Scripts
{
    internal class CharacterCreatorScript : IScript
    {
        private readonly EventService _eventManager;

        public CharacterCreatorScript(EventService eventManager)
        {
            _eventManager = eventManager;

            Logger.Write("CharacterCreatorService", "Initialized successfully");
        }

        internal void StartCreator(Client client)
        {
            _eventManager.EmitClient(client, "character-creator:start_creator");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
