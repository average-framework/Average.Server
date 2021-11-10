using Average.Server.Framework.Commands;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Handlers;
using Average.Server.Framework.Jobs;
using Average.Server.Framework.Mongo;
using Average.Server.Framework.Repositories;
using Average.Server.Framework.Services;
using Average.Server.Framework.Utilities;
using Average.Server.Scripts;
using Average.Server.Scripts.Handlers;
using CitizenFX.Core;
using DryIoc;
using Newtonsoft.Json.Linq;

namespace Average.Server
{
    internal class Bootstrapper
    {
        private readonly IContainer _container;
        private readonly EventHandlerDictionary _eventHandlers;
        private readonly PlayerList _players;

        internal static JObject BaseConfig;

        public Bootstrapper(IContainer container, EventHandlerDictionary eventHandlers, PlayerList players)
        {
            _container = container;
            _eventHandlers = eventHandlers;
            _players = players;

            BaseConfig = FileUtility.ReadFileFromRootDir("config.json").ToJObject();

            Init();
            Register();
        }

        internal void Register()
        {
            //_container.RegisterDelegate(() => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton);

            // Cfx
            _container.RegisterInstance(_eventHandlers);
            _container.RegisterInstance(_players);

            // Database
            _container.Register<DatabaseContextFactory>();

            // Framework Services
            _container.Register<EventService>();
            _container.Register<RpcService>();
            _container.Register<UIService>();
            _container.Register<CommandService>();
            _container.Register<ThreadService>();
            _container.Register<RequestInternalService>();
            _container.Register<RequestService>();
            _container.Register<ServerJobService>();
            _container.Register<ClientService>();

            // Repositories
            _container.Register<UserRepository>();
            _container.Register<CharacterRepository>();
            _container.Register<WorldRepository>();
            _container.Register<InventoryRepository>();
            _container.Register<BankRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();
            _container.Register<PermissionService>();
            _container.Register<UserStateService>();
            _container.Register<AreaService>();
            _container.Register<WorldService>();
            _container.Register<DoorService>();
            _container.Register<InventoryService>();
            _container.Register<InventoryItemsService>();
            _container.Register<BankService>();

            // Jobs
            _container.Register<CharacterJob>();
            _container.Register<InventoryJob>();

            // Scripts
            _container.Register<JobScript>();
            _container.Register<CharacterCreatorScript>();
            _container.Register<AIZombieScript>();

            // Framework Handlers
            _container.Register<ClientHandler>();
            _container.Register<CharacterHandler>();
            _container.Register<RpcHandler>();
            _container.Register<InventoryHandler>();
            _container.Register<DoorHandler>();
            _container.Register<WorldHandler>();

            // Script Handlers
            _container.Register<AIZombieHandler>();

            // Commands
            _container.Register<ServerJobCommand>();
            _container.Register<CharacterCommand>();
            _container.Register<WorldCommand>();
            _container.Register<DebugCommand>();
            _container.Register<InventoryCommand>();

            // Reflections
            _container.GetService<ThreadService>().Reflect();
            _container.GetService<EventService>().Reflect();
            _container.GetService<UIService>().Reflect();
            _container.GetService<CommandService>().Reflect();
            _container.GetService<ServerJobService>().Reflect();
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
