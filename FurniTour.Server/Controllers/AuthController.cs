using FurniTour.Server.Interfaces;
using FurniTour.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController (IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] LoginModel loginModel)
        {
            var state = authService.SignInAsync(loginModel);
            if (state.Result)
            {
                return Ok(new Response(true, "Signed in successfully"));
            }
            else
            {
                return BadRequest(new Response(false, "Invalid credentials"));
            }
        }

        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var userClaims = User.Claims.Select(x => new UserClaim(x.Type, x.Value)).ToList();
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
            if (state.Result)
            {
                return Ok(new Response(true, "Registered successfully"));
            }
            else
            {
                return BadRequest(new Response(false, "Failed to register"));
            }
        }

        [HttpGet("getrole")]
        public IActionResult GetUserRole()
        {
            var role = authService.GetUserRole();
            return Ok(new { role });
        }


    }

    public record Response(bool IsSuccess, string Message);
    public record UserClaim(string Type, string Value);
}
