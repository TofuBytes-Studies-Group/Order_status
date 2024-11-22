using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Order_status.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Order_status.Infrastructure.Repositories
{
    public class MongoDBRepository : IOrderStatusRepository
    {
        private readonly IMongoCollection<OrderStatusDTO> _collection;
        private readonly ILogger<MongoDBRepository> _logger;

        public MongoDBRepository(IOptions<MongoDBConnection> mongoConnection, ILogger<MongoDBRepository> logger)
        {
            _logger = logger;

            // Needed to serialize the guid 
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var mongoClient = new MongoClient(
                mongoConnection.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                mongoConnection.Value.DatabaseName);

            _collection = mongoDatabase.GetCollection<OrderStatusDTO>(
                mongoConnection.Value.CollectionName);

            _logger.LogInformation("Successfully connected to MongoDB: Database: {DatabaseName}, Collection: {CollectionName}",
                    mongoConnection.Value.DatabaseName, mongoConnection.Value.CollectionName);
        }

        public async Task CreateOrderStatusAsync(OrderStatusDTO orderStatus)
        {
            try
            {
                await _collection.InsertOneAsync(orderStatus);
                _logger.LogInformation("Successfully inserted order status for order with id: {OrderId}", orderStatus.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting order status for order with id: {OrderId}", orderStatus.OrderId);
                throw;
            }
        }

        public async Task<OrderStatusDTO> GetOrderStatusAsync(Guid orderId)
        {
            return await _collection.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();
        }

        public async Task UpdateOrderStatusAsync(OrderStatusDTO orderStatus)
        {
            await _collection.ReplaceOneAsync(o => o.OrderId == orderStatus.OrderId, orderStatus);
        }
    }
}
