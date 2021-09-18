using Newtonsoft.Json.Linq;
using System;

namespace Average.Server.Database
{
    public class DbContextFactory
    {
        private readonly string _connectionString;
        private readonly JObject _baseConfig;

        public DbContextFactory()
        {
            _baseConfig = SDK.Server.Configuration.ParseToObj("config.json");
            _connectionString = $"Server={(string)_baseConfig["MySQL"]["Host"]};Port={(int)_baseConfig["MySQL"]["Port"]};Database={(string)_baseConfig["MySQL"]["Database"]};Uid={(string)_baseConfig["MySQL"]["Username"]};Pwd={(string)_baseConfig["MySQL"]["Password"]};";
        }

        public DatabaseContext CreateDbContext() => new DatabaseContext(_connectionString);
    }
}
