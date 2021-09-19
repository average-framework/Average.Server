using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Net;

namespace Average.Server.Services
{
    internal class DiscordService : IService
    {
        private readonly RequestManager _requestManager;
        private const string Token = "ODgyODAyMzc4Njk5NjA0MDA5.YTAryA.Zbg1Cz5v36KknoeF55BnNwayXys";
        private const string BaseUrl = "https://discord.com/api/v8";
        private const long GuildId = 868112465081294890;

        private Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "Authorization", $"Bot {Token}" }
        };

        public DiscordService(RequestManager request)
        {
            _requestManager = request;

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public void Get(Player player)
        {
            Logger.Error("Get discord id: " + player.Name + ", " + player.DiscordId());
            try
            {
                _requestManager.Http(string.Join("/", BaseUrl, $"/guilds/{GuildId}/members/{player.DiscordId()}"), headers: _headers);
            }
            catch
            {
                Logger.Error("error");
            }
        }
    }
}
