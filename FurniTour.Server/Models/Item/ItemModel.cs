using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Models.Item
{
    public class ItemModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public int? ManufacturerId { get; set; }
        public List<string> AdditionalPhotos { get; set; } = new List<string>(); // Base64 encoded images
        public List<string> PhotoDescriptions { get; set; } = new List<string>();
    }
}
