using Average.Server.Services;
using SDK.Server.Interfaces;

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
