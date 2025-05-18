using FurniTour.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService;
        private readonly IAuthService _authService;

        public LoyaltyController(ILoyaltyService loyaltyService, IAuthService authService)
        {
            _loyaltyService = loyaltyService;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserLoyalty()
        {
            var user = _authService.GetUser();
            if (user == null)
            {
                return Unauthorized("User not authenticated");
            }

            var loyalty = await _loyaltyService.GetUserLoyaltyAsync(user.Id);
            return Ok(loyalty);
        }

        [HttpGet("discount")]
        [Authorize]
        public async Task<IActionResult> GetUserDiscount()
        {
            var user = _authService.GetUser();
            if (user == null)
            {
                return Unauthorized("User not authenticated");
            }

            var discount = await _loyaltyService.GetUserDiscountAsync(user.Id);
            return Ok(new { discount = discount, discountPercent = discount * 100 });
        }
    }
}
