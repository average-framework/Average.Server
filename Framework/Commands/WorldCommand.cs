using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Enums;
using System;

namespace Average.Server.Framework.Commands
{
    internal class WorldCommand : ICommand
    {
        private readonly WorldService _worldService;

        public WorldCommand(WorldService worldService)
        {
            _worldService = worldService;
        }

        [ClientCommand("world:set_time")]
        private void SetTime(Client client, int hours, int minutes, int seconds, int transitionTimeInSeconds)
        {
            _worldService.SetTime(new TimeSpan(hours, minutes, seconds), transitionTimeInSeconds * 1000);
        }

        [ClientCommand("world:set_weather")]
        private void SetWeather(Client client, string weatherName, float transitionTimeInSeconds)
        {
            if (Enum.TryParse(weatherName, true, out Weather weather))
            {
                _worldService.SetWeather(weather, transitionTimeInSeconds);
            }
            else
            {
                Logger.Error("[World] This weather does not exists.");
            }
        }

        [ClientCommand("world:set_next_weather")]
        private void SetNextWeather(Client client, float transitionTime)
        {
            _worldService.SetNextWeather(transitionTime);
        }
    }
}
