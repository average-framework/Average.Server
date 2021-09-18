﻿using Average.Server.Database;
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
using System;
using System.Linq;

namespace Average.Server
{
    public class Bootstrapper
    {
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;
        private readonly PlayerList _players;
        private JObject _baseConfig;

        public Bootstrapper(IContainer container, EventHandlerDictionary eventHandlers, PlayerList players)
        {
            _container = container;
            _eventHandlers = eventHandlers;
            _players = players;

            _baseConfig = SDK.Server.Configuration.ParseToObj("config.json");

            Init();
            MigrateDatabase();
        }

        private void MigrateDatabase()
        {
            var context = new DbContextFactory().CreateDbContext();
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count != 0)
            {
                //context.Database.Migrate();
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

            // Database
            _container.Register<DbContextFactory>(Reuse.Singleton);

            // Registers
            _container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IHandler), Reuse.Singleton);
            //_container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IRepository), Reuse.Singleton);
            //_container.RegisterMany(new[] { typeof(Bootstrapper).Assembly }, serviceTypeCondition: t => t == typeof(IService), Reuse.Singleton);

            // Repositories
            _container.Register<UserRepository>();
            _container.Register<CharacterRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();

            // Handlers
            _container.Register<UserHandler>();
            _container.Register<CharacterHandler>();

            // Resolves
            //_container.ResolveMany<IRepository>().ToList();
            //_container.ResolveMany<IService>().ToList();
            //_container.ResolveMany<IHandler>().ToList();

            // Managers
            _container.Register<CommandManager>();
            //_container.Resolve<CommandManager>();
            //_container.BindSingletonAndInstanciateOnStartup<CommandManager>();

            //_container.InstanciateDefinedBindings();

            // Reflections
            _container.GetService<CommandManager>().Reflect();
        }
    }
}