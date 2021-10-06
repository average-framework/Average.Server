using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Shared.Attributes;
using Average.Shared.Sync;
using CitizenFX.Core;
using DryIoc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class ReplicateStateService : IService
    {
        private readonly IContainer _container;
        private readonly EventService _eventService;
        private readonly ThreadService _threadService;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private readonly Dictionary<string, ReplicatedState> _replicatedStates = new Dictionary<string, ReplicatedState>();

        private const int SyncRate = 60;

        public ReplicateStateService(IContainer container, EventService eventService, ThreadService threadService)
        {
            _container = container;
            _eventService = eventService;
            _threadService = threadService;

            Logger.Write("ReplicateStateService", "Initialized successfully");
        }

        private async Task Update()
        {
            ReplicateValues();
            await BaseScript.Delay(SyncRate);
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            foreach (var type in types)
            {
                if (_container.IsRegistered(type))
                {
                    // Continue if the service have the same type of this class
                    if (type == GetType()) continue;

                    var service = _container.GetService(type);
                    var properties = type.GetProperties(flags);

                    for (int i = 0; i < properties.Length; i++)
                    {
                        var property = properties[i];
                        var attrs = property.GetAttributes().ToList();

                        if (attrs != null && attrs.Exists(x => x.GetType() == typeof(ReplicateAttribute)))
                        {
                            var attr = attrs.Find(x => x.GetType() == typeof(ReplicateAttribute)) as ReplicateAttribute;
                            RegisterInternalReplicateState(attr, service, ref property);
                        }
                    }
                }
            }

            // Start thread only the reflection have finded ReplicateAttribute
            if (_replicatedStates.Count > 0)
            {
                _threadService.StartThread(Update);
            }
            else
            {
                _threadService.StopThread(Update);
            }
        }

        private object GetPropertyValue(PropertyInfo property, object classObj)
        {
            if (property.GetIndexParameters().Length == 0)
            {
                return property.GetValue(classObj, null);
            }

            return null;
        }

        private void ReplicateValues()
        {
            for (int i = 0; i < _replicatedStates.Count; i++)
            {
                var state = _replicatedStates.ElementAt(i).Value;
                var stateVal = GetPropertyValue(state.Property, state.ClassObj);

                if (state.LastValue == null)
                {
                    state.LastValue = stateVal;
                }

                if (!state.LastValue.Equals(stateVal))
                {
                    state.LastValue = stateVal;
                    _eventService.EmitClients("replicate:property_value", state.Attribute.Name, stateVal);
                }
            }
        }

        private void RegisterInternalReplicateState(ReplicateAttribute attr, object classObj, ref PropertyInfo property)
        {
            if (!_replicatedStates.ContainsKey(attr.Name))
            {
                if (property.CanRead)
                {
                    _replicatedStates.Add(attr.Name, new ReplicatedState(attr, property, classObj));

                    Logger.Debug($"Registering [Replicate]: {attr.Name} on property: {property.Name}");
                }
                else
                {
                    Logger.Error($"Unable to register [Replicate]: {attr.Name} on property: {property.Name}, this property need a getter.");
                }
            }
            else
            {
                Logger.Error($"Unable to register [Replicate]: {attr.Name} on property: {property.Name}, an [Replicate] attribute have already been registered with this name.");
            }
        }
    }
}
