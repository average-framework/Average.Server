using Average.Server.Framework.Commands;
using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Rpc;
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

            Init();
            MigrateDatabase();
            Register();
        }

        internal void MigrateDatabase()
        {
            var context = new DbContextFactory().CreateDbContext();
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count != 0)
            {
                Logger.Warn("[Database] Appling pending migrations.. this may take a few moments.");
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
            _container.RegisterInstance(new PlayerList());

            _container.RegisterInstance(_main._attachCallback);
            _container.RegisterInstance(_main._detachCallback);

            // Rpc
            _container.Register<RpcRequest>(Reuse.Transient);

            // Database
            _container.Register<DbContextFactory>();

            // Managers
            _container.Register<PermissionManager>();
            _container.Register<EventManager>();
            _container.Register<CommandManager>();
            _container.Register<ThreadManager>();
            _container.Register<SyncManager>();
            _container.Register<RequestInternalManager>();
            _container.Register<RequestManager>();

            // Repositories
            _container.Register<UserRepository>();
            _container.Register<CharacterRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();
            _container.Register<ClientService>();
            _container.Register<UserStateService>();
            _container.Register<AreaService>();
            _container.Register<UIService>();
            _container.Register<MenuService>();
            _container.Register<SpawnService>();

            // Handlers
            _container.Register<CommandHandler>();
            _container.Register<UserHandler>();
            _container.Register<ClientHandler>();
            _container.Register<CharacterHandler>();

            // Commands
            _container.Register<CharacterCommand>();

            // Reflections
            _container.GetService<ThreadManager>().Reflect();
            _container.GetService<SyncManager>().Reflect();
            _container.GetService<EventManager>().Reflect();
            _container.GetService<CommandManager>().Reflect();
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
