namespace FurniTour.Server.Models.Auth
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool isMaster { get; set; }
    }
}
