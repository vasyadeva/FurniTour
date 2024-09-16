using FurniTour.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Хальо ворлд
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<string> RegisterAsync(RegisterModel model);
        public Task<string> SignInAsync(LoginModel loginModel);
        public bool SignOut();
        public string GetUserRole();
    }
}
