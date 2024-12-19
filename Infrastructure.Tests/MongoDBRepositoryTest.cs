using Infrastructure.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Order_status.Domain.Entities;
using Order_status.Domain.Exceptions;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;

namespace Infrastructure.Test
{
    public class MongoDBRepositoryTest : IClassFixture<MongoDBFixture>
    {
        private readonly MongoDBRepository _repository;
        private readonly MongoDBFixture _mongoDBFixture;

        public MongoDBRepositoryTest(MongoDBFixture mongoDBFixture)
        {
            _mongoDBFixture = mongoDBFixture;
            var loggerMock = new Mock<ILogger<MongoDBRepository>>();

            var mongoDBConnection = Options.Create(new MongoDBConnection
            {
                ConnectionString = _mongoDBFixture.ConnectionString,
                DatabaseName = _mongoDBFixture.DatabaseName,
                CollectionName = _mongoDBFixture.CollectionName
            });

            _repository = new MongoDBRepository(mongoDBConnection, loggerMock.Object);
        }

        [Fact]
        public async Task CreateOrderStatusAsync_ValidOrderStatus_ShouldInsertOrderStatus()
        {
            // Arrange
            var orderStatus = new OrderStatusDTO
            {
                OrderId = Guid.NewGuid(),
                CustomerUsername = "Test User",
                Status = Status.Accepted
            };

            // Act 
            await _repository.CreateOrderStatusAsync(orderStatus);

            // Assert
            var result = await _repository.GetOrderStatusAsync(orderStatus.OrderId);
            Assert.NotNull(result);
            Assert.Equal(orderStatus.OrderId, result.OrderId);
            Assert.Equal(orderStatus.Status, result.Status);
        }

        [Fact]
        public async Task CreateOrderStatusAsync_DuplicateOrderStatus_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderStatus = new OrderStatusDTO
            {
                OrderId = orderId,
                CustomerUsername = "Test User",
                Status = Status.Accepted
            };
            await _repository.CreateOrderStatusAsync(orderStatus);

            // Act & Assert 
            var exception = await Assert.ThrowsAsync<MongoWriteException>(
                () => _repository.CreateOrderStatusAsync(orderStatus)
            );
        }

        [Fact]
        public async Task GetOrderStatusAsync_OrderNotFoundInDatabase_ShouldThrowOrderStatusNotFoundException()
        {
            // Arrange
            var nonExistentOrderId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderStatusNotFoundException>(
                () => _repository.GetOrderStatusAsync(nonExistentOrderId)
            );
            Assert.Equal($"OrderStatus with OrderId {nonExistentOrderId} not found in database.", exception.Message);
        }

        [Theory]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_ValidOrderStatus_ShouldUpdateOrderStatus(Status newStatus)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderStatus = new OrderStatusDTO
            {
                OrderId = orderId,
                CustomerUsername = "Test User",
                Status = Status.Accepted
            };
            await _repository.CreateOrderStatusAsync(orderStatus);

            // Act
            await _repository.UpdateOrderStatusAsync(orderId, newStatus);

            // Assert
            var result = await _repository.GetOrderStatusAsync(orderId);
            Assert.NotNull(result);
            Assert.Equal(newStatus, result.Status);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_OrderStatusNotFound_ShouldThrowOrderStatusNotFoundException()
        {
            // Arrange
            var nonExistentOrderId = Guid.NewGuid();
            var newStatus = Status.ReadyForPickUp;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderStatusNotFoundException>(
                () => _repository.UpdateOrderStatusAsync(nonExistentOrderId, newStatus)
            );
            Assert.Equal($"OrderStatus with OrderId {nonExistentOrderId} not found in database.", exception.Message);
        }
    }
}