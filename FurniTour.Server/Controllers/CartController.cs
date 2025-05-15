using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api;
using FurniTour.Server.Models.Api.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartFurnitureService cartService;
        public CartController(ICartFurnitureService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet("getcart")]
        public IActionResult GetCart()
        {
            var cart = cartService.GetCartFurniture();
            return Ok(cart);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartRequest request)
        {
            var state = await cartService.AddToCartAsync(request.Id, request.Quantity);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFromCartAsync(int id)
        {
            var state = await cartService.RemoveFromCartAsync(id);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCartAsync([FromBody] UpdateCartRequestAI request)
        {
            var state = await cartService.UpdateCartAsync(request.Id, request.Quantity);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }


        #region AIEndpoints

        [HttpPost("addcopilot")]
        public async Task<IActionResult> AddToCartCopilot([FromBody] AddToCartRequestAI request)
        {
            var state = await cartService.AddToCartCopilot(request.Id, request.Quantity, request.UserID);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpDelete("deletecopilot/{id}")]
        public async Task<IActionResult> DeleteFromCartCopilot(int id, [FromQuery] string userID)
        {
            var state = await cartService.RemoveFromCartCopilot(id, userID);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpPost("updatecopilot")]
        public async Task<IActionResult> UpdateCartCopilot([FromBody] UpdateCartRequestAI request)
        {
            var state = await cartService.UpdateCartCopilot(request.Id, request.Quantity, request.UserID);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpGet("getcartcopilot/{userID}")]
        public async Task<IActionResult> GetCartCopilot(string userID)
        {
            var cart = await cartService.GetCartFurnitureCopilot(userID);
            if (cart == null)
            {
                return NotFound();
            }
            var serialized = System.Text.Json.JsonSerializer.Serialize(cart);

            return Content(serialized, "text/plain");
        }

        #endregion
    }
}
