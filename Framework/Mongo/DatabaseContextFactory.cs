using Average.Server.Framework.Extensions;
using Average.Server.Framework.Utilities;
using MongoDB.Driver;

namespace Average.Server.Framework.Mongo
{
    internal class DatabaseContextFactory
    {
        private readonly MongoClient _dbClient;
        private readonly IMongoDatabase _database;

        public DatabaseContextFactory()
        {
            var baseConfig = FileUtility.ReadFileFromRootDir("config.json").ToJObject();
            var mongoDb = baseConfig["MongoDb"];

            _dbClient = new MongoClient($"mongodb://{(string)mongoDb["Host"]}:{(int)mongoDb["Port"]}");
            _database = _dbClient.GetDatabase((string)mongoDb["Database"]);
        }

        internal IMongoDatabase Database => _database;
    }
}
