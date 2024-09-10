using FurniTour.Server.Interfaces;
using FurniTour.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FurniTour.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace FurniTour.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;
        public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<bool> RegisterAsync(RegisterModel registerModel)
        {
            var user = new IdentityUser
            {
                UserName = registerModel.UserName
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                var createdUser = await userManager.FindByNameAsync(registerModel.UserName);
                await userManager.AddToRoleAsync(createdUser, "User");
                await SignInAsync(new LoginModel
                {
                    UserName = registerModel.UserName,
                    Password = registerModel.Password
                });
            }
            return result.Succeeded;
        }

        public async Task<bool> SignInAsync(LoginModel loginModel)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var user = context.Users.FirstOrDefault(x => x.UserName == loginModel.UserName);

            if (user is null)
            {
                return false;
            }

            var isCorrectPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

            if (isCorrectPassword != PasswordVerificationResult.Success)
            {
                return false;
            }

            var claims = new List<Claim>
            {
               // new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24),
                    });
            }

            return true;
        }

        public bool SignOut()
        {
            var result =  httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.IsCompletedSuccessfully)
            {
                return true;
            }
            return false;
        }
    }
}
