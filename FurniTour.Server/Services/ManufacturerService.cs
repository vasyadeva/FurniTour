using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Manufacturer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IAuthService authService;
        private readonly ApplicationDbContext context;

        public ManufacturerService(IAuthService authService, ApplicationDbContext context)
        {
            this.authService = authService;
            this.context = context;
        }
        public async Task<string> AddManufacturer(AddManufacturerModel model)
        {
            var check = authService.CheckRoleAdmin();
            if (check.IsNullOrEmpty())
            {
                var manufacturer = new Manufacturer
                {
                    Name = model.Name
                };
                context.Add(manufacturer);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return check;
        }

        public async Task<string> DeleteManufacturer(int id)
        {
            var check = authService.CheckRoleAdmin();
            if (check.IsNullOrEmpty())
            {
                var manufacturer = context.Manufacturers.Find(id);
                if (manufacturer != null)
                {
                    context.Manufacturers.Remove(manufacturer);
                    context.SaveChanges();
                    return string.Empty;
                }
                return "Manufacturer not found";
            }
            return check;

        }
        public async Task<Manufacturer> GetManufacturer(int id)
        {
            var manufacturer = await context.Manufacturers.FirstOrDefaultAsync();
            return manufacturer;
        }
        public  List<Manufacturer> GetManufacturersList()
        {
            var manufacturers = context.Manufacturers.ToList();
            return manufacturers;
        }

        public async Task<string> UpdateManufacturer(ManufacturerModel model)
        {
            var check = authService.CheckRoleAdmin();
            if (check.IsNullOrEmpty())
            {
                var manufacturer = context.Manufacturers.Find(model.Id);
                if (manufacturer != null)
                {
                    manufacturer.Name = model.Name;
                    context.SaveChanges();
                    return string.Empty;
                }
                return "Manufacturer not found";
            }
            return check;
        }
    }
}
