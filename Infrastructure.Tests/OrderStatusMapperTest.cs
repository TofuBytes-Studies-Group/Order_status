using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Infrastructure.Mappers;
using Order_status.Infrastructure.Models;

namespace Infrastructure.Tests
{
    public class OrderStatusMapperTest
    {
        [Fact]
        public void ToDomain_ShouldReturnCorrectOrderStatusObject()
        {
            // Arrange
            var dto = new OrderStatusDTO()
            {
                Id = "1",
                OrderId = Guid.NewGuid(),
                CustomerUsername = "Test User",
                Status = Status.Accepted
            };

            // Act
            var domain = dto.ToDomain();

            // Assert
            Assert.NotNull(domain);
            Assert.Equal(dto.OrderId, domain.OrderId);
            Assert.Equal(dto.CustomerUsername, domain.CustomerUsername);
            Assert.Equal(dto.Status, domain.Status);
            Assert.NotEmpty(domain.StatusDescription);
            Assert.Equal("Your order has been accepted by the restaurant", domain.StatusDescription);
        }

        [Fact]
        public void ToDTO_ShouldReturnCorrectOrderStatusDTOObject()
        {
            // Arrange 
            var domain = new OrderStatus
            {
                OrderId = Guid.NewGuid(),
                CustomerUsername = "Test User",
                Status = Status.Accepted
            };

            // Act
            var dto = domain.ToDTO();

            // Assert
            Assert.NotNull(dto);
            Assert.Null(dto.Id);
            Assert.Equal(domain.OrderId, dto.OrderId);
            Assert.Equal(domain.CustomerUsername, dto.CustomerUsername);
            Assert.Equal(domain.Status, dto.Status);
        }
    }
}