using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Furniture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public int? ColorId { get; set; }
        [ForeignKey(nameof(ColorId))]
        public Color Color { get; set; }
        public string? MasterId { get; set; }
        [ForeignKey(nameof(MasterId))]
        public IdentityUser Master { get; set; }
        public int? ManufacturerId { get; set; }
        [ForeignKey(nameof(ManufacturerId))]
        public Manufacturer Manufacturer { get; set; }
    }
}
