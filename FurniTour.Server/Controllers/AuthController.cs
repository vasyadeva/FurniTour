﻿using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FurniTour.Server.Models.Api;
using Microsoft.AspNetCore.Identity;

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
            var state = await authService.SignInAsync(loginModel);
            if (state.IsNullOrEmpty())
            {
                return Ok(new Response { IsSuccess = true, Message = "Успішно увійшли в систему" });
            }
            else
            {
                return BadRequest(new Response { IsSuccess = false, Message = state });
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
                return Ok(new Response { IsSuccess = true, Message = "Успішно зареєстровані" });
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

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await authService.GetProfile();
            return Ok(profile);
        }

        [HttpGet("credentials")]
        [Authorize]
        public IActionResult Credentials()
        {
            var credentials = authService.GetCredentials();
            return Ok(credentials);
        }

        [HttpGet("master/getall")]
        public async Task<IActionResult> GetAllMasters([FromServices] UserManager<IdentityUser> userManager)
        {
            var masters = await userManager.GetUsersInRoleAsync("Master");
            
            var mastersList = masters.Select(m => new 
            {
                id = m.Id,
                userName = m.UserName
            }).ToList();
            
            return Ok(mastersList);
        }

        [HttpPost("changeprofile")]
        [Authorize]
        public IActionResult ChangeProfile([FromBody] ChangeProfileModel model)
        {
            var state =  authService.ChangeProfile(model);
            if (state.IsNullOrEmpty())
            {
                return Ok(new Response { IsSuccess = true, Message = "Профіль успішно змінено" });
            }
            return BadRequest(new Response { IsSuccess = false, Message = state });
        }
    }
}
