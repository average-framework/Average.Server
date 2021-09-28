using System;

namespace Average.Server.Interfaces
{
    public enum JobState
    {
        Started,
        Stopped
    }

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
