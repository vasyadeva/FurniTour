using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClickController : ControllerBase
    {
        private readonly IClickService clickService;

        public ClickController(IClickService clickService)
        {
            this.clickService = clickService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddClick([FromBody] ClickModel model)
        {
            var state = await clickService.AddClick(model.itemId);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }
    }
}
