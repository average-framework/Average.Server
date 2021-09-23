using Average.Server.DataModels;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;

namespace Average.Server.Services
{
    internal class SpawnService : IService
    {
        private readonly CharacterService _characterService;
        private readonly EventManager _eventManager;

        public SpawnService(CharacterService characterService, EventManager eventManager)
        {
            _characterService = characterService;
            _eventManager = eventManager;
        }

        internal void OnCreateCharacter(Client client)
        {
            _eventManager.EmitClient(client, "spawn:create_character");
        }

        internal void OnSpawnCharacter(Client client, CharacterData character)
        {
            _eventManager.EmitClient(client, "spawn:spawn_character", character.ToJson());
        }

        internal void OnRespawnCharacter(Client client)
        {
            _characterService.OnRespawnPed(client);
        }
    }
}
