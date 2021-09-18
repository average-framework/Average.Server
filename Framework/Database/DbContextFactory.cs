using Average.Server.Framework.Extensions;
using Average.Server.Framework.Utilities;
using Newtonsoft.Json.Linq;

namespace Average.Server.Framework.Database
{
    public class DbContextFactory
    {
        private readonly string _connectionString;
        private readonly JObject _baseConfig;

        public DbContextFactory()
        {
            _baseConfig = FileUtility.ReadFileFromRootDir("config.json").ToJObject();
            _connectionString = $"Server={(string)_baseConfig["MySQL"]["Host"]};Port={(int)_baseConfig["MySQL"]["Port"]};Database={(string)_baseConfig["MySQL"]["Database"]};Uid={(string)_baseConfig["MySQL"]["Username"]};Pwd={(string)_baseConfig["MySQL"]["Password"]};";
        }

        public DatabaseContext CreateDbContext() => new DatabaseContext(_connectionString);
    }
}
