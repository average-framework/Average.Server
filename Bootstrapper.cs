﻿using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Utilities;
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

            _baseConfig = FileUtility.ReadFileFromRootDir("config.json").ToJObject();

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

            _container.RegisterInstance(_main.attachCallback);
            _container.RegisterInstance(_main.detachCallback);

            // Database
            _container.Register<DbContextFactory>(Reuse.Singleton);

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
            _container.Register<PermissionService>();
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
