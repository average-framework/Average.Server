using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class JobManager : IJobManager
    {
        Logger logger;
        CharacterManager character;
        PlayerList players;

        public JobManager(Logger logger, CharacterManager character, EventHandlerDictionary eventHandler, PlayerList players)
        {
            this.logger = logger;
            this.character = character;
            this.players = players;

            eventHandler["Job.RecruitPlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(RecruitPlayerToJobByRockstarIdEvent);
            eventHandler["Job.FiredPlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(FiredPlayerToJobByRockstarIdEvent);
            eventHandler["Job.PromotePlayerToJobByRockstarId"] += new Action<int, string, string, string, int>(PromotePlayerToJobByRockstarIdEvent);
            eventHandler["Job.RecruitPlayerToJobByServerId"] += new Action<int, int, string, string, int>(RecruitPlayerToJobByServerIdEvent);
            eventHandler["Job.FiredPlayerToJobByServerId"] += new Action<int, int, string, string, int>(FiredPlayerToJobByServerIdEvent);
            eventHandler["Job.PromotePlayerToJobByServerId"] += new Action<int, int, string, string, int>(PromotePlayerToJobByServerIdEvent);
        }

        [Export("Job.SetJob")]
        public async Task SetJob(Player player, Player target, string jobName, string roleName, int roleLevel)
        {
            var targetRockstarId = target.Identifiers["license"];
            await character.Load(targetRockstarId);

            character.Characters[targetRockstarId].Job.Name = jobName;
            character.Characters[targetRockstarId].Job.Role.Name = roleName;
            character.Characters[targetRockstarId].Job.Role.Level = roleLevel;

            await character.Save(target);

            logger.Warn($"Job setted by {player.Name} for {target.Name}, job: {jobName}, rolename: {roleName}");
        }

        #region Events

        private async void RecruitPlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.RecruitPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void FiredPlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.FiredPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void PromotePlayerToJobByRockstarIdEvent(int player, string targetRockstarId, string jobName, string roleName, int roleLevel)
        {
            var target = players.ToList().Find(x => x.Identifiers["license"] == targetRockstarId);
            var p = players[player];

            if (target == null || p == null)
                return;

            target.TriggerEvent("Job.PromotePlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void RecruitPlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = players[serverId];
            var p = players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.RecruitPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void FiredPlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = players[serverId];
            var p = players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.FiredPlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        private async void PromotePlayerToJobByServerIdEvent(int player, int serverId, string jobName, string roleName, int roleLevel)
        {
            var target = players[serverId];
            var p = players[player];

            if (target == null || p == null)
                return;

            var targetRockstarId = target.Identifiers["license"];
            target.TriggerEvent("Job.PromotePlayerToJobByRockstarId", targetRockstarId, jobName, roleName, roleLevel);
            await SetJob(p, target, jobName, roleName, roleLevel);
        }

        #endregion
    }
}
