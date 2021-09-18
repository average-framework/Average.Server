﻿using Average.Server.Framework.Attributes;
using System.Reflection;

namespace Average.Server.Framework.Sync
{
    public class PropertyNetworkSyncState
    {
        public NetworkSyncAttribute Attribute { get; }
        public PropertyInfo Property { get; }
        public object LastValue { get; set; }
        public object ClassObj { get; }

        public PropertyNetworkSyncState(NetworkSyncAttribute syncAttr, PropertyInfo property, object classObj)
        {
            Attribute = syncAttr;
            Property = property;
            ClassObj = classObj;
        }
    }
}