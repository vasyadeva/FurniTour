using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly UserManager<IdentityUser> userManager;

        public MasterController(IAuthService authService, UserManager<IdentityUser> userManager)
        {
            this.authService = authService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMasters()
        {
            var masters = await userManager.GetUsersInRoleAsync("Master");
            
            var mastersList = masters.Select(m => new 
            {
                id = m.Id,
                username = m.UserName
            }).ToList();
            
            return Ok(mastersList);
        }
    }
}
