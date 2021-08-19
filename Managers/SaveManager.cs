using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Average.Server.Managers
{
    public class SaveManager : ISaveManager
    {
        Logger logger;
        EventManager eventManager;
        PlayerList players;

        int saveInterval;
        int deferredInterval;
        int cancelSaveInterval;

        JObject baseConfig;

        List<ISaveable> tasks = new List<ISaveable>();
        Dictionary<string, bool> playersSavedState = new Dictionary<string, bool>();

        public SaveManager(Logger logger, ThreadManager thread, EventManager eventManager, EventHandlerDictionary eventHandlers, PlayerList players)
        {
            this.logger = logger;
            this.eventManager = eventManager;
            this.players = players;

            baseConfig = SDK.Server.Configuration.Parse("config.json");

            saveInterval = (int)baseConfig["Save"]["SaveInterval"];
            deferredInterval = (int)baseConfig["Save"]["DeferredInterval"];
            cancelSaveInterval = (int)baseConfig["Save"]["CancelSaveInterval"];

            thread.StartThread(Update);
            
            #region Command

            RegisterCommand("save.all", new Action(SaveAllCommand), false);

            #endregion

            #region Event

            eventHandlers["Save.All"] += new Action<int>(SaveAllEvent);

            #endregion
        }

        protected async Task Update()
        {
            await BaseScript.Delay(saveInterval);

            logger.Info("[Save] Saving server data..");

            await SaveAll();

            logger.Info($"[Save] Server data saved successfully.");
            logger.Info("[Save] Saving player data..");

            playersSavedState.Clear();

            for (int i = 0; i < players.Count(); i++)
                playersSavedState.Add(players.ElementAt(i).Identifiers["license"], false);

            eventManager.EmitClients("Save.All");

            var time = GetGameTimer();

            while (!playersSavedState.Values.All(x => x) && (GetGameTimer() - time) < cancelSaveInterval) await BaseScript.Delay(0);

            var seconds = saveInterval / 1000;
            var minutes = seconds / 60;

            logger.Info($"[Save] Players data saved successfully.");
            logger.Info($"[Save] Next save in {seconds} seconds ({(minutes >= 1 ? $"{minutes} minutes" : seconds.ToString())})");
        }

        public async Task SaveAll()
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                await tasks[i].SaveAll();
                await BaseScript.Delay(deferredInterval);
            }
        }

        public void AddInQueue(ISaveable saveable) => tasks.Add(saveable);

        public void DeleteFromQueue(ISaveable saveable) => tasks.Remove(saveable);

        #region Command

        protected async void SaveAllCommand() => await SaveAll();

        #endregion

        #region Event

        protected async void SaveAllEvent(int player)
        {
            var p = players[player];
            var license = p.Identifiers["license"];

            if (playersSavedState.ContainsKey(license))
            {
                playersSavedState[license] = true;

                for (int i = 0; i < tasks.Count; i++)
                    await tasks[i].Save(p);

                logger.Debug($"[Save] Saving cache for player: {license}. [{playersSavedState[license]}]");
            }
        }

        #endregion
    }
}
