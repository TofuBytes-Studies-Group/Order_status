using Moq;
using Order_status.API.Controllers;
using Order_status.API.Services;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Order_status.Domain.Entities;
using Microsoft.Extensions.Logging;
using Order_status.Infrastructure.Repositories;
using Order_status.Infrastructure.Models;

namespace Order_status.Tests.API.Controllers
{
    public class OrderStatusControllerTest
    {
        [Fact]
        public async Task GetOrderStatusAsync_ValidOrderId_ShouldReturnOkAndTheOrderStatusAsString()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();
            var orderStatus = new OrderStatus
            {
                OrderId = orderId,
                CustomerName = "Test User",
                Status = Status.Accepted
            };

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
            var result = await controller.GetOrderStatusAsync(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(orderStatus.ToString(), returnValue);
        }

        [Fact]
        public async Task GetOrderStatusAsync_InvalidOrderId_ShouldReturnNotFound_WhenOrderStatusNotFoundExceptionIsThrown()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();
            mockRepo
                .Setup(repo => repo.GetOrderStatusAsync(orderId))
                .ThrowsAsync(new OrderStatusNotFoundException($"OrderStatus with OrderId {orderId} not found in database."));

            // Act
            var result = await controller.GetOrderStatusAsync(orderId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"OrderStatus with OrderId {orderId} not found in database.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetOrderStatusAsync_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();
            mockRepo
                .Setup(r => r.GetOrderStatusAsync(orderId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await controller.GetOrderStatusAsync(orderId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred. Please try again later.", statusCodeResult.Value);
        }

        [Theory]
        [InlineData(Status.Accepted)]
        [InlineData(Status.Rejected)]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_ValidInput_ShouldReturnOk(Status status)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();

            // Act
            var result = await controller.UpdateOrderStatusAsync(orderId, status);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            mockRepo.Verify(repo =>
                repo.UpdateOrderStatusAsync(orderId, status), Times.Once);
        }

        [Theory]
        [InlineData(Status.Accepted)]
        [InlineData(Status.Rejected)]
        [InlineData(Status.BeingPrepared)]
        [InlineData(Status.ReadyForPickUp)]
        [InlineData(Status.PickedUp)]
        [InlineData(Status.Delivered)]
        public async Task UpdateOrderStatusAsync_OrderIdNotFound_ShouldReturnNotFound(Status status)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();

            mockRepo
                .Setup(repo => repo.UpdateOrderStatusAsync(orderId, status))
                .ThrowsAsync(new OrderStatusNotFoundException($"OrderStatus with OrderId {orderId} not found in database."));

            // Act
            var result = await controller.UpdateOrderStatusAsync(orderId, status);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"OrderStatus with OrderId {orderId} not found in database.", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UnexpectedError_ShouldReturnInternalServerError()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<OrderStatusController>>();
            var mockRepo = new Mock<IOrderStatusRepository>();
            var service = new OrderStatusService(mockRepo.Object);
            var controller = new OrderStatusController(mockLogger.Object, service);

            var orderId = Guid.NewGuid();
            var newOrderStatus = Status.Delivered;

            mockRepo
                .Setup(repo => repo.UpdateOrderStatusAsync(orderId, newOrderStatus))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await controller.UpdateOrderStatusAsync(orderId, newOrderStatus);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred. Please try again later.", statusCodeResult.Value);
        }
    }
}
