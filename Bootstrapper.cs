using Average.Server.Framework.Commands;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Mongo;
using Average.Server.Framework.Utilities;
using Average.Server.Handlers;
using Average.Server.Jobs;
using Average.Server.Repositories;
using Average.Server.Services;
using CitizenFX.Core;
using DryIoc;
using MemBus;
using MemBus.Configurators;
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
            _container.RegisterDelegate(() => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton);

            // Cfx
            _container.RegisterInstance(_eventHandlers);
            _container.RegisterInstance(_players);

            // Database
            _container.Register<DatabaseContextFactory>();

            // Framework Services
            _container.Register<EventService>();
            _container.Register<RpcService>();
            _container.Register<CommandService>();
            _container.Register<ThreadService>();
            _container.Register<ReplicateStateService>();
            _container.Register<RequestInternalService>();
            _container.Register<RequestService>();
            _container.Register<ServerJobService>();
            _container.Register<ClientService>();
            _container.Register<InputService>();

            // Repositories
            _container.Register<UserRepository>();
            _container.Register<CharacterRepository>();
            _container.Register<WorldRepository>();

            // Services
            _container.Register<UserService>();
            _container.Register<CharacterService>();
            _container.Register<JobService>();
            _container.Register<PermissionService>();
            _container.Register<UserStateService>();
            _container.Register<AreaService>();
            _container.Register<CharacterCreatorService>();
            _container.Register<WorldService>();
            _container.Register<DoorService>();
            _container.Register<GameService>();

            // Jobs
            _container.Register<CharacterJob>();

            // Handlers
            _container.Register<CommandHandler>();
            _container.Register<UserHandler>();
            _container.Register<ClientHandler>();
            _container.Register<CharacterHandler>();
            _container.Register<InputHandler>();
            _container.Register<RpcHandler>();

            // Commands
            _container.Register<ServerJobCommand>();
            _container.Register<CharacterCommand>();
            _container.Register<WorldCommand>();
            _container.Register<DebugCommand>();

            // Reflections
            _container.GetService<ThreadService>().Reflect();
            _container.GetService<ReplicateStateService>().Reflect();
            _container.GetService<EventService>().Reflect();
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
