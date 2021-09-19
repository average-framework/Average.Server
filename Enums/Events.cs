namespace Average.Server.Enums
{
    internal static class Events
    {
        internal const string PlayerConnecting = "playerConnecting";
        internal const string PlayerDisconnected = "playerDropped";
        internal const string ResourceStop = "onResourceStop";
        internal const string ResourceStart = "onResourceStart";
        internal const string ResourceListRefresh = "onResourceListRefresh";
        internal const string ResourceStarting = "onResourceStarting";
        internal const string ServerResourceStart = "onServerResourceStart";
        internal const string ServerResourceStop = "onServerResourceStop";
        internal const string PlayerJoining = "playerJoining";
        internal const string EntityCreated = "entityCreated";
        internal const string EntityCreating = "entityCreating";
        internal const string EntityRemoved = "entityRemoved";
        internal const string PlayerEnteredScope = "playerEnteredScope";
        internal const string PlayerLeftScope = "playerLeftScope";
    }
}
