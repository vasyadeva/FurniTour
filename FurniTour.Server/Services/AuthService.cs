﻿using FurniTour.Server.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FurniTour.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using FurniTour.Server.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using FurniTour.Server.Constants;

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

        public CredentialsModel GetCredentials()
        {
            var user = httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userm = userManager.FindByNameAsync(user.Identity.Name).Result;
                var id = userm.Id;
                var username = user.Identity.Name;

                return new CredentialsModel
                {
                    id = id,
                    username = username
                };
            }
            return null;
        }
        public async Task<string> RegisterAsync(RegisterModel registerModel)
        {
            var user = new IdentityUser
            {
                UserName = registerModel.UserName
            };
            var userExists = await userManager.FindByNameAsync(registerModel.UserName);
            if (userExists != null)
            {
                return "Користувач вже існує";
            }
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                var createdUser = await userManager.FindByNameAsync(registerModel.UserName);
                switch (registerModel.isMaster)
                {
                    case true:
                        await userManager.AddToRoleAsync(createdUser, "Master");
                        break;
                    case false:
                        await userManager.AddToRoleAsync(createdUser, "User");
                        break;
                }
                await SignInAsync(new LoginModel
                {
                    UserName = registerModel.UserName,
                    Password = registerModel.Password
                });
            }
            else
            {
                return result.Errors.ToString();
            }
            return string.Empty;
        }

        public async Task<string> SignInAsync(LoginModel loginModel)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var user = context.Users.FirstOrDefault(x => x.UserName == loginModel.UserName);

            if (user is null)
            {
                return "Користувача не існує";
            }

            var isCorrectPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

            if (isCorrectPassword != PasswordVerificationResult.Success)
            {
                return "Невірний пароль";
            }

            var role = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role.FirstOrDefault() ?? "User")
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

            return string.Empty;
        }

        public bool SignOut()
        {
            var result = httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return result.IsCompletedSuccessfully;
        }

        public IdentityUser GetUser()
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            return user;
        }

 
        public string GetUserRole()
        {
            var role = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            return role;
        }

        public bool IsRole(string role)
        {
            return GetUserRole() == role;
        }

        public string IsAuthenticated()
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            return user != null ? string.Empty : "Ви не увійшли у систему!";
        }

        public string IsMaster()
        {
            return IsRole(Roles.Master) ? string.Empty : "Ви не є Майстром";
        }

        public string IsAdmin()
        {
            return IsRole(Roles.Administrator) ? string.Empty : "Ви не є Адміністратором";
        }
        public string IsMasterOrAdmin()
        {
            return IsRole(Roles.Master) || IsRole(Roles.Administrator) ? 
                string.Empty : "Ви не є Майстром або Адміністратором";
        }
        public string IsUser()
        {
            return IsRole(Roles.User) ? string.Empty : "Ви не є Користувачем";
        }

        public string CheckRoleMasterOrAdmin()
        {
            string isAuth = IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var isMasterOrAdmin = IsMasterOrAdmin();
                return isMasterOrAdmin.IsNullOrEmpty() ? string.Empty : isMasterOrAdmin;
            }
            return isAuth;
        }

        public string CheckRoleUser()
        {
            string isAuth = IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var isUser = IsUser();
                return isUser.IsNullOrEmpty() ? string.Empty : isUser;
            }
            return isAuth;
        }

        public string CheckRoleMaster()
        {
            string isAuth = IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var isMaster = IsMaster();
                return isMaster.IsNullOrEmpty() ? string.Empty : isMaster;
            }
            return isAuth;
        }

        public string CheckRoleAdmin()
        {
            string isAuth = IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var isAdmin = IsAdmin();
                return isAdmin.IsNullOrEmpty() ? string.Empty : isAdmin;
            }
            return isAuth;
        }

        public string CheckMasterByUsername(string username)
        {
            var user = context.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                var role = userManager.GetRolesAsync(user).Result;
                if (role.FirstOrDefault() == "Master")
                {
                    return string.Empty;
                }
                return "Користувач не є майстром";
            }
            return "Користувача не знайдено";
        }

        public async Task<ProfileModel> GetProfile()
        {
            var user = GetUser();

            if (user != null)
            {
                var role = await userManager.GetRolesAsync(user);
                var profile = new ProfileModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Phonenumber = user.PhoneNumber,
                    Role = role.FirstOrDefault()
                };
                return profile;
            }
            return null;
        }

        public string ChangeProfile(ChangeProfileModel model)
        {
            var user = GetUser();
            if (user != null)
            {
                user.Email = model.Email;
                user.PhoneNumber = model.Phonenumber;
                var result = userManager.UpdateAsync(user).Result;
                if (result.Succeeded)
                {
                    return string.Empty;
                }
                return result.Errors.ToString();
            }
            return "Користувача не знайдено";
        }

        public async Task<IdentityUser> GetUserById(string UserId)
        { 
            var user = await userManager.FindByIdAsync(UserId);
            return user;
        }
    }
}
