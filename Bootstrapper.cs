using Average.Server.Database;
using Average.Server.Extensions;
using Average.Server.Handlers;
using Average.Server.Managers;
using Average.Server.Repositories;
using Average.Server.Services;
using CitizenFX.Core;
using DryIoc;
using MemBus;
using MemBus.Configurators;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using System.Linq;

namespace Average.Server
{
    internal class Bootstrapper
    {
        private readonly Main _main;
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;
        private readonly PlayerList _players;
        private JObject _baseConfig;

        public Bootstrapper(Main main, IContainer container, EventHandlerDictionary eventHandlers, PlayerList players)
        {
            _main = main;
            _container = container;
            _eventHandlers = eventHandlers;
            _players = players;

            _baseConfig = SDK.Server.Configuration.ParseToObj("config.json");

            MigrateDatabase();
            Init();
        }

        private void MigrateDatabase()
        {
            var context = new DbContextFactory().CreateDbContext();
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count != 0)
            {
                context.Database.Migrate();
                Logger.Warn($"[Migration] Successfully applied {pendingMigrations.Count} pending migrations.");
            }
            else
            {
                Logger.Warn("[Migration] No pending database migrations.");
            }
        }

        private void Init()
        {
            _container.RegisterDelegate(() => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton);

            // Others
            _container.RegisterInstance(_eventHandlers);
            _container.RegisterInstance(_players);

            _container.RegisterInstance(Main.attachCallback);
            _container.RegisterInstance(Main.detachCallback);

            // Database
            _container.Register<DbContextFactory>(Reuse.Singleton);

            // Registers
            _container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IHandler), Reuse.Singleton);
            //_container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IRepository), Reuse.Singleton);
            //_container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IService), Reuse.Singleton);

            // Repositories
            _container.Register<UserRepository>(Reuse.Singleton);
            _container.Register<CharacterRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();

            // Handlers
            _container.Register<UserHandler>();
            _container.Register<CharacterHandler>();

            // Managers
            _container.Register<PermissionManager>();
            _container.Register<EventManager>();
            _container.Register<CommandManager>();
            _container.Register<ThreadManager>();
            _container.Register<ExportManager>();
            _container.Register<SyncManager>();
            _container.Register<RequestInternalManager>();
            _container.Register<RequestManager>();

            // Reflections
            _container.GetService<CommandManager>().Reflect();
            _container.GetService<ThreadManager>().Reflect();
            _container.GetService<ExportManager>().Reflect();
            _container.GetService<SyncManager>().Reflect();
        }
    }
}
