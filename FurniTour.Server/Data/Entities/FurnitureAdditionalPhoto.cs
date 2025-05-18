using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class FurnitureAdditionalPhoto
    {
        public int Id { get; set; }
        public byte[] PhotoData { get; set; }
        public string? Description { get; set; }
        
        public int FurnitureId { get; set; }
        [ForeignKey(nameof(FurnitureId))]
        public Furniture Furniture { get; set; }
    }
}
