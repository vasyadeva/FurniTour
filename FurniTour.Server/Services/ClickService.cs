using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;

namespace FurniTour.Server.Services
{
    public class ClickService : IClickService
    {
        private readonly IAuthService authService;
        private readonly ApplicationDbContext context;

        public ClickService(IAuthService authService, ApplicationDbContext context)
        {
            this.authService = authService;
            this.context = context;
        }
        public async Task<string> AddClick(int itemId)
        {
            var user = authService.GetUser();
            if (user == null)
            {
                return "User not found";
            }
            var item = context.Furnitures.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
            {
                return "Item not found";
            }
            var click = context.Clicks.FirstOrDefault(x => x.UserId == user.Id && x.FurnitureId == itemId);
            if (click != null)
            {
                click.InteractionCount++;
                click.LastInteractionTime = DateTime.Now;
                context.Clicks.Update(click);
                context.SaveChanges();
            }
            else
            {
                click = new Clicks
                {
                    UserId = user.Id,
                    FurnitureId = itemId,
                    InteractionCount = 1,
                    LastInteractionTime = DateTime.Now
                };
                context.Clicks.Add(click);
                context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
