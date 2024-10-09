using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService profileService;
        public ProfileController(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpGet("getmaster/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMaster(string id)
        {
            var profile = await profileService.GetMasterProfile(id);
            return Ok(profile);
        }

        [HttpPost("addmaster")]
        [Authorize]
        public IActionResult AddMaster([FromBody] AddMasterReview model)
        {
            var state = profileService.MakeMasterReview(model);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }
    }
}
