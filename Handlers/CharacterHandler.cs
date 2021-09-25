using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.DataModels;
using Newtonsoft.Json;
using System;

namespace Average.Server.Handlers
{
    internal class CharacterHandler : IHandler
    {
        private readonly CharacterService _characterService;
        private readonly UserService _userService;

        public CharacterHandler(CharacterService characterService, UserService userService)
        {
            _characterService = characterService;
            _userService = userService;
        }

        [ServerEvent("character:create_character")]
        private async void OnCreateCharacter(Client client, string json)
        {
            var characterData = json.Convert<CharacterData>();
            characterData.License = client.License;

            var userData = await _userService.Get(client);
            characterData.UserId = userData.Id;

            _characterService.Create(userData, characterData);
        }
    }
}
