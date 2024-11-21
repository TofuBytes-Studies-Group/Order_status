using Microsoft.AspNetCore.Mvc;
using Order_status.API.Services;

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

    }
}
