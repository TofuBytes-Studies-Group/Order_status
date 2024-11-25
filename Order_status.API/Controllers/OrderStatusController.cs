using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Order_status.API.Services;
using Order_status.Domain.Aggregates;
using Order_status.Domain.Entities;
using Order_status.Domain.Exceptions;

namespace Order_status.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStatusController : ControllerBase
    {
        private readonly ILogger<OrderStatusController> _logger;
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusController(ILogger<OrderStatusController> logger, IOrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderStatusService = orderStatusService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<string>> GetOrderStatusAsync(Guid orderId)
        {
            _logger.LogInformation("Fetching order status for order with orderId: {OrderId}", orderId);
            try
            {
                OrderStatus orderStatus = await _orderStatusService.GetOrderStatusAsync(orderId);
                _logger.LogInformation("Order status: {OrderStatus} fetched successfully for order with id: {OrderId}",
                    JsonConvert.SerializeObject(orderStatus), orderId);
                return Ok(orderStatus.ToString());
            }
            catch (OrderStatusNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order status not found for orderId: {OrderId}", orderId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching order status for orderId: {OrderId}", orderId);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPatch("update/{orderId}/{newOrderStatus}")]
        public async Task<ActionResult> UpdateOrderStatusAsync(Guid orderId, Status newOrderStatus)
        {
            _logger.LogInformation("Trying to update order status for order with orderId: {OrderId}", orderId);
            try
            {
                await _orderStatusService.UpdateOrderStatusAsync(orderId, newOrderStatus);
                _logger.LogInformation("Succesfully updated the order with {OrderId} to have status: {OrderStatus}", orderId, newOrderStatus);
                return Ok();
            }
            catch (OrderStatusNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order status not found for orderId: {OrderId}", orderId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to update the order status for orderId: {OrderId}", orderId);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
