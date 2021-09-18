using Average.Server.Framework.Interfaces;
using Average.Server.Services;

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
