using Average.Server.Data;
using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Rpc;
using SDK.Shared.DataModels;
using System;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class UserManager : IUserManager
    {
        SQL sql;

        public UserManager(Logger logger, RpcRequest rpc, SQL sql, PlayerList players)
        {
            this.sql = sql;

            rpc.Event("User.GetUser").On(async (message, callback) =>
            {
                logger.Debug("Getted user");
                var data = await GetUser(players[message.Target]);
                callback(data);
            });
        }

        public async Task<UserData> GetUser(Player player)
        {
            var data = await sql.GetAllAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);
            return data[0];
        }

        public async Task<bool> Exist(Player player) => await sql.ExistsAsync<UserData>("users", x => x.RockstarId == player.Identifiers["license"]);

        public async void CreateAccount(Player player) => await sql.InsertAsync("users", new UserData
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

        public async void Save(UserData data) => await sql.InsertOrUpdateAsync("users", data);

        public async Task<DateTime> GetLastConnectionTime(Player player)
        {
            var result = await GetUser(player);
            return DateTime.Parse(result.LastConnection).ToLocalTime();
        }
    }
}
