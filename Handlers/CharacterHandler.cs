using Average.Server.Framework.Attributes;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.DataModels;

namespace Average.Server.Handlers
{
    internal class CharacterHandler : IHandler
    {
        private readonly CharacterService _characterService;
        private readonly EventService _eventService;

        public CharacterHandler(CharacterService characterService, EventService eventService)
        {
            _characterService = characterService;
            _eventService = eventService;
        }

        [ServerEvent("character:create_character")]
        private void OnCreateCharacter(Client client, string json)
        {
            var characterData = json.Convert<CharacterData>();
            characterData.License = client.License;

            _characterService.Create(characterData);
            _eventService.Emit("character:character_created", client.License, characterData.CharacterId);
        }
    }
}
