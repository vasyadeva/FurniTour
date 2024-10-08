using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Manufacturer;

namespace FurniTour.Server.Interfaces
{
    public interface IManufacturerService
    {
        public List<Manufacturer> GetManufacturersList();
        public Task<Manufacturer> GetManufacturer(int id);
        public Task<string> AddManufacturer(AddManufacturerModel model);
        public Task<string> UpdateManufacturer(ManufacturerModel model);
        public Task<string> DeleteManufacturer(int id);
    }
}
