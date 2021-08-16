using CitizenFX.Core;
using SDK.Server;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class UserManager : IUserManager
    {
        Framework framework;

        public UserManager(Framework framework)
        {
            this.framework = framework;

            Task.Factory.StartNew(async () =>
            {
                await framework.IsReadyAsync();

                //framework.Logger.Trace("Get user 0");
                framework.Rpc.Event("User.GetUser").On(async (message, callback) =>
                {
                    framework.Logger.Trace("Get user 1: " + framework.Players[message.Target].Name);
                    var data = await GetUser(framework.Players[message.Target]);
                    callback(data);
                });
            });
        }

        public async Task<UserData> GetUser(Player player)
        {
            var data = await framework.Sql.GetAllAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);
            return data[0];
        }

        public async Task<bool> Exist(Player player) => await framework.Sql.ExistsAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);

        public async void CreateAccount(Player player) => await framework.Sql.InsertAsync("users", new UserData
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

        public async void Save(UserData data) => await framework.Sql.InsertOrUpdateAsync("users", data);

        public async Task<DateTime> GetLastConnectionTime(Player player)
        {
            var result = await GetUser(player);
            return DateTime.Parse(result.LastConnection).ToLocalTime();
        }
    }
}
