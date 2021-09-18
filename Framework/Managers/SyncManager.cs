using CitizenFX.Core;
using DryIoc;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Shared;
using SDK.Shared.Sync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Framework.Managers
{
    internal class SyncManager
    {
        private readonly IContainer _container;
        private readonly ThreadManager _thread;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private Dictionary<string, SyncPropertyState> _propertiesSyncs = new Dictionary<string, SyncPropertyState>();
        private Dictionary<string, SyncFieldState> _fieldsSyncs = new Dictionary<string, SyncFieldState>();

        private List<GetSyncPropertyState> _propertiesGetSyncs = new List<GetSyncPropertyState>();
        private List<GetSyncFieldState> _fieldsGetSyncs = new List<GetSyncFieldState>();

        private Dictionary<string, SyncPropertyState> _networkedPropertiesSyncs = new Dictionary<string, SyncPropertyState>();
        private Dictionary<string, SyncFieldState> _networkedFieldsSyncs = new Dictionary<string, SyncFieldState>();

        private const int SyncRate = 60;

        public SyncManager(IContainer container, ThreadManager thread)
        {
            _container = container;
            _thread = thread;

            _thread.StartThread(Update);

            Logger.Write("SyncManager", "Initialized successfully");
        }

        private async Task Update()
        {
            await BaseScript.Delay(SyncRate);

            SynchronizeProperties();
            SynchronizeFields();
            SynchronizeNetworkedProperties();
            SynchronizeNetworkedFields();
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

        internal void SynchronizeFields()
        {
            for (int i = 0; i < _fieldsGetSyncs.Count; i++)
            {
                var getSync = _fieldsGetSyncs[i];

                if (_fieldsSyncs.ContainsKey(getSync.Attribute.Name))
                {
                    var sync = _fieldsSyncs[getSync.Attribute.Name];

                    if (sync.Field.FieldType == getSync.Field.FieldType)
                    {
                        var syncValue = GetFieldValue(sync.Field, sync.ClassObj);

                        if (sync.LastValue != syncValue)
                        {
                            sync.LastValue = syncValue;
                            getSync.Field.SetValue(getSync.ClassObj, syncValue);
                        }
                    }
                    else
                    {
                        Logger.Error($"Unable to sync fields from {sync.Attribute.Name}: {sync.Field.Name} with {getSync.Attribute.Name}: {getSync.Field.Name} because types is not the same [{string.Join(", ", sync.Field.FieldType, getSync.Field.FieldType)}]");
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

        internal void SynchronizeNetworkedFields()
        {
            for (int i = 0; i < _networkedFieldsSyncs.Count; i++)
            {
                var sync = _networkedFieldsSyncs.ElementAt(i).Value;
                var syncValue = GetFieldValue(sync.Field, sync.ClassObj);

                if (sync.LastValue == null)
                    sync.LastValue = syncValue;

                if (!sync.LastValue.Equals(syncValue))
                {
                    sync.LastValue = syncValue;
                    BaseScript.TriggerClientEvent("sync:sync_field", sync.Attribute.Name, syncValue);
                }
            }
        }

        internal void RegisterInternalSyncProperty(SyncAttribute syncAttr, object classObj, ref PropertyInfo property)
        {
            if (!_propertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _propertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));

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

        internal void RegisterInternalSyncField(SyncAttribute syncAttr, object classObj, ref FieldInfo field)
        {
            if (!_fieldsSyncs.ContainsKey(syncAttr.Name))
            {
                _fieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));

                Logger.Debug($"Registering [Sync]: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                Logger.Error($"Unable to register [Sync]: {syncAttr.Name} on field: {field.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        internal void RegisterInternalGetSyncProperty(GetSyncAttribute getSyncAttr, object classObj, ref PropertyInfo property)
        {
            if (property.CanWrite && property.CanRead)
            {
                _propertiesGetSyncs.Add(new GetSyncPropertyState(getSyncAttr, property, classObj));

                Logger.Debug($"Registering [GetSync]: {getSyncAttr.Name} on property: {property.Name}.");
            }
            else
            {
                Logger.Error($"Unable to register [GetSync]: {getSyncAttr.Name} on property: {property.Name}, [GetSync] attribute can only be placed on getter & setter property.");
            }
        }

        internal void RegisterInternalGetSyncField(GetSyncAttribute getSyncAttr, object classObj, ref FieldInfo field)
        {
            _fieldsGetSyncs.Add(new GetSyncFieldState(getSyncAttr, field, classObj));

            Logger.Debug($"Registering [GetSync]: {getSyncAttr.Name} on field: {field.Name}.");
        }

        internal void RegisterInternalNetworkSyncProperty(NetworkSyncAttribute syncAttr, object classObj, ref PropertyInfo property)
        {
            if (!_networkedPropertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _networkedPropertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));

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

        internal void RegisterInternalNetworkSyncField(NetworkSyncAttribute syncAttr, object classObj, ref FieldInfo field)
        {
            if (!_networkedFieldsSyncs.ContainsKey(syncAttr.Name))
            {
                _networkedFieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));

                Logger.Debug($"Registering [NetworkSync]: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                Logger.Error($"Unable to register [NetworkSync]: {syncAttr.Name} on field: {field.Name}, an [NetworkSync] attribute have already been registered with this name.");
            }
        }

        public IEnumerable<SyncPropertyState> GetAllSyncProperties() => _propertiesSyncs.Values.AsEnumerable();
        public IEnumerable<SyncFieldState> GetAllSyncFields() => _fieldsSyncs.Values.AsEnumerable();
        public IEnumerable<GetSyncPropertyState> GetAllGetSyncProperties() => _propertiesGetSyncs.AsEnumerable();
        public IEnumerable<GetSyncFieldState> GetAllGetSyncFields() => _fieldsGetSyncs.AsEnumerable();
        public IEnumerable<SyncPropertyState> GetAllNetworkedSyncProperties() => _networkedPropertiesSyncs.Values.AsEnumerable();
        public IEnumerable<SyncFieldState> GetAllNetworkedSyncFields() => _networkedFieldsSyncs.Values.AsEnumerable();
    }
}
