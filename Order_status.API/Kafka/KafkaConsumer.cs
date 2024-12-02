using Confluent.Kafka;
using Newtonsoft.Json;
using Order_status.API.Kafka.DTOs;
using Order_status.API.Services;

namespace Order_status.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IOrderStatusService _orderStatusService;

        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger, IOrderStatusService orderStatusService)
        {
            _configuration = configuration;
            _logger = logger;
            _orderStatusService = orderStatusService;

            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                //TODO
                GroupId = "groupId",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO: It will consume from the Order microservice and then use the order to create the order status
            _consumer.Subscribe("order.accepted");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer is running.");
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5)); // Is here to not block swagger 

                        if (consumeResult != null)
                        {
                            var message = consumeResult.Message.Value;
                            _logger.LogInformation($"Received Message: {message}, Key: {consumeResult.Message.Key}");

                            var orderDto = JsonConvert.DeserializeObject<OrderDTO>(message);
                            if (orderDto != null)
                            {
                                await _orderStatusService.SetOrderStatusAsAcceptedAsync(orderDto);
                            }
                        }

                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Error consuming Kafka message: {ex.Message}");
                    }
                    // Adding a delay which is non-blocking and will stop, if the application is shutting down
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer stopping gracefully.");
            }
            finally
            {
                _consumer.Close();
                _logger.LogInformation("Kafka consumer has stopped.");
            }
        }
    }
}
