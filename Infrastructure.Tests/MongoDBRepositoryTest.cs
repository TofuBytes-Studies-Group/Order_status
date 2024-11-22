using Moq;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;
using Order_status.Domain.Entities;

namespace Infrastructure.Tests
{
    public class MongoDBRepositoryTest
    {
        private readonly Mock<IMongoCollection<OrderStatusDTO>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly IOptions<MongoDBConnection> _mockOptions;
        private readonly MongoDBRepository _repository;

        public MongoDBRepositoryTest()
        {
            // Mock MongoDB connection settings
            var mongoConnection = new MongoDBConnection
            {
                ConnectionString = "mongodb://admin:password@mongodb:27017",
                DatabaseName = "TestDatabase",
                CollectionName = "TestCollection"
            };
            _mockOptions = Options.Create(mongoConnection);

            // Mock MongoDB client, database, and collection
            _mockCollection = new Mock<IMongoCollection<OrderStatusDTO>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();

            _mockClient.Setup(c => c.GetDatabase(mongoConnection.DatabaseName, null))
                       .Returns(_mockDatabase.Object);

            _mockDatabase.Setup(d => d.GetCollection<OrderStatusDTO>(mongoConnection.CollectionName, null))
                         .Returns(_mockCollection.Object);

            // Initialize repository with mocked dependencies
            _repository = new MongoDBRepository(_mockOptions);
        }

        [Fact]
        public async Task CreateOrderStatusAsync_InsertsDocument()
        {
            // Arrange
            var orderStatus = new OrderStatusDTO
            {
                Id = "1",
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Status = Status.Accepted
            };

            _mockCollection.Setup(c => c.InsertOneAsync(orderStatus, null, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _repository.CreateOrderStatusAsync(orderStatus);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(orderStatus, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrderStatusAsync_ReturnsCorrectDocument()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedOrderStatus = new OrderStatusDTO
            {
                Id = "1",
                OrderId = orderId,
                CustomerName = "Test Customer",
                Status = Status.PickedUp
            };

            var mockCursor = new Mock<IAsyncCursor<OrderStatusDTO>>();
            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                      .Returns(true)
                      .Returns(false);
            mockCursor.Setup(c => c.Current).Returns([expectedOrderStatus]);

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<OrderStatusDTO>>(),
                                                   It.IsAny<FindOptions<OrderStatusDTO, OrderStatusDTO>>(),
                                                   It.IsAny<CancellationToken>()))
                           .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetOrderStatusAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrderStatus.OrderId, result.OrderId);
            Assert.Equal(expectedOrderStatus.CustomerName, result.CustomerName);
            _mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<OrderStatusDTO>>(),
                                                    It.IsAny<FindOptions<OrderStatusDTO, OrderStatusDTO>>(),
                                                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesDocument()
        {
            // Arrange
            var orderStatus = new OrderStatusDTO
            {
                Id = "1",
                OrderId = Guid.NewGuid(),
                CustomerName = "Updated Customer",
                Status = Status.Delivered
            };

            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockReplaceResult.Setup(r => r.MatchedCount).Returns(1);

            _mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<OrderStatusDTO>>(),
                orderStatus,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceResult.Object);

            // Act
            await _repository.UpdateOrderStatusAsync(orderStatus);

            // Assert
            _mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<OrderStatusDTO>>(),
                orderStatus,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
