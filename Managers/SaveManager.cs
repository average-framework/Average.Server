using CitizenFX.Core;
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
        private readonly int _saveInterval;
        private readonly int _deferredInterval;
        private readonly int _cancelSaveTimeout;

        private List<ISaveable> _tasks = new List<ISaveable>();
        private Dictionary<string, bool> _playersSavedState = new Dictionary<string, bool>();

        public SaveManager()
        {
            var baseConfig = SDK.Server.Configuration.Parse("config.json");

            _saveInterval = (int)baseConfig["Save"]["SaveInterval"];
            _deferredInterval = (int)baseConfig["Save"]["DeferredInterval"];
            _cancelSaveTimeout = (int)baseConfig["Save"]["CancelSaveTimeout"];

            Main.threadManager.StartThread(Update);
            
            #region Command

            RegisterCommand("save.all", new Action(SaveAllCommand), false);

            #endregion

            #region Event

            Main.eventHandlers["Save.All"] += new Action<int>(SaveAllEvent);

            #endregion
        }

        private async Task Update()
        {
            await BaseScript.Delay(_saveInterval);

            Log.Info("[Save] Saving server data..");

            await SaveAll();

            Log.Info($"[Save] Server data saved successfully.");
            Log.Info("[Save] Saving player data..");

            _playersSavedState.Clear();

            for (int i = 0; i < Main.players.Count(); i++)
                _playersSavedState.Add(Main.players.ElementAt(i).Identifiers["license"], false);

            Main.eventManager.EmitClients("Save.All");

            var time = GetGameTimer();

            while (!_playersSavedState.Values.All(x => x) && (GetGameTimer() - time) < _cancelSaveTimeout) await BaseScript.Delay(0);

            var seconds = _saveInterval / 1000;
            var minutes = seconds / 60;

            Log.Info($"[Save] Players data saved successfully.");
            Log.Info($"[Save] Next save in {seconds} seconds ({(minutes >= 1 ? $"{minutes} minutes" : seconds.ToString())})");
        }

        public async Task SaveAll()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                await _tasks[i].SaveAll();
                await BaseScript.Delay(_deferredInterval);
            }
        }

        public void AddInQueue(ISaveable saveable) => _tasks.Add(saveable);

        public void DeleteFromQueue(ISaveable saveable) => _tasks.Remove(saveable);

        private async void SaveAllCommand() => await SaveAll();

        #region Event

        private async void SaveAllEvent(int player)
        {
            var p = Main.players[player];
            var license = p.Identifiers["license"];

            if (_playersSavedState.ContainsKey(license))
            {
                _playersSavedState[license] = true;

                for (int i = 0; i < _tasks.Count; i++)
                    await _tasks[i].Save(p);

                Log.Debug($"[Save] Saving cache for player: {license}. [{_playersSavedState[license]}]");
            }
        }

        #endregion
    }
}
