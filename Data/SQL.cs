using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;

namespace Average.Server.Data
{
    public class SQL : ISQL
    {
        private MySqlConnection _connection;
        private readonly SQLConnection _connectionInfo;

        public bool IsOpen { get; private set; }
        public bool IsWorking { get; private set; }

        public SQL()
        {
            var baseConfig = SDK.Server.Configuration.ParseToObj("config.json");

            _connectionInfo = new SQLConnection((string) baseConfig["MySQL"]["Host"], (int) baseConfig["MySQL"]["Port"],
                (string) baseConfig["MySQL"]["Database"], (string) baseConfig["MySQL"]["Username"],
                (string) baseConfig["MySQL"]["Password"]);
        }

        public async void Connect()
        {
            try
            {
                _connection = new MySqlConnection($"SERVER={_connectionInfo.Host};PORT={_connectionInfo.Port};DATABASE={_connectionInfo.Database};UID={_connectionInfo.Username};PASSWORD={_connectionInfo.Password};");
                await _connection.OpenAsync();

                IsOpen = true;

                Logger.Info($"SQL ready");
            }
            catch (MySqlException ex)
            {
                Logger.Error("Unable to initialize mysql connection.");
            }
        }

        public async Task Wait()
        {
            while (IsWorking) await Task.Delay(0);
        }

        public async Task<List<T>> GetAllAsync<T>(string table)
        {
            IsWorking = true;

            if (!IsOpen)
                return new List<T>();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table}";
            var reader = await cmd.ExecuteReaderAsync();
            var results = await MapDeserialize<T>(reader);

            IsWorking = false;
            return results;
        }

        public async Task<List<T>> GetAllAsync<T>(string table, string where)
        {
            IsWorking = true;

            if (!IsOpen)
                return new List<T>();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table} WHERE {where}";
            var reader = await cmd.ExecuteReaderAsync();
            var result = await MapDeserialize<T>(reader);

            IsWorking = false;
            return result;
        }

        public async Task<bool> ExistsAsync<T>(string table, Func<T, bool> predicate)
        {
            IsWorking = true;

            if (!IsOpen)
                return false;

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table}";
            var reader = await cmd.ExecuteReaderAsync();
            var result = await MapDeserialize<T>(reader);
            var exists = false;

            for (int i = 0; i < result.Count; i++)
            {
                if (predicate.Invoke(result[i]))
                {
                    exists = true;
                    break;
                }
            }

            IsWorking = false;
            return exists;
        }

        public async Task<bool> DeleteAllAsync(string table, string where)
        {
            IsWorking = true;

            if (!IsOpen)
                return false;

            var isUpdated = false;
            
            try
            {
                var cmd = _connection.CreateCommand();
                cmd.CommandText = $"DELETE FROM {table} WHERE {where}";
                await cmd.ExecuteNonQueryAsync();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"[SQL] Unable to delete this value at {where}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
            
            IsWorking = false;
            return isUpdated;
        }

        public async Task InsertAsync(string table, object value)
        {
            IsWorking = true;

            if (!IsOpen)
                return;

            try
            {
                var result = MapSerialize(value);
                var cmd = _connection.CreateCommand();

                for (var i = 0; i < result.Count; i++)
                {
                    var val = result.ElementAt(i);

                    if (val.Value.GetType() == typeof(double) || val.Value.GetType() == typeof(float) || val.Value.GetType() == typeof(decimal) || val.Value.GetType() == typeof(int))
                    {
                        result[val.Key] = $"{val.Value}";
                    }
                    else
                    {
                        result[val.Key] = $"'{val.Value}'";
                    }
                }

                var keys = string.Join(",", result.Keys);
                var values = string.Join(",", result.Values);
                cmd.CommandText = $"INSERT INTO {table}({keys}) VALUES({values})";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("[SQL] Unable to insert this value: " + ex.Message);
            }

            IsWorking = false;
        }

        public async Task InsertOrUpdateAsync(string table, object newValue)
        {
            if (!IsOpen) return;

            try
            {
                IsWorking = true;

                var result = MapSerialize(newValue);
                var duplicateDict = new Dictionary<string, object>();

                for (var i = 0; i < result.Count; i++)
                {
                    var val = result.ElementAt(i);

                    if (val.Value.GetType() == typeof(double) || val.Value.GetType() == typeof(float) || val.Value.GetType() == typeof(decimal) || val.Value.GetType() == typeof(int))
                    {
                        result[val.Key] = val.Value;
                        duplicateDict[val.Key] = $"{val.Key}={val.Value.ToString().Replace(",", ".")}";
                    }
                    else
                    {
                        result[val.Key] = $"'{val.Value}'";
                        duplicateDict[val.Key] = $"{val.Key}='{val.Value}'";
                    }
                }

                var keys = string.Join(",", result.Keys);
                var values = string.Join(",", result.Values);
                var duplicates = string.Join(", ", duplicateDict.Values);
                var cmd = _connection.CreateCommand();
                cmd.CommandText = $"INSERT INTO {table}({keys}) VALUES({values}) ON DUPLICATE KEY UPDATE {duplicates}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("[SQL] Unable to insert or update this value: " + ex.Message + ", table: " + table + " value:" + newValue);
            }

            IsWorking = false;
        }

        public async Task<bool> UpdateAsync<T>(string table, string where, T newValue)
        {
            IsWorking = true;

            if (!IsOpen)
                return false;

            var isUpdated = false;

            try
            {
                var result = MapSerialize(newValue);

                for (var o = 0; o < result.Count; o++)
                {
                    var val = result.ElementAt(o);
                                
                    if (val.Value.GetType() == typeof(double) || val.Value.GetType() == typeof(float) || val.Value.GetType() == typeof(decimal) || val.Value.GetType() == typeof(int))
                    {
                        result[val.Key] = $"{val.Key}={val.Value.ToString().Replace(",", ".")}";
                    }
                    else
                    {
                        result[val.Key] = $"{val.Key}='{val.Value}'";
                    }
                }

                var values = string.Join(",", result.Values);
                var updateCmd = _connection.CreateCommand();
                updateCmd.CommandText = $"UPDATE {table} SET {values} WHERE {where}";
                await updateCmd.ExecuteNonQueryAsync();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"[SQL] Unable to update this value at where: {where}. Error: {ex.Message}\n{ex.StackTrace}.");
            }

            IsWorking = false;
            return isUpdated;
        }

        private async Task<List<T>> MapDeserialize<T>(DbDataReader reader)
        {
            IsWorking = true;

            var list = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var dict = new Dictionary<string, object>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var value = reader.GetValue(i);

                    if (IsJson(value.ToString()))
                    {
                        dict.Add(name, JsonConvert.DeserializeObject(value.ToString()));
                    }
                    else
                    {
                        dict.Add(name, reader.GetValue(i));
                    }
                }

                list.Add(dict);
            }

            reader.Close();
            IsWorking = false;
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(list));
        }

        private Dictionary<string, object> MapSerialize(object value) => JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(value));

        private bool IsJson(string value) => value.StartsWith("{") && value.EndsWith("}") || value.StartsWith("[") && value.EndsWith("]");
    }
}
