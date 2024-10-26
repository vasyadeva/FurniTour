namespace FurniTour.Server.Interfaces
{
    public interface IClickService
    {
        public Task<string> AddClick(int itemId);  
    }
}
