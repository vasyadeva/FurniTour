using FurniTour.Server.Constants;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api;
using FurniTour.Server.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        [Authorize]
        public IActionResult MyOrders()
        {
            var orders = orderService.MyOrders();
            return Ok(orders);
        }

        [HttpGet("adminorders")]
        [Authorize(Roles = Roles.Administrator)]
        public IActionResult AdminOrders()
        {
            var orders = orderService.AdminOrders();
            return Ok(orders);
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderModel orderModel)
        {
            var state = await orderService.Order(orderModel);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderAsync([FromBody] UpdateResponse response)
        {
            if (!OrderStatesConst.ValidStates.Contains(response.state))
            {
                return BadRequest("Invalid order state.");
            }

            var state = await orderService.ChangeOrderStateAsync(response.id, response.state);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

    }

}
