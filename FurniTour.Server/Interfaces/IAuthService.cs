using FurniTour.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> RegisterAsync(RegisterModel model);
        public Task<bool> SignInAsync(LoginModel loginModel);

        public bool SignOut();
    }
}
