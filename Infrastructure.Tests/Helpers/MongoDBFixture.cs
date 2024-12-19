using MongoDB.Driver;

namespace Infrastructure.Tests.Helpers
{
    public class MongoDBFixture : IDisposable
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        private IMongoClient _mongoClient;
        private IMongoDatabase _mongoDatabase;

        public MongoDBFixture()
        {
            var password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? "password";
            ConnectionString = $"mongodb://root:{password}@localhost:27017";
            DatabaseName = "test_db_for_mtogo_order_status_tests";
            CollectionName = "order_statuses";

            _mongoClient = new MongoClient(ConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(DatabaseName);

            _mongoDatabase.DropCollection(CollectionName);
        }

        public void Dispose()
        {
            _mongoDatabase.DropCollection(CollectionName);
        }
    }
}