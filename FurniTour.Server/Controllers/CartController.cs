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

        [HttpPost("add")]
        public async Task<IActionResult> AddToCartAsync([FromBody] int furnitureId, int quantity)
        {
            await cartService.AddToCartAsync(furnitureId, quantity);
            return Ok();
        }
    }
}
