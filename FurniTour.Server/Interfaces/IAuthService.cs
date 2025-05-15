using FurniTour.Server.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Interfaces
{
    public interface IAuthService
    {

        public CredentialsModel GetCredentials();
        public Task<string> RegisterAsync(RegisterModel model);
        public Task<string> SignInAsync(LoginModel loginModel);
        public bool SignOut();
        public Task<ProfileModel> GetProfile();
        public string ChangeProfile(ChangeProfileModel model);
        public IdentityUser GetUser();
        public string GetUserRole();
        public bool IsRole(string role);
        public string IsAuthenticated();
        public string IsMaster();
        public string IsAdmin();
        public string IsMasterOrAdmin();
        public string IsUser();
        public string CheckRoleMasterOrAdmin();
        public string CheckRoleAdmin();
        public string CheckRoleMaster();
        public string CheckRoleUser();
        public string CheckMasterByUsername(string username);
        public Task<IdentityUser> GetUserById(string UserId);
    }
}
