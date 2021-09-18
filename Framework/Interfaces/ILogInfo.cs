using Average.Server.Framework.Diagnostics;
using System;

namespace SDK.Shared.Diagnostics
{
    public interface ILogInfo
    {
        string Message { get; }
        LogLevel Level { get; }
        DateTime Date { get; }
    }
}
