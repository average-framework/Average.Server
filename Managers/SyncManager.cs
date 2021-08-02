using CitizenFX.Core;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Managers;
using SDK.Shared;
using SDK.Shared.Sync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Managers
{
    public class SyncManager : ISyncManager
    {
        Dictionary<string, SyncPropertyState> propertiesSyncs;
        Dictionary<string, SyncFieldState> fieldsSyncs;

        List<GetSyncPropertyState> propertiesGetSyncs;
        List<GetSyncFieldState> fieldsGetSyncs;

        Dictionary<string, SyncPropertyState> networkedPropertiesSyncs;
        Dictionary<string, SyncFieldState> networkedFieldsSyncs;

        Logger logger;

        public int SyncRate { get; set; } = 60;

        public SyncManager(Logger logger, Framework framework)
        {
            this.logger = logger;

            propertiesSyncs = new Dictionary<string, SyncPropertyState>();
            propertiesGetSyncs = new List<GetSyncPropertyState>();

            networkedPropertiesSyncs = new Dictionary<string, SyncPropertyState>();
            networkedFieldsSyncs = new Dictionary<string, SyncFieldState>();

            fieldsSyncs = new Dictionary<string, SyncFieldState>();
            fieldsGetSyncs = new List<GetSyncFieldState>();

            framework.Thread.StartThread(Update);
        }

        protected async Task Update()
        {
            await BaseScript.Delay(100);

            SyncProperties();
            SyncProperties();
        }

        #region Internal

        internal object GetPropertyValue(PropertyInfo property, object classObj)
        {
            if (property.GetIndexParameters().Length == 0)
            {
                // Can get value
                return property.GetValue(classObj, null);
            }
            else
            {
                // Cannot get value
            }

            return null;
        }

        internal object GetFieldValue(FieldInfo field, object classObj) => field.GetValue(classObj);

        #endregion

        public void SyncProperties()
        {
            for (int i = 0; i < propertiesGetSyncs.Count; i++)
            {
                var getSync = propertiesGetSyncs[i];

                if (propertiesSyncs.ContainsKey(getSync.Attribute.Name))
                {
                    var sync = propertiesSyncs[getSync.Attribute.Name];

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
                        logger.Error($"Unable to sync properties from {sync.Attribute.Name}: {sync.Property.Name} with {getSync.Attribute.Name}: {getSync.Property.Name} because types is not the same [{string.Join(", ", sync.Property.PropertyType, getSync.Property.PropertyType)}]");
                    }
                }
            }
        }

        public void SyncFields()
        {
            for (int i = 0; i < fieldsGetSyncs.Count; i++)
            {
                var getSync = fieldsGetSyncs[i];

                if (fieldsSyncs.ContainsKey(getSync.Attribute.Name))
                {
                    var sync = fieldsSyncs[getSync.Attribute.Name];

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
                        logger.Error($"Unable to sync fields from {sync.Attribute.Name}: {sync.Field.Name} with {getSync.Attribute.Name}: {getSync.Field.Name} because types is not the same [{string.Join(", ", sync.Field.FieldType, getSync.Field.FieldType)}]");
                    }
                }
            }
        }

        public void SyncNetworkedProperties()
        {
            for (int i = 0; i < networkedPropertiesSyncs.Count; i++)
            {
                var sync = networkedPropertiesSyncs.ElementAt(i).Value;
                var syncValue = GetPropertyValue(sync.Property, sync.ClassObj);

                if (sync.LastValue == null)
                {
                    sync.LastValue = syncValue;
                }

                if (!sync.LastValue.Equals(syncValue))
                {
                    sync.LastValue = syncValue;
                    BaseScript.TriggerClientEvent("avg.internal.sync_property", sync.Attribute.Name, syncValue);
                }
            }
        }

        public void SyncNetworkedFields()
        {
            for (int i = 0; i < networkedFieldsSyncs.Count; i++)
            {
                var sync = networkedFieldsSyncs.ElementAt(i).Value;
                var syncValue = GetFieldValue(sync.Field, sync.ClassObj);

                if (sync.LastValue == null)
                {
                    sync.LastValue = syncValue;
                }

                if (!sync.LastValue.Equals(syncValue))
                {
                    sync.LastValue = syncValue;
                    BaseScript.TriggerClientEvent("avg.internal.sync_field", sync.Attribute.Name, syncValue);
                }
            }
        }

        public void RegisterSync(ref PropertyInfo property, SyncAttribute syncAttr, object classObj)
        {
            if (!propertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    propertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));
                    logger.Debug($"Registering [Sync] attribute: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    logger.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on property: {property.Name}, [Sync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                logger.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on property: {property.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        public void RegisterSync(ref FieldInfo field, SyncAttribute syncAttr, object classObj)
        {
            if (!fieldsSyncs.ContainsKey(syncAttr.Name))
            {
                fieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));
                logger.Debug($"Registering [Sync] attribute: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                logger.Error($"Unable to register [Sync] attribute: {syncAttr.Name} on field: {field.Name}, an [Sync] attribute have already been registered with this name.");
            }
        }

        public void RegisterGetSync(ref PropertyInfo property, GetSyncAttribute getSyncAttr, object classObj)
        {
            if (property.CanWrite && property.CanRead)
            {
                propertiesGetSyncs.Add(new GetSyncPropertyState(getSyncAttr, property, classObj));
                logger.Debug($"Registering [GetSync] attribute: {getSyncAttr.Name} on property: {property.Name}.");
            }
            else
            {
                logger.Error($"Unable to register [GetSync] attribute: {getSyncAttr.Name} on property: {property.Name}, [GetSync] attribute can only be placed on getter & setter property.");
            }
        }

        public void RegisterGetSync(ref FieldInfo field, GetSyncAttribute getSyncAttr, object classObj)
        {
            fieldsGetSyncs.Add(new GetSyncFieldState(getSyncAttr, field, classObj));
            logger.Debug($"Registering [GetSync] attribute: {getSyncAttr.Name} on field: {field.Name}.");
        }

        public void RegisterNetworkSync(ref PropertyInfo property, NetworkSyncAttribute syncAttr, object classObj)
        {
            if (!networkedPropertiesSyncs.ContainsKey(syncAttr.Name))
            {
                if (property.CanWrite && property.CanRead)
                {
                    networkedPropertiesSyncs.Add(syncAttr.Name, new SyncPropertyState(syncAttr, property, classObj));
                    logger.Debug($"Registering [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}");
                }
                else
                {
                    logger.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}, [NetworkSync] attribute can only be placed on getter & setter property.");
                }
            }
            else
            {
                logger.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on property: {property.Name}, an [NetworkSync] attribute have already been registered with this name.");
            }
        }

        public void RegisterNetworkSync(ref FieldInfo field, NetworkSyncAttribute syncAttr, object classObj)
        {
            if (!networkedFieldsSyncs.ContainsKey(syncAttr.Name))
            {
                networkedFieldsSyncs.Add(syncAttr.Name, new SyncFieldState(syncAttr, field, classObj));
                logger.Debug($"Registering [NetworkSync] attribute: {syncAttr.Name} on field: {field.Name}");
            }
            else
            {
                logger.Error($"Unable to register [NetworkSync] attribute: {syncAttr.Name} on field: {field.Name}, an [NetworkSync] attribute have already been registered with this name.");
            }
        }

        public async Task SyncUpdate()
        {
            await BaseScript.Delay(SyncRate);

            SyncProperties();
            SyncFields();
        }

        public IEnumerable<SyncPropertyState> GetAllSyncProperties() => propertiesSyncs.Values.AsEnumerable();

        public IEnumerable<SyncFieldState> GetAllSyncFields() => fieldsSyncs.Values.AsEnumerable();

        public IEnumerable<GetSyncPropertyState> GetAllGetSyncProperties() => propertiesGetSyncs.AsEnumerable();

        public IEnumerable<GetSyncFieldState> GetAllGetSyncFields() => fieldsGetSyncs.AsEnumerable();

        public IEnumerable<SyncPropertyState> GetAllNetworkedSyncProperties() => networkedPropertiesSyncs.Values.AsEnumerable();

        public IEnumerable<SyncFieldState> GetAllNetworkedSyncFields() => networkedFieldsSyncs.Values.AsEnumerable();
    }
}
