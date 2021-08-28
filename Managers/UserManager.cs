using CitizenFX.Core;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Threading.Tasks;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class UserManager : InternalPlugin, IUserManager
    {
        public override void OnInitialized()
        {
            #region Rpc

            Rpc.Event("User.GetUser").On(async (message, callback) =>
            {
                Log.Debug("Getted user");
                var data = await GetUser(Players[message.Target]);
                callback(data);
            });

            #endregion
        }

        public async Task<UserData> GetUser(Player player)
        {
            var data = await Sql.GetAllAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);
            return data[0];
        }

        public async Task<bool> Exist(Player player) => await Sql.ExistsAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);

        public async void CreateAccount(Player player) => await Sql.InsertAsync("users", new UserData
        {
            RockstarId = player.Identifiers["license"],
            Name = player.Name,
            IsBanned = 0,
            IsWhitelisted = 0,
            IsConnected = 0,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"),
            LastConnection = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"),
            Permission = new PermissionData("player", 0)
        });

        public void UpdateLastConnectionTime(UserData data)
        {
            data.LastConnection = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            Save(data);
        }

        public void UpdateConnectionState(UserData data, bool isConnected)
        {
            data.IsConnected = isConnected ? 1 : 0;
            Save(data);
        }

        public async void Save(UserData data) => await Sql.InsertOrUpdateAsync("users", data);

        public async Task<DateTime> GetLastConnectionTime(Player player)
        {
            var result = await GetUser(player);
            return DateTime.Parse(result.LastConnection).ToLocalTime();
        }
    }
}
