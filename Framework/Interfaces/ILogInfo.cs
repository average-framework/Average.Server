using Average.Server.Framework.Diagnostics;
using System;

namespace Average.Server.Framework.Interfaces
{
    public interface ILogInfo
    {
        string Message { get; }
        LogLevel Level { get; }
        DateTime Date { get; }
    }
}
