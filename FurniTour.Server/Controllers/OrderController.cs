using FurniTour.Server.Constants;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderFurnitureService orderService;
        public OrderController(IOrderFurnitureService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet("myorders")]
        public IActionResult MyOrders()
        {
            var orders = orderService.MyOrders();
            return Ok(orders);
        }

        [HttpGet("adminorders")]
        public IActionResult AdminOrders()
        {
            var orders = orderService.AdminOrders();
            return Ok(orders);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderModel orderModel)
        {
            await orderService.Order(orderModel);
            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrderAsync([FromBody] UpdateResponse response)
        {
            if (!OrderStatesConst.ValidStates.Contains(response.state))
            {
                return BadRequest("Invalid order state.");
            }

            await orderService.ChangeOrderStateAsync(response.id, response.state);
            return Ok();
        }

    }

    public class UpdateResponse
    {
        public int id { get; set; }
        public int state { get; set; }
    }

}
