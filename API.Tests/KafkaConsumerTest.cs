using Confluent.Kafka;
using Moq;
using Newtonsoft.Json;
using Order_status.API.Kafka;
using Order_status.API.Kafka.DTOs;
using Order_status.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Order_status.Tests.API.Controllers
{
    public class KafkaConsumerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldProcesMessageCorrect()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<KafkaConsumer>>();
            var mockOrderStatusService = new Mock<IOrderStatusService>();
            var mockConsumer = new Mock<IConsumer<string, string>>();

            mockConfiguration.Setup(config => config["Kafka:BootstrapServers"]).Returns("localhost:9092");

            var orderId = Guid.NewGuid();
            var sampleOrderDto = new OrderDTO { Id = orderId, CustomerName = "Test User" };
            var sampleMessage = JsonConvert.SerializeObject(sampleOrderDto);

            mockConsumer
                .Setup(consumer => consumer.Consume(It.IsAny<TimeSpan>()))
                .Returns(new ConsumeResult<string, string>
                {
                    Message = new Message<string, string>
                    {
                        Key = "key",
                        Value = sampleMessage
                    }
                });

            var kafkaConsumer = new KafkaConsumer(
                mockConfiguration.Object,
                mockLogger.Object,
                mockOrderStatusService.Object
            );

            // Override the private _consumer field with the mocked consumer
            typeof(KafkaConsumer)
                .GetField("_consumer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(kafkaConsumer, mockConsumer.Object);

            // Act
            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000); // Stop after 2 seconds to simulate limited processing
            await kafkaConsumer.StartAsync(cts.Token);

            // Assert
            mockOrderStatusService.Verify(
                service => service.SetOrderStatusAsAcceptedAsync(It.Is<OrderDTO>(dto =>
                    dto.Id == orderId &&
                    dto.CustomerName == "Test User"
                )),
                Times.Once
            );

            mockConsumer.Verify(consumer => consumer.Subscribe("topic"), Times.Once);
            mockConsumer.Verify(consumer => consumer.Consume(It.IsAny<TimeSpan>()), Times.AtLeastOnce);
        }
    }
}