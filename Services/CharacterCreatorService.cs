using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;

namespace Average.Server.Services
{
    internal class CharacterCreatorService : IService
    {
        private readonly EventManager _eventManager;

        public CharacterCreatorService(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        internal void StartCreator(Client client)
        {
            _eventManager.EmitClient(client, "character-creator:start_creator");
        }
    }
}
