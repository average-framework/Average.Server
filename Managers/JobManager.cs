using SDK.Server.Interfaces;
using SDK.Shared;
using System;
using System.Threading.Tasks;
using SDK.Server;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class JobManager : InternalPlugin, IJobManager
    {
        public override void OnInitialized()
        {
            #region Event

            Main.eventHandlers["Job.RecruitPlayerToJobByServerId"] += new Action<int, int, string, string, int>(RecruitPlayerToJobByServerIdEvent);
            Main.eventHandlers["Job.FiredPlayerToJobByServerId"] += new Action<int, int, string, string, int>(FiredPlayerToJobByServerIdEvent);
            Main.eventHandlers["Job.PromotePlayerToJobByServerId"] += new Action<int, int, string, string, int>(PromotePlayerToJobByServerIdEvent);

            #endregion
        }

        [Export("Job.SetJob")]
        public async Task SetJob(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var p = Players[player];
            var t = Players[target];

            if (p == null || t == null) return;
            
            var targetRockstarId = t.Identifiers["license"];
            await Character.Load(targetRockstarId);

            var cache = Character.GetCache(targetRockstarId);

            if (cache != null)
            {
                cache.Job.Name = jobName;
                cache.Job.Role.Name = roleName;
                cache.Job.Role.Level = roleLevel;
                Character.UpdateCache(t, cache);
                
                await Character.SaveData(t);

                Log.Warn($"Job setted by {p.Name} for {t.Name}, job: {jobName}, rolename: {roleName}");   
            }
            else
            {
                Log.Error(($"Unable to set job to player: {targetRockstarId}"));
            }
        }

        #region Event
        
        private async void RecruitPlayerToJobByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            t.TriggerEvent("Job.RecruitPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        private async void FiredPlayerToJobByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            t.TriggerEvent("Job.FiredPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        private async void PromotePlayerToJobByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            t.TriggerEvent("Job.PromotePlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        #endregion
    }
}
