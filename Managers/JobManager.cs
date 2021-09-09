using SDK.Server.Interfaces;
using SDK.Shared;
using System.Threading.Tasks;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Extensions;

namespace Average.Server.Managers
{
    public class JobManager : InternalPlugin, IJobManager
    {
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

        [ServerEvent("Job.RecruitPlayerByServerId")]
        private async void RecruitPlayerByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            // Event.EmitClient(player.ToPlayer(), "Job.RecruitPlayerByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            t.TriggerEvent("Job.RecruitPlayerByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        [ServerEvent("Job.FiredPlayerByServerId")]
        private async void FiredPlayerByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            t.TriggerEvent("Job.FiredPlayerByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        [ServerEvent("Job.PromotePlayerByServerId")]
        private async void PromotePlayerByServerIdEvent(int player, int target, string jobName, string roleName, int roleLevel)
        {
            var t = Players[target];
            var p = Players[player];

            if (t == null || p == null) return;

            var targetRockstarId = t.Identifiers["license"];
            t.TriggerEvent("Job.PromotePlayerByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(player, target, jobName, roleName, roleLevel);
        }

        #endregion
    }
}
