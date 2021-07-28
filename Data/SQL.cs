using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using SDK.Server.Managers;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Data
{
    public class SQL : ISQL
    {
        MySqlConnection connection;
        SQLConnection connectionInfo;

        Logger logger;

        public bool IsOpen { get; set; }
        public bool IsWorking { get; set; }

        public SQL(Logger logger, SQLConnection connectionInfo)
        {
            this.logger = logger;
            this.connectionInfo = connectionInfo;

            try
            {
                connection = new MySqlConnection($"SERVER={connectionInfo.Host};PORT={connectionInfo.Port};DATABASE={connectionInfo.Database};UID={connectionInfo.Username};PASSWORD={connectionInfo.Password};CHARSET=utf8;");
                connection.Open();

                IsOpen = true;

                logger.Info($"SQL ready");
            }
            catch (MySqlException ex)
            {
                logger.Error("Unable to initialize mysql connection.");
            }
        }

        public async Task Wait()
        {
            while (IsWorking)
            {
                await Task.Delay(0);
            }
        }

        public async Task<List<T>> GetAllAsync<T>(string table, Func<T, bool> predicate)
        {
            IsWorking = true;

            if (!IsOpen)
            {
                return new List<T>();
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table}";
            var reader = await cmd.ExecuteReaderAsync();
            var result = await MapDeserialize<T>(reader);
            var results = new List<T>();

            for (int i = 0; i < result.Count; i++)
            {
                if (predicate.Invoke(result[i]))
                {
                    results.Add(result[i]);
                }
            }

            IsWorking = false;
            return results;
        }

        public async Task<bool> ExistsAsync<T>(string table, Func<T, bool> predicate)
        {
            IsWorking = true;

            if (!IsOpen)
            {
                return false;
            }

            var cmd = connection.CreateCommand();
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

        public async Task<bool> DeleteAllAsync<T>(string table, Func<T, bool> predicate)
        {
            IsWorking = true;

            if (!IsOpen)
            {
                return false;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table}";
            var reader = await cmd.ExecuteReaderAsync();
            var result = await MapDeserialize<T>(reader);
            var isDeleted = false;

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].GetType().GetInterfaces().ToList().Find(x => x == typeof(IDataDeletable)) != null)
                {
                    if (predicate.Invoke(result[i]))
                    {
                        var data = result[i] as IDataDeletable;
                        var delCmd = connection.CreateCommand();
                        delCmd.CommandText = $"DELETE FROM {table} WHERE Id=\"{data.Id}\"";
                        await delCmd.ExecuteNonQueryAsync();
                        isDeleted = true;
                    }
                }
                else
                {
                    var temp = (T)Activator.CreateInstance(typeof(T));
                    logger.Error($"[SQL] Unable to delete this value because the {temp.GetType().Name} type does not inherits from IDataDeletable");
                    return false;
                }
            }

            IsWorking = false;
            return isDeleted;
        }

        public async Task InsertAsync(string table, object value)
        {
            IsWorking = true;

            if (!IsOpen)
            {
                return;
            }

            try
            {
                var result = MapSerialize(value);
                var cmd = connection.CreateCommand();

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
                logger.Error("[SQL] Unable to insert this value: " + ex.Message);
            }

            IsWorking = false;
        }

        public async Task InsertOrUpdateAsync(string table, object newValue)
        {
            if (!IsOpen)
            {
                return;
            }

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
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"INSERT INTO {table}({keys}) VALUES({values}) ON DUPLICATE KEY UPDATE {duplicates}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.Error("[SQL] Unable to insert or update this value: " + ex.Message);
            }

            IsWorking = false;
        }

        public async Task<bool> UpdateAsync<T>(string table, Func<T, bool> predicate, T newValue)
        {
            IsWorking = true;

            if (!IsOpen)
            {
                return false;
            }

            var isUpdated = false;

            try
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT * FROM {table}";
                var reader = await cmd.ExecuteReaderAsync();
                var results = await MapDeserialize<T>(reader);

                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].GetType().GetInterfaces().ToList().Find(x => x == typeof(IDataDeletable)) != null)
                    {
                        if (predicate.Invoke(results[i]))
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

                            var data = results[i] as IDataDeletable;
                            var values = string.Join(",", result.Values);
                            var updateCmd = connection.CreateCommand();
                            updateCmd.CommandText = $"UPDATE {table} SET {values} WHERE Id=\"{data.Id}\"";
                            await updateCmd.ExecuteNonQueryAsync();
                            isUpdated = true;
                        }
                    }
                    else
                    {
                        var temp = (T)Activator.CreateInstance(typeof(T));
                        logger.Error($"[SQL] Unable to update this value because the {temp.GetType().Name} type does not inherits from IDataUpdatable");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("[SQL] Unable to update this value: " + ex.Message);
            }

            IsWorking = false;
            return isUpdated;
        }

        async Task<List<T>> MapDeserialize<T>(DbDataReader reader)
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

        Dictionary<string, object> MapSerialize(object value) => JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(value));

        bool IsJson(string value) => value.StartsWith("{") && value.EndsWith("}") || value.StartsWith("[") && value.EndsWith("]");
    }
}
