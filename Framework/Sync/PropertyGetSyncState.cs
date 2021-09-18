using Average.Server.Framework.Attributes;
using System.Reflection;

namespace Average.Server.Framework.Sync
{
    public class PropertyGetSyncState
    {
        public GetSyncAttribute Attribute { get; }
        public PropertyInfo Property { get; }
        public object ClassObj { get; }

        public PropertyGetSyncState(GetSyncAttribute getSyncAttr, PropertyInfo property, object classObj)
        {
            Attribute = getSyncAttr;
            Property = property;
            ClassObj = classObj;
        }
    }
}
