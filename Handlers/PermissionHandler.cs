using Average.Server.Framework.Interfaces;
using Average.Server.Services;

namespace Average.Server.Handlers
{
    public class PermissionHandler : IHandler
    {
        private readonly UserService _userService;

        public PermissionHandler(UserService userService)
        {
            _userService = userService;
        }
    }
}
