using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.DataModels;

namespace Average.Server.Services
{
    internal class PermissionService : IService
    {
        private readonly UserService _userService;

        public PermissionService(UserService userService)
        {
            _userService = userService;

            Logger.Write("PermissionService", "Initialized successfully");
        }

        public async void SetPermission(Client client, int permissionLevel)
        {
            var userData = await _userService.Get(client);
            userData.PermissionLevel = permissionLevel;
            _userService.Update(userData);

            Logger.Info($"Set permission: [{permissionLevel}] to client: {client.License}");
        }

        public bool HasPermission(UserData userData, int needPermissionLevel)
        {
            return userData.PermissionLevel >= needPermissionLevel;
        }

        public bool HasPermission(int needPermissionLevel, int permissionLevel)
        {
            return permissionLevel >= needPermissionLevel;
        }
    }
}
