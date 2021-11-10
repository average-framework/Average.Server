using Average.Shared.Enums;
using System;

namespace Average.Server.Interfaces
{
    internal interface IServerJob : IDisposable
    {
        Guid Id { get; }
        DateTime LastTriggered { get; set; }
        TimeSpan Recurring { get; }
        JobState State { get; set; }
        Func<bool> StartCondition { get; }
        Func<bool> StopCondition { get; }
        void OnStart();
        void OnStop();
        void OnUpdate();
    }
}
