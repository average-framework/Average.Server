using System;
using System.Collections.Generic;
using static Average.Server.Framework.ServerAPI;

namespace Average.Server.DataModels
{
    public class StorageItemData
    {
        public string UniqueId { get; }
        public string Name { get; set; }
        public int Count { get; set; }

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public StorageItemData()
        {
            UniqueId = RandomString();
        }

        public StorageItemData(string name, int count)
        {
            UniqueId = RandomString();
            Name = name;
            Count = count;
        }

        public StorageItemData(string name, int count, Dictionary<string, object> overrideData)
        {
            UniqueId = RandomString();
            Name = name;
            Count = count;
            Data = overrideData;
        }

        public T GetData<T>(string key)
        {
            if (Data.ContainsKey(key))
            {
                return (T)Convert.ChangeType(Data[key], typeof(T));
            }
            else
            {
                return default;
            }
        }

        public void SetData(string key, object value)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = value;
            }
            else
            {
                Data.Add(key, value);
            }
        }
    }
}