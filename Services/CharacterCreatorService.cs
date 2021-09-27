using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;

namespace Average.Server.Services
{
    internal class CharacterCreatorService : IService
    {
        private readonly EventService _eventManager;

        public CharacterCreatorService(EventService eventManager)
        {
            _eventManager = eventManager;
        }

        internal void StartCreator(Client client)
        {
            _eventManager.EmitClient(client, "character-creator:start_creator");
        }
    }
}
