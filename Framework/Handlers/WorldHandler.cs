using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Framework.Services;
using Average.Shared.Enums;
using System;

namespace Average.Server.Framework.Handlers
{
    internal class WorldHandler : IHandler
    {
        private readonly WorldService _worldService;

        public WorldHandler(WorldService worldService)
        {
            _worldService = worldService;
        }

        [ServerEvent("world:set_time")]
        private void OnSetTime(Client client, int hours, int minutes, int seconds, int transitionTime)
        {
            _worldService.SetTime(new TimeSpan(hours, minutes, seconds), transitionTime);
        }

        [ServerEvent("world:set_weather")]
        private void OnSetWeather(Client client, uint weather, float transitionTime)
        {
            _worldService.SetWeather((Weather)weather, transitionTime);
        }

        [ServerEvent("world:set_next_weather")]
        private void OnSetNextWeather(Client client, float transitionTime)
        {
            _worldService.SetNextWeather(transitionTime);
        }
    }
}
