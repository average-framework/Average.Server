using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Sync;
using CitizenFX.Core;
using DryIoc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class SyncService : IService
    {
        private readonly IContainer _container;
        private readonly ThreadService _thread;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private Dictionary<string, PropertySyncState> _propertiesSyncs = new Dictionary<string, PropertySyncState>();
        private List<PropertyGetSyncState> _propertiesGetSyncs = new List<PropertyGetSyncState>();
        private Dictionary<string, PropertyNetworkSyncState> _networkedPropertiesSyncs = new Dictionary<string, PropertyNetworkSyncState>();

        private const int SyncRate = 60;

        public SyncService(IContainer container, ThreadService thread)
        {
            _container = container;
            _thread = thread;

            _thread.StartThread(Update);

            Logger.Write("SyncManager", "Initialized successfully");
        }

        private async Task Update()
        {
            SynchronizeProperties();
            SynchronizeNetworkedProperties();

            await BaseScript.Delay(SyncRate);
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.GetService(service);
                    var properties = service.GetProperties(flags);

                    for (int i = 0; i < properties.Length; i++)
                    {
                        var property = properties[i];
                        var attrs = property.GetAttributes().ToList();

                        if (attrs != null)
                        {
                            if (attrs.Exists(x => x.GetType() == typeof(SyncAttribute)))
                            {
                                var attr = attrs.Find(x => x.GetType() == typeof(SyncAttribute)) as SyncAttribute;
                                RegisterInternalSyncProperty(attr, _service, ref property);
                            }
                            else if (attrs.Exists(x => x.GetType() == typeof(NetworkSyncAttribute)))
                            {
                                var attr = attrs.Find(x => x.GetType() == typeof(NetworkSyncAttribute)) as NetworkSyncAttribute;
                                RegisterInternalNetworkSyncProperty(attr, _service, ref property);
                            }
                            else if (attrs.Exists(x => x.GetType() == typeof(GetSyncAttribute)))
                            {
                                var attr = attrs.Find(x => x.GetType() == typeof(GetSyncAttribute)) as GetSyncAttribute;
                                RegisterInternalGetSyncProperty(attr, _service, ref property);
                            }
                        }
                    }
                }
            }
        }

        internal object GetPropertyValue(PropertyInfo property, object classObj)
        {
            if (property.GetIndexParameters().Length == 0)
                return property.GetValue(classObj, null);

            return null;
        }

        internal object GetFieldValue(FieldInfo field, object classObj) => field.GetValue(classObj);

        internal void SynchronizeProperties()
        {
            for (int i = 0; i < _propertiesGetSyncs.Count; i++)
            {
                var getSync = _propertiesGetSyncs[i];

                if (_propertiesSyncs.ContainsKey(getSync.Attribute.Name))
                {
                    var sync = _propertiesSyncs[getSync.Attribute.Name];

                    if (sync.Property.PropertyType == getSync.Property.PropertyType)
                    {
                        var syncValue = GetPropertyValue(sync.Property, sync.ClassObj);

                        if (sync.LastValue != syncValue)
                        {
                            sync.LastValue = syncValue;
                            getSync.Property.SetValue(getSync.ClassObj, syncValue, null);
                        }
                    }
                    else
                    {
                        Logger.Error($"Unable to sync properties from {sync.Attribute.Name}: {sync.Property.Name} with {getSync.Attribute.Name}: {getSync.Property.Name} because types is not the same [{string.Join(", ", sync.Property.PropertyType, getSync.Property.PropertyType)}]");
                    }
                }
            }
        }

        internal void SynchronizeNetworkedProperties()
        {
            for (int i = 0; i < _networkedPropertiesSyncs.Count; i++)
            {
                var sync = _networkedPropertiesSyncs.ElementAt(i).Value;
                var syncValue = GetPropertyValue(sync.Property, sync.ClassObj);

                if (sync.LastValue == null)
                    sync.LastValue = syncValue;

                if (!sync.LastValue.Equals(syncValue))
                {
                    sync.LastValue = syncValue;
                    BaseScript.TriggerClientEvent("sync:sync_property", sync.Attribute.Name, syncValue);
                }
            }
        }

        internal void RegisterInternalSyncProperty(SyncAttribute syncAttr, object classObj, ref PropertyInfo property)
        {
            if (!_propertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _propertiesSyncs.Add(syncAttr.Name, new PropertySyncState(syncAttr, property, classObj));

                    Logger.Debug($"Registering [Sync]: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    Logger.Error($"Unable to register [Sync]: {syncAttr.Name} on property: {property.Name}, [Sync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                Logger.Error($"Unable to register [Sync]: {syncAttr.Name} on property: {property.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        internal void RegisterInternalGetSyncProperty(GetSyncAttribute getSyncAttr, object classObj, ref PropertyInfo property)
        {
            if (property.CanWrite && property.CanRead)
            {
                _propertiesGetSyncs.Add(new PropertyGetSyncState(getSyncAttr, property, classObj));

                Logger.Debug($"Registering [GetSync]: {getSyncAttr.Name} on property: {property.Name}.");
            }
            else
            {
                Logger.Error($"Unable to register [GetSync]: {getSyncAttr.Name} on property: {property.Name}, [GetSync] attribute can only be placed on getter & setter property.");
            }
        }

        internal void RegisterInternalNetworkSyncProperty(NetworkSyncAttribute syncAttr, object classObj, ref PropertyInfo property)
        {
            if (!_networkedPropertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _networkedPropertiesSyncs.Add(syncAttr.Name, new PropertyNetworkSyncState(syncAttr, property, classObj));

                    Logger.Debug($"Registering [NetworkSync]: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    Logger.Error($"Unable to register [NetworkSync]: {syncAttr.Name} on property: {property.Name}, [NetworkSync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                Logger.Error($"Unable to register [NetworkSync]: {syncAttr.Name} on property: {property.Name}, an [NetworkSync] attribute have already been registered with this name.");
            }
        }

        public IEnumerable<PropertySyncState> GetAllSyncProperties() => _propertiesSyncs.Values.AsEnumerable();
        public IEnumerable<PropertyGetSyncState> GetAllGetSyncProperties() => _propertiesGetSyncs.AsEnumerable();
        public IEnumerable<PropertyNetworkSyncState> GetAllNetworkedSyncProperties() => _networkedPropertiesSyncs.Values.AsEnumerable();
    }
}
