using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Utilities;
using Average.Server.Handlers;
using Average.Server.Repositories;
using Average.Server.Services;
using CitizenFX.Core;
using DryIoc;
using MemBus;
using MemBus.Configurators;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Average.Server
{
    internal class Bootstrapper
    {
        private readonly Main _main;
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;
        private readonly PlayerList _players;

        internal static JObject BaseConfig = null;

        public Bootstrapper(Main main, IContainer container, EventHandlerDictionary eventHandlers, PlayerList players)
        {
            _main = main;
            _container = container;
            _eventHandlers = eventHandlers;
            _players = players;

            BaseConfig = FileUtility.ReadFileFromRootDir("config.json").ToJObject();

            MigrateDatabase();
            Init();
            Register();
        }

        internal void MigrateDatabase()
        {
            var context = new DbContextFactory().CreateDbContext();
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count != 0)
            {
                context.Database.Migrate();
                Logger.Warn($"[Database] Successfully applied {pendingMigrations.Count} pending migrations.");
            }
            else
            {
                Logger.Warn("[Database] No pending database migrations.");
            }
        }

        internal void Register()
        {
            _container.RegisterDelegate(() => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton);

            // Others
            _container.RegisterInstance(_eventHandlers);
            _container.RegisterInstance(_players);

            _container.RegisterInstance(_main.attachCallback);
            _container.RegisterInstance(_main.detachCallback);

            // Database
            _container.Register<DbContextFactory>(Reuse.Singleton);

            // Managers
            _container.Register<EventManager>();
            _container.Register<CommandManager>();
            _container.Register<ThreadManager>();
            _container.Register<ExportManager>();
            _container.Register<SyncManager>();
            _container.Register<RequestInternalManager>();
            _container.Register<RequestManager>();

            // Repositories
            _container.Register<UserRepository>(Reuse.Singleton);
            _container.Register<CharacterRepository>();

            // Services
            _container.Register<PermissionService>();
            _container.Register<UserService>();
            _container.Register<CharacterService>();
            _container.Register<ClientListService>();
            _container.Register<UserStateService>();

            // Handlers
            _container.Register<UserHandler>();
            _container.Register<CharacterHandler>();

            // Reflections
            _container.GetService<CommandManager>().Reflect();
            _container.GetService<ThreadManager>().Reflect();
            _container.GetService<ExportManager>().Reflect();
            _container.GetService<SyncManager>().Reflect();
        }

        internal void Init()
        {
#if DEBUG
            Logger.IsDebug = true;
#else
            Logger.IsDebug = false;
#endif
        }
    }
}
