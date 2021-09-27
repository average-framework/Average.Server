using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using Average.Shared.Enums;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class WorldService : IService
    {
        private readonly ThreadService _threadManager;
        private readonly EventService _eventManager;
        private readonly WorldRepository _repository;
        private readonly RpcService _rpcService;

        private int _minTransitionTime;
        private int _maxTransitionTime;
        private int _minTimeBetweenWeatherChanging;
        private int _maxTimeBetweenWeatherChanging;
        private JObject _baseConfig;

        public WorldData World { get; private set; }

        public WorldService(RpcService rpcService, WorldRepository repository, ThreadService threadManager, EventService eventManager)
        {
            _rpcService = rpcService;
            _threadManager = threadManager;
            _eventManager = eventManager;
            _repository = repository;

            _baseConfig = Configuration.ParseToObject("configs/world.json");

            _minTransitionTime = _baseConfig["MinTransitionTime"].Convert<int>();
            _maxTransitionTime = _baseConfig["MaxTransitionTime"].Convert<int>();
            _minTimeBetweenWeatherChanging = _baseConfig["MinTimeBetweenWeatherChanging"].Convert<int>();
            _maxTimeBetweenWeatherChanging = _baseConfig["MaxTimeBetweenWeatherChanging"].Convert<int>();

            Task.Factory.StartNew(async () =>
            {
                if (!Exists(0))
                {
                    var world = new WorldData
                    {
                        WorldId = 0,
                        Weather = Weather.Sunny,
                        Time = new TimeSpan(8, 0, 0)
                    };

                    World = world;
                    await Add(world);
                }
                else
                {
                    // Get default world
                    World = Get(0);
                }

                _threadManager.StartThread(TimeUpdate);
                _threadManager.StartThread(WeatherUpdate);
            });
        }

        private async Task TimeUpdate()
        {
            World.Time += TimeSpan.FromSeconds(120);
            Update(World);

            _eventManager.EmitClients("world:set_time", World.Time.Hours, World.Time.Minutes, World.Time.Seconds);
            await BaseScript.Delay(10000);
        }

        private async Task WeatherUpdate()
        {
            var rndTimeToWait = new Random(Environment.TickCount + 1).Next(_minTimeBetweenWeatherChanging, _maxTimeBetweenWeatherChanging);

            await BaseScript.Delay(rndTimeToWait);

            var rndTransitionTime = (float)new Random(Environment.TickCount + 2).Next(_minTransitionTime, _maxTransitionTime);
            var nextWeather = GetNextWeather();

            Logger.Info($"Changing weather from {World.Weather} to {nextWeather}, waiting time: {rndTimeToWait}, transition time: {rndTransitionTime} seconds.");

            World.Weather = nextWeather;

            Update(World);

            _eventManager.EmitClients("world:set_weather", nextWeather, rndTransitionTime);
        }

        private Weather GetNextWeather()
        {
            switch (World.Weather)
            {
                case Weather.Sunny:
                    {
                        var list = new List<Weather> { Weather.Clearing, Weather.Overcast, Weather.OvercastDark, Weather.Rain, Weather.Thunderstorm };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Rain:
                    {
                        var list = new List<Weather> { Weather.Overcast, Weather.OvercastDark, Weather.Thunder, Weather.Shower, Weather.Sleet };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Thunderstorm:
                    {
                        var list = new List<Weather> { Weather.Overcast, Weather.OvercastDark, Weather.Thunder, Weather.Shower, Weather.Sleet, Weather.Hurricane };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Thunder:
                    {
                        var list = new List<Weather> { Weather.Overcast, Weather.Shower, Weather.Rain, Weather.Clearing, Weather.Sunny, Weather.Sleet, Weather.Hurricane };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Clearing:
                    {
                        var list = new List<Weather> { Weather.Sunny, Weather.Overcast };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Overcast:
                    {
                        var list = new List<Weather> { Weather.Sunny, Weather.OvercastDark, Weather.Rain, Weather.Clearing, Weather.Shower, Weather.Sleet, Weather.Drizzle };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.OvercastDark:
                    {
                        var list = new List<Weather> { Weather.Overcast, Weather.Rain, Weather.Thunderstorm, Weather.Hail, Weather.Drizzle };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Misty:
                    {
                        var list = new List<Weather> { Weather.Fog, Weather.Clearing, Weather.Sunny };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Fog:
                    {
                        var list = new List<Weather> { Weather.Misty, Weather.Clearing };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Hurricane:
                    {
                        var list = new List<Weather> { Weather.Clearing, Weather.Sleet, Weather.Shower };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                case Weather.Sleet:
                case Weather.Shower:
                    {
                        var list = new List<Weather> { Weather.Clearing, Weather.Sunny, Weather.Overcast };
                        var rnd = new Random().Next(0, list.Count - 1);
                        return list[rnd];
                    }
                default:
                    return Weather.Clearing;
            }
        }

        internal void OnSetWorldForClient(/*Client client*/)
        {
            _rpcService.GlobalNativeCall(0x59174F1AFE095B5A, (uint)World.Weather, true, true, true, 0f, false);
            _rpcService.GlobalNativeCall(0x669E223E64B1903C, World.Time.Hours, World.Time.Minutes, World.Time.Seconds, 5000, true);
            //_eventManager.EmitClient(client, "world:set_world", (uint)World.Weather, World.Time.Hours, World.Time.Minutes, World.Time.Seconds);
        }

        internal void SetTime(TimeSpan time, int transitionTime = 0)
        {
            Logger.Debug($"[World] Set time from {World.Time} to {time}");

            if (time.Hours == 0)
            {
                transitionTime = 0;
            }

            Logger.Debug("Transition time: " + transitionTime);

            _rpcService.GlobalNativeCall(0x669E223E64B1903C, time.Hours, time.Minutes, time.Seconds, transitionTime, true);
            //NetworkClockTimeOverride(time.Hours, time.Minutes, time.Seconds, transitionTime, true);

            //_eventManager.EmitClients("world:set_time", time.Hours, time.Minutes, time.Seconds, transitionTime);

            World.Time = time;
            Update(World);
        }

        internal void SetWeather(Weather weather, float transitionTime)
        {
            Logger.Debug($"[World] Set weather from {World.Weather} to {weather} in {transitionTime} second(s).");

            _rpcService.GlobalNativeCall(0xD74ACDF7DB8114AF, false);
            _rpcService.GlobalNativeCall(0x59174F1AFE095B5A, (uint)weather, true, true, true, transitionTime, false);

            //_eventManager.EmitClients("world:set_weather", (uint)weather, transitionTime);

            World.Weather = weather;
            Update(World);
        }

        internal void SetNextWeather(float transitionTime)
        {
            var nextWeather = GetNextWeather();
            Logger.Debug($"[World] Set next weather from {World.Weather} to {nextWeather} in {transitionTime} second(s).");

            _rpcService.GlobalNativeCall(0x59174F1AFE095B5A, (uint)nextWeather, true, true, true, transitionTime, false);
            //_eventManager.EmitClients("world:set_weather", (uint)nextWeather, transitionTime);

            World.Weather = nextWeather;
            Update(World);
        }

        #region Repository

        public async Task Add(WorldData data) => await _repository.Add(data);
        public ICollection<WorldData> GetAll() => _repository.GetAll();
        public WorldData Get(long worldId, bool includeChild = false) => _repository.GetAll(includeChild).Find(x => x.WorldId == worldId);
        public async void Update(WorldData data) => await _repository.Update(data);
        public async void Delete(WorldData data) => await _repository.Delete(data.Id);
        public bool Exists(WorldData data) => Get(data.WorldId) != null;
        public bool Exists(long worldId) => Get(worldId) != null;

        #endregion
    }
}
