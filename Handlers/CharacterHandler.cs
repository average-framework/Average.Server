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

        public CharacterHandler(CharacterService characterService)
        {
            _characterService = characterService;
        }

        [ServerEvent("character:create_character")]
        private void OnCreateCharacter(Client client, string json)
        {
            var characterData = json.Convert<CharacterData>();
            characterData.License = client.License;

            _characterService.Create(characterData);
        }
    }
}
