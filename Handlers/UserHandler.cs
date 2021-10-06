using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Services;

namespace Average.Server.Handlers
{
    internal class UserHandler : IHandler
    {
        private readonly UserService _userService;

        public UserHandler(UserService userService)
        {
            _userService = userService;
        }
    }
}
