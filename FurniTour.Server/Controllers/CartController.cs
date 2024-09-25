using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api;
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
        public async Task<IActionResult> UpdateCartAsync([FromBody] UpdateCartRequest request)
        {
            var state = await cartService.UpdateCartAsync(request.Id, request.Quantity);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }
    }
}
