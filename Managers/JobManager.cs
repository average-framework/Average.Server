using CitizenFX.Core;
using SDK.Server.Interfaces;
using SDK.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class JobManager : IJobManager
    {
        public JobManager()
        {
            Main.eventHandlers["Job.RecruitPlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(RecruitPlayerToJobByRockstarIdEvent);
            Main.eventHandlers["Job.FiredPlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(FiredPlayerToJobByRockstarIdEvent);
            Main.eventHandlers["Job.PromotePlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(PromotePlayerToJobByRockstarIdEvent);
            Main.eventHandlers["Job.RecruitPlayerToJobByServerId"] += new Action<int, int, string, string, int>(RecruitPlayerToJobByServerIdEvent);
            Main.eventHandlers["Job.FiredPlayerToJobByServerId"] += new Action<int, int, string, string, int>(FiredPlayerToJobByServerIdEvent);
            Main.eventHandlers["Job.PromotePlayerToJobByServerId"] += new Action<int, int, string, string, int>(PromotePlayerToJobByServerIdEvent);
        }

        [Export("Job.SetJob")]
        public async Task SetJob(Player player, Player target, string jobName, string roleName, int roleLevel)
        {
            var targetRockstarId = target.Identifiers["license"];
            await Main.characterManager.Load(targetRockstarId);

            var cache = Main.characterManager.GetCache(targetRockstarId);

            if (cache != null)
            {
                cache.Job.Name = jobName;
                cache.Job.Role.Name = roleName;
                cache.Job.Role.Level = roleLevel;
                Main.characterManager.UpdateCache(target, cache);

                await Main.characterManager.Save(target);

                Log.Warn($"Job setted by {player.Name} for {target.Name}, job: {jobName}, rolename: {roleName}");   
            }
            else
            {
                Log.Error(($"Unable to set job to player: {targetRockstarId}"));
            }
        }

        #region Event

        private async void RecruitPlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.RecruitPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void FiredPlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.FiredPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void PromotePlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.PromotePlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void RecruitPlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players[serverId];
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.RecruitPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void FiredPlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players[serverId];
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.FiredPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void PromotePlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = Main.players[serverId];
            var p = Main.players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.PromotePlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        #endregion
    }
}
