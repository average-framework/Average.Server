using Average.Server.Framework.Commands;
using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
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
            _container.RegisterInstance(_players);

            _container.RegisterInstance(_main._attachCallback);
            _container.RegisterInstance(_main._detachCallback);

            // Database
            _container.Register<DbContextFactory>();

            // Framework Services
            _container.Register<EventService>();
            _container.Register<RpcService>(Reuse.Transient);
            _container.Register<PermissionService>();
            _container.Register<CommandService>();
            _container.Register<ThreadService>();
            _container.Register<ReplicateStateService>();
            _container.Register<RequestInternalService>();
            _container.Register<RequestService>();

            // Repositories
            _container.Register<UserRepository>();
            _container.Register<CharacterRepository>();
            _container.Register<WorldRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();
            _container.Register<ClientService>();
            _container.Register<UserStateService>();
            _container.Register<AreaService>();
            _container.Register<CharacterCreatorService>();
            _container.Register<WorldService>();

            // Handlers
            _container.Register<CommandHandler>();
            _container.Register<UserHandler>();
            _container.Register<ClientHandler>();
            _container.Register<CharacterHandler>();

            // Commands
            _container.Register<CharacterCommand>();
            _container.Register<WorldCommand>();

            // Reflections
            _container.GetService<ThreadService>().Reflect();
            _container.GetService<ReplicateStateService>().Reflect();
            _container.GetService<EventService>().Reflect();
            _container.GetService<CommandService>().Reflect();
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
