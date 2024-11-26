using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Order_status.API.Kafka.DTOs;
using Order_status.API.Services;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Domain.Exceptions;
using Order_status.Infrastructure.Models;
using Order_status.Infrastructure.Repositories;

namespace Order_status.Tests.API.Controllers
{
    public class OrderStatusServiceTest
    {
        [Fact]
        public async void GetOrderStatusAsync_ValidOrderId_ShouldReturnOrderStatusForSpecificOrderId()
        {
            // Arrange 
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderId = Guid.NewGuid();
            var orderStatusDTO = new OrderStatusDTO
            {
                OrderId = orderId,
                CustomerName = "Test User",
                Status = Status.Accepted
            };

            mockRepo
                .Setup(r => r.GetOrderStatusAsync(orderId))
                .ReturnsAsync(orderStatusDTO);

            // Act
            var result = await service.GetOrderStatusAsync(orderId);

            // Assert
            mockRepo.Verify(r => r.GetOrderStatusAsync(orderId), Times.Once);

            Assert.NotNull(result);
            Assert.IsType<OrderStatus>(result);
            Assert.Equal(orderStatusDTO.OrderId, result.OrderId);
            Assert.Equal(orderStatusDTO.CustomerName, result.CustomerName);
            Assert.Equal(orderStatusDTO.Status, result.Status);
        }

        [Fact]
        public async void GetOrderStatusAsync_InValidOrderId_ThrowsOrderNotFoundException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderId = Guid.NewGuid();

            mockRepo
                .Setup(repo => repo.GetOrderStatusAsync(orderId))
                .ThrowsAsync(new OrderStatusNotFoundException($"OrderStatus with OrderId {orderId} not found in database."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderStatusNotFoundException>(() => service.GetOrderStatusAsync(orderId));
            Assert.Equal($"OrderStatus with OrderId {orderId} not found in database.", exception.Message);

            mockRepo.Verify(repo => repo.GetOrderStatusAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task SetOrderStatusAsAcceptedAsync_ShouldSaveOrderStatusWithAcceptedStatus()
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderDto = new OrderDTO
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test User"
            };

            mockRepo
                .Setup(repo => repo.CreateOrderStatusAsync(It.IsAny<OrderStatusDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            await service.SetOrderStatusAsAcceptedAsync(orderDto);

            // Assert
            mockRepo.Verify(repo =>
                repo.CreateOrderStatusAsync(It.Is<OrderStatusDTO>(dto =>
                    dto.OrderId == orderDto.Id &&
                    dto.CustomerName == orderDto.CustomerName &&
                    dto.Status == Status.Accepted
                )), Times.Once);
        }

        [Fact]
        public async Task SetOrderStatusAsAcceptedAsync_OrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderDto = new OrderDTO
            {
                Id = Guid.Empty, // Invalid ID
                CustomerName = "Test User"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.SetOrderStatusAsAcceptedAsync(orderDto)
            );
            Assert.Equal("The order must have a valid order id and customer name", exception.Message);
        }

        [Fact]
        public async Task SetOrderStatusAsAcceptedAsync_CustomerNameIsNullOrEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderDto = new OrderDTO
            {
                Id = Guid.NewGuid(),
                CustomerName = string.Empty // Invalid CustomerName
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.SetOrderStatusAsAcceptedAsync(orderDto)
            );
            Assert.Equal("The order must have a valid order id and customer name", exception.Message);
        }

        [Fact]
        public async Task SetOrderStatusAsAcceptedAsync_CustomerNameIsNull_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderDto = new OrderDTO
            {
                Id = Guid.NewGuid(),
                CustomerName = null // Invalid CustomerName
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.SetOrderStatusAsAcceptedAsync(orderDto)
            );
            Assert.Equal("The order must have a valid order id and customer name", exception.Message);
        }

        [Theory]
        [InlineData(Status.Accepted)]
        [InlineData(Status.Rejected)]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_OrderIdFoundInDatabase_ShoulUpdateCorrectOrderStatusWithCorrectStatus(Status status)
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderId = Guid.NewGuid();

            mockRepo
                .Setup(repo => repo.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<Status>()))
                .Returns(Task.CompletedTask);

            // Act
            await service.UpdateOrderStatusAsync(orderId, status);

            // Assert
            mockRepo.Verify(repo =>
                repo.UpdateOrderStatusAsync(It.Is<Guid>(id => id == orderId), It.Is<Status>(s => s == status)),
                Times.Once);
        }

        [Theory]
        [InlineData(Status.Accepted)]
        [InlineData(Status.Rejected)]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_OrderIdIsEmpty_ShouldThrowArgumentException(Status status)
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderId = Guid.Empty;

            mockRepo
                .Setup(repo => repo.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<Status>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.UpdateOrderStatusAsync(orderId, status)
            );
            Assert.Equal("The order must have a valid order id", exception.Message);
        }

        [Theory]
        [InlineData(Status.Accepted)]
        [InlineData(Status.Rejected)]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_OrderIdNotFoundInDatabase_ShouldThrowNotFoundException(Status status)
        {
            // Arrange
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);

            var orderId = Guid.NewGuid();

            mockRepo
                .Setup(repo => repo.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<Status>()))
                .ThrowsAsync(new OrderStatusNotFoundException($"OrderStatus with OrderId {orderId} not found in database."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderStatusNotFoundException>(() => service.UpdateOrderStatusAsync(orderId, status));
            Assert.Equal($"OrderStatus with OrderId {orderId} not found in database.", exception.Message);

            mockRepo.Verify(repo =>
                repo.UpdateOrderStatusAsync(It.Is<Guid>(id => id == orderId), It.Is<Status>(s => s == status)),
                Times.Once);
        }
    }
}