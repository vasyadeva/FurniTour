using FurniTour.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public class AddToCartRequest
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartRequest request)
        {
            await cartService.AddToCartAsync(request.Id, request.Quantity);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFromCartAsync(int id)
        {
            await cartService.RemoveFromCartAsync(id);
            return Ok();
        }
    }
}
