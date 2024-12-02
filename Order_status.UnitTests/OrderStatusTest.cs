using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Domain.Exceptions;
using Order_status.UnitTests.Helpers;

namespace Order_status.UnitTests
{
    public class OrderStatusTest
    {

        [Theory]
        [ClassData(typeof(StatusTestData))]
        public void GetStatusDescription_ShouldReturnCorrectOrderStatusDescription(Status status, string expectedDescription)
        {
            // Arrange
            var orderStatus = new OrderStatus
            {
                OrderId = Guid.NewGuid(),
                CustomerUsername = "TestUser1",
                Status = status
            };

            // Act
            var description = orderStatus.StatusDescription;

            // Assert
            Assert.Equal(expectedDescription, description);
        }
         
        [Fact]
        public void GetStatusDescription_ThrowsUnknownStatusException()
        {
            // Arrange
            var orderStatus = new OrderStatus 
            {
                OrderId = Guid.NewGuid(),
                CustomerUsername = "TestUser1",
                Status = (Status)999
            };

            // Act & Assert
            Assert.Throws<UnknownStatusException>(() => _ = orderStatus.StatusDescription);
        }

        [Theory]
        [ClassData(typeof(StatusTestData))]
        public void ToString_ShouldReturnCorrectToString(Status status, string expectedDescription)
        {
            // Arrange
            var customerUsername = "TestUser1";
            var orderStatus = new OrderStatus
            {
                OrderId = Guid.NewGuid(), 
                CustomerUsername = customerUsername,
                Status = status
            };

            var expectedToString = $"Hi {customerUsername}! {expectedDescription}.";

            // Act
            var toString = orderStatus.ToString();

            // Assert
            Assert.Equal(expectedToString, toString);
        }

    }
}