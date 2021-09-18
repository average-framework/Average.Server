//using SDK.Server.Interfaces;
//using SDK.Shared;
//using System.Threading.Tasks;
//using SDK.Server;
//using SDK.Server.Diagnostics;
//using SDK.Server.Extensions;

//namespace Average.Server.Managers
//{
//    public class JobManager : InternalPlugin, IJobManager
//    {
//        public async Task SetJob(int player, int target, string jobName, int jobLevel)
//        {
//            var p = Players[player];
//            var t = Players[target];

//            if (p == null || t == null) return;
            
//            var targetRockstarId = t.Identifiers["license"];
//            await Character.Load(targetRockstarId);

//            var cache = Character.GetCache(targetRockstarId);

//            if (cache != null)
//            {
//                cache.Job.Name = jobName;
//                cache.Job.Level = jobLevel;
//                Character.UpdateCache(t, cache);
                
//                await Character.SaveData(t);

//                Log.Warn($"[Job] Job set by {p.Name} for {t.Name}, job: {jobName}");
//            }
//            else
//            {
//                Log.Error(($"[Job] Unable to set job to player: {targetRockstarId}"));
//            }
//        }

//        #region Event

//        [ServerEvent("Job.RecruitPlayerByServerId")]
//        private async void RecruitPlayerByServerIdEvent(int player, int target, string jobName, int jobLevel)
//        {
//            var t = Players[target];
//            var p = Players[player];

//            if (t == null || p == null) return;

//            var targetRockstarId = t.Identifiers["license"];
//            Event.EmitClient(player.ToPlayer(), "Job.RecruitPlayerByRockstarId", targetRockstarId, jobName, jobLevel);
//            await SetJob(player, target, jobName, jobLevel);
//        }

//        [ServerEvent("Job.FiredPlayerByServerId")]
//        private async void FiredPlayerByServerIdEvent(int player, int target, string jobName, int jobLevel)
//        {
//            var t = Players[target];
//            var p = Players[player];

//            if (t == null || p == null) return;

//            var targetRockstarId = t.Identifiers["license"];
//            t.TriggerEvent("Job.FiredPlayerByRockstarId", targetRockstarId, jobName, jobLevel);
//            await SetJob(player, target, jobName, jobLevel);
//        }

//        [ServerEvent("Job.PromotePlayerByServerId")]
//        private async void PromotePlayerByServerIdEvent(int player, int target, string jobName, int jobLevel)
//        {
//            var t = Players[target];
//            var p = Players[player];

//            if (t == null || p == null) return;

//            var targetRockstarId = t.Identifiers["license"];
//            Event.EmitClient(player.ToPlayer(), "Job.PromotePlayerByRockstarId", targetRockstarId, jobName, jobLevel);
//            await SetJob(player, target, jobName, jobLevel);
//        }

//        #endregion
//    }
//}
