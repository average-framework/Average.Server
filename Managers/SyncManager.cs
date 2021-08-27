using CitizenFX.Core;
using SDK.Server;
using SDK.Server.Managers;
using SDK.Shared;
using SDK.Shared.Sync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class SyncManager : InternalPlugin, ISyncManager
    {
        private static Dictionary<string, SyncPropertyState> _propertiesSyncs;
        private static Dictionary<string, SyncFieldState> _fieldsSyncs;

        private static List<GetSyncPropertyState> _propertiesGetSyncs;
        private static List<GetSyncFieldState> _fieldsGetSyncs;

        private static Dictionary<string, SyncPropertyState> _networkedPropertiesSyncs;
        private static Dictionary<string, SyncFieldState> _networkedFieldsSyncs;

        private const int SyncRate = 60;
        
        public override void OnInitialized()
        {
            _propertiesSyncs = new Dictionary<string, SyncPropertyState>();
            _propertiesGetSyncs = new List<GetSyncPropertyState>();

            _networkedPropertiesSyncs = new Dictionary<string, SyncPropertyState>();
            _networkedFieldsSyncs = new Dictionary<string, SyncFieldState>();

            _fieldsSyncs = new Dictionary<string, SyncFieldState>();
            _fieldsGetSyncs = new List<GetSyncFieldState>();
            
            Log.Warn("Cmd count: " + Command.GetCommands().Count());
            // Log.Warn($"Is null: {(Thread == null)}");
            // Thread.StartThread(Update);
        }

        private async Task Update()
        {
            await BaseScript.Delay(SyncRate);

            SyncProperties();
            SyncFields();
            SyncNetworkedProperties();
            SyncNetworkedFields();
        }

        #region Internal

        private object GetPropertyValue(PropertyInfo property, object classObj)
        {
            if (property.GetIndexParameters().Length == 0)
                return property.GetValue(classObj, null);

            return null;
        }

        private object GetFieldValue(FieldInfo field, object classObj) => field.GetValue(classObj);

        #endregion

        private void SyncProperties()
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
                        Log.Error($"Unable to sync properties from {sync.Attribute.Name}: {sync.Property.Name} with {getSync.Attribute.Name}: {getSync.Property.Name} because types is not the same [{string.Join(", ", sync.Property.PropertyType, getSync.Property.PropertyType)}]");
                    }
                }
            }
        }

        private void SyncFields()
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
                        Log.Error($"Unable to sync fields from {sync.Attribute.Name}: {sync.Field.Name} with {getSync.Attribute.Name}: {getSync.Field.Name} because types is not the same [{string.Join(", ", sync.Field.FieldType, getSync.Field.FieldType)}]");
                    }
                }
            }
        }

        private void SyncNetworkedProperties()
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
                    BaseScript.TriggerClientEvent("avg.internal.sync_property", sync.Attribute.Name, syncValue);
                }
            }
        }

        private void SyncNetworkedFields()
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
                    BaseScript.TriggerClientEvent("avg.internal.sync_field", sync.Attribute.Name, syncValue);
                }
            }
        }

        internal static void RegisterInternalSync(ref PropertyInfo property, SyncAttribute syncAttr, object classObj)
        {
            if (!_propertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _propertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));
                    Log.Debug($"Registering [Sync] attribute: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    Log.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on property: {property.Name}, [Sync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                Log.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on property: {property.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        internal static void RegisterInternalSync(ref FieldInfo field, SyncAttribute syncAttr, object classObj)
        {
            if (!_fieldsSyncs.ContainsKey(syncAttr.Name))
            {
                _fieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));
                Log.Debug($"Registering [Sync] attribute: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                Log.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on field: {field.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        internal static void RegisterInternalGetSync(ref PropertyInfo property, GetSyncAttribute getSyncAttr, object classObj)
        {
            if (property.CanWrite && property.CanRead)
            {
                _propertiesGetSyncs.Add(new GetSyncPropertyState(getSyncAttr, property, classObj));
                Log.Debug($"Registering [GetSync] attribute: {getSyncAttr.Name} on property: {property.Name}.");
            }
            else
            {
                Log.Error($"Unable to register [GetSync] attribute: {getSyncAttr.Name} on property: {property.Name}, [GetSync] attribute can only be placed on getter & setter property.");
            }
        }

        internal static void RegisterInternalGetSync(ref FieldInfo field, GetSyncAttribute getSyncAttr, object classObj)
        {
            _fieldsGetSyncs.Add(new GetSyncFieldState(getSyncAttr, field, classObj));
            Log.Debug($"Registering [GetSync] attribute: {getSyncAttr.Name} on field: {field.Name}.");
        }

        internal static void RegisterInternalNetworkSync(ref PropertyInfo property, NetworkSyncAttribute syncAttr, object classObj)
        {
            if (!_networkedPropertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    _networkedPropertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));
                    Log.Debug($"Registering [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    Log.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}, [NetworkSync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                Log.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}, an [NetworkSync] attribute have already been registered with this name.");
            }
        }

        internal static void RegisterInternalNetworkSync(ref FieldInfo field, NetworkSyncAttribute syncAttr, object classObj)
        {
            if (!_networkedFieldsSyncs.ContainsKey(syncAttr.Name))
            {
                _networkedFieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));
                Log.Debug($"Registering [NetworkSync] attribute: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                Log.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on field: {field.Name}, an [NetworkSync] attribute have already been registered with this name.");
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
