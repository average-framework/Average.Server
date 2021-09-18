using Average.Server.Services;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;

namespace Average.Server.Handlers
{
    public class UserHandler : IHandler
    {
        private readonly UserService _userService;

        public UserHandler(UserService userService)
        {
            _userService = userService;

            #region Rpc

            //Rpc.Event("user:get").On(async (message, callback) =>
            //{
            //    Log.Debug("Getted user");
            //    var data = await GetUser(Players[message.Target]);
            //    callback(data);
            //});

            #endregion
        }
    }
}
