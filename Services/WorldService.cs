using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using Average.Shared.Enums;
using Average.Shared.Events;
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
        private readonly ClientService _clientService;

        private readonly int _minTransitionTime;
        private readonly int _maxTransitionTime;
        private readonly int _minTimeBetweenWeatherChanging;
        private readonly int _maxTimeBetweenWeatherChanging;
        private readonly JObject _baseConfig;

        public WorldData World { get; private set; }

        public event EventHandler<WorldTimeEventArgs> TimeChanged;
        public event EventHandler<WorldWeatherEventArgs> WeatherChanged;

        public WorldService(ClientService clientService, RpcService rpcService, WorldRepository repository, ThreadService threadManager, EventService eventManager)
        {
            _clientService = clientService;
            _rpcService = rpcService;
            _threadManager = threadManager;
            _eventManager = eventManager;
            _repository = repository;

            _baseConfig = Configuration.ParseToObject("configs/world.json");

            _minTransitionTime = _baseConfig["MinTransitionTime"].Convert<int>();
            _maxTransitionTime = _baseConfig["MaxTransitionTime"].Convert<int>();
            _minTimeBetweenWeatherChanging = _baseConfig["MinTimeBetweenWeatherChanging"].Convert<int>();
            _maxTimeBetweenWeatherChanging = _baseConfig["MaxTimeBetweenWeatherChanging"].Convert<int>();

            Task.Run(async () =>
            {
                if (!await Exists(0))
                {
                    var world = new WorldData
                    {
                        WorldId = 0,
                        Weather = Weather.Sunny,
                        Time = new TimeSpan(8, 0, 0)
                    };

                    World = world;
                    await Create(world);
                }
                else
                {
                    // Get default world
                    World = await Get(0);
                }

                _threadManager.StartThread(TimeUpdate);
                _threadManager.StartThread(WeatherUpdate);

                Logger.Write("WorldService", "Initialized successfully");
            });
        }

        private async Task TimeUpdate()
        {
            World.Time += TimeSpan.FromSeconds(120);

            SetTime(World.Time, 10000);
            Update(World);

            await BaseScript.Delay(10000);
        }

        private async Task WeatherUpdate()
        {
            var rndTimeToWait = new Random(Environment.TickCount + 1).Next(_minTimeBetweenWeatherChanging, _maxTimeBetweenWeatherChanging);

            await BaseScript.Delay(rndTimeToWait);

            var rndTransitionTime = (float)new Random(Environment.TickCount + 2).Next(_minTransitionTime, _maxTransitionTime);
            var nextWeather = GetNextWeather();

            Logger.Info($"Changing weather from {World.Weather} to {nextWeather}, waiting time: {rndTimeToWait}, transition time: {rndTransitionTime} seconds.");

            SetWeather(nextWeather, rndTransitionTime);

            await Update(World);
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

        internal void OnClientInitialized(Client client)
        {
            _rpcService.NativeCall(client, 0x59174F1AFE095B5A, (uint)World.Weather, true, true, true, 0f, false);
            _rpcService.NativeCall(client, 0x669E223E64B1903C, World.Time.Hours, World.Time.Minutes, World.Time.Seconds, 5000, true);
        }

        internal async void SetTime(TimeSpan time, int transitionTime = 0)
        {
            if (time.Hours == 0)
            {
                transitionTime = 0;
            }

            _eventManager.EmitClients("world:set_time", time.Hours, time.Minutes, time.Seconds, transitionTime);

            //Logger.Debug($"[World] Set time from {World.Time} to {time} in {transitionTime / 1000} second(s).");

            World.Time = time;
            TimeChanged?.Invoke(this, new WorldTimeEventArgs(World.Time, transitionTime));

            await Update(World);
        }

        internal async void SetWeather(Weather weather, float transitionTime)
        {
            _eventManager.EmitClients("world:set_weather", (uint)weather, transitionTime);

            //Logger.Debug($"[World] Set weather from {World.Weather} to {weather} in {transitionTime} second(s).");

            World.Weather = weather;
            WeatherChanged?.Invoke(this, new WorldWeatherEventArgs(World.Weather, transitionTime));

            await Update(World);
        }

        internal async void SetNextWeather(float transitionTime)
        {
            var nextWeather = GetNextWeather();

            _eventManager.EmitClients("world:set_weather", (uint)nextWeather, transitionTime);

            World.Weather = nextWeather;
            await Update(World);

            Logger.Debug($"[World] Set next weather from {World.Weather} to {nextWeather} in {transitionTime} second(s).");
        }

        #region Repository

        public async Task<bool> Create(WorldData data) => await _repository.AddAsync(data);
        public async Task<List<WorldData>> GetAll() => await _repository.GetAllAsync();
        public async Task<WorldData> Get(long worldId) => await _repository.GetAsync(x => x.WorldId == worldId);
        public async Task<bool> Update(WorldData data) => await _repository.ReplaceOneAsync(x => x.Id, data.Id, data);
        public async Task<bool> Delete(WorldData data) => await _repository.DeleteOneAsync(x => x.Id == data.Id);
        public async Task<bool> Delete(long worldId) => await _repository.DeleteOneAsync(x => x.WorldId == worldId);
        public async Task<bool> Exists(WorldData data) => await _repository.ExistsAsync(x => x.Id == data.Id);
        public async Task<bool> Exists(long worldId) => await _repository.ExistsAsync(x => x.WorldId == worldId);

        #endregion
    }
}
