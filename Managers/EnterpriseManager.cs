using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;

namespace Average.Server.Managers
{
    public class EnterpriseManager : InternalPlugin, IEnterpriseManager
    {
        public const string TableName = "enterprises";
        
        private List<EnterpriseData> _enterprises { get; set; } = new List<EnterpriseData>();
        private Dictionary<string, List<CharacterData>> _employees { get; } = new Dictionary<string, List<CharacterData>>();

        public override void OnInitialized()
        {
            #region Rpc

            Rpc.Event("Enterprise.GetAllCharactersInfos").On((message, callback) =>
            {
                try
                {
                    callback(JsonConvert.SerializeObject(GetAllCharactersInfos()));
                }
                catch (Exception ex)
                {
                    Log.Error($"[Enterprise] Unable to load characters infos. Error: {ex.Message}\n{ex.StackTrace}.");
                }
            });
            
            Rpc.Event("Enterprise.LoadEmployees").On<string>(async (jobName, callback) =>
            {
                try
                {
                    await LoadEnterprises();
                    await LoadEmployees();
                    var employees = GetEmployeesInEnterprise(jobName);
                    callback(employees);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Enterprise] Unable to load employees. Error: {ex.Message}\n{ex.StackTrace}.");
                }
            });
            
            Rpc.Event("Enterprise.Load").On<string>(async (jobName, callback) =>
            {
                try
                {
                    await LoadEnterprises();
                    var enterprise = GetEnterprise(jobName);
                    callback(enterprise);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Enterprise] Unable to load enterprise [{jobName}]. Error: {ex.Message}\n{ex.StackTrace}.");
                }
            });

            #endregion
            
            Task.Factory.StartNew(async () =>
            {
                await Sql.Wait();
                await LoadEnterprises();
            });
        }

        public async Task Save(EnterpriseData data) => await Sql.InsertOrUpdateAsync("enterprises", data);

        public EnterpriseData GetEnterprise(string jobName) => _enterprises.Find(x => x.JobName == jobName);

        public List<CharacterData> GetEmployeesInEnterprise(string jobName)
        {
            if (_employees.ContainsKey(jobName))
                return _employees[jobName];

            return new List<CharacterData>();
        }

        private async Task LoadEnterprises() => _enterprises = await Sql.GetAllAsync<EnterpriseData>(TableName);

        private async Task LoadEmployees()
        {
            _employees.Clear();
         
            var characters = await Sql.GetAllAsync<CharacterData>(CharacterManager.TableName);

            foreach (var character in characters)
            {
                var enterprise = _enterprises.Find(x => x.JobName == character.Job.Name);

                if (enterprise == null) continue;
                
                if (_employees.ContainsKey(enterprise.JobName))
                {
                    _employees[enterprise.JobName].Add(character);
                }
                else
                {
                    _employees.Add(enterprise.JobName, new List<CharacterData> {character});
                }
            }
        }

        private async Task<List<dynamic>> GetAllCharactersInfos()
        {
            var infos = new List<dynamic>();
            
            var characters = await Sql.GetAllAsync<CharacterData>(CharacterManager.TableName);
            
            foreach(var c in characters)
            {
                infos.Add(new
                {
                    FirstName = c.Firstname,
                    LastName = c.Lastname,
                    RockstarId = c.RockstarId,
                    Job = new
                    {
                        Name = c.Job.Name,
                        Role = new {
                            Name = c.Job.Role.Name
                        }
                    }
                });
            }
            
            return infos;
        }

        #region Event

        [ServerEvent("Enterprise.AddTreasury")]
        private async void OnAddTreasuryEvent(string jobName, string treasuryAmount)
        {
            try
            {
                var enterpriseIndex = _enterprises.FindIndex(x => x.JobName == jobName);
                _enterprises[enterpriseIndex].TreasuryAmount = treasuryAmount;
                await Save(_enterprises[enterpriseIndex]);
            }
            catch(Exception ex)
            {
                Log.Warn($"[Enterprise] Unable to add treasury amount. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }
        
        [ServerEvent("Enterprise.RemoveTreasury")]
        private async void OnRemoveTreasuryEvent(string jobName, string treasuryAmount)
        {
            try
            {
                var enterpriseIndex = _enterprises.FindIndex(x => x.JobName == jobName);
                _enterprises[enterpriseIndex].TreasuryAmount = treasuryAmount;
                await Save(_enterprises[enterpriseIndex]);
            }
            catch(Exception ex)
            {
                Log.Warn($"[Enterprise] Unable to remove treasury amount. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        #endregion
    }
}