using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Order_status.API.Services;
using Order_status.Domain.Aggregates;

namespace Order_status.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStatusController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<OrderStatusController> _logger;
        private readonly OrderStatusService _orderStatusService;

        public OrderStatusController(ILogger<OrderStatusController> logger, OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderStatusService = orderStatusService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _orderStatusService.DoStuff();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<string>> GetCartAsync(Guid orderId)
        {
            _logger.LogInformation("Fetching order status for order with orderId: {OrderId}", orderId);

            OrderStatus orderStatus = await _orderStatusService.GetOrderStatusAsync(orderId);
            _logger.LogInformation("Order status: {OrderStatus} fetched successfully for order with id: {OrderId}", 
                JsonConvert.SerializeObject(orderStatus), orderId);

            return Ok(orderStatus.ToString());
        }

    }
}
