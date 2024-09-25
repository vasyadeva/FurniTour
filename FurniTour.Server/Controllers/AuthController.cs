using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FurniTour.Server.Models.Api;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] LoginModel loginModel)
        {
            var state = authService.SignInAsync(loginModel);
            if (state.Result.IsNullOrEmpty())
            {
                return Ok(new Response { IsSuccess = true, Message = "Signed in successfully" });
            }
            else
            {
                return BadRequest(new Response { IsSuccess = false, Message = state.Result });
            }
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var userClaims = User.Claims.Select(x => new UserClaim { Type = x.Type, Value = x.Value }).ToList();
            if (userClaims.IsNullOrEmpty())
            {
                return Ok();
            }
            return Ok(userClaims);
        }

        [Authorize]
        [HttpGet("signout")]
        public async Task SignOutAsync()
        {
            authService.SignOut();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel registerModel)
        {
            var state = authService.RegisterAsync(registerModel);
            if (state.Result.IsNullOrEmpty())
            {
                return Ok(new Response { IsSuccess = true, Message = "Registered successfully" });
            }
            else
            {
                return BadRequest(new Response { IsSuccess = false, Message = state.Result });
            }
        }

        [HttpGet("getrole")]
        [Authorize]
        public IActionResult GetUserRole()
        {
            var role = authService.GetUserRole();
            return Ok(new { role });
        }
    }
}
