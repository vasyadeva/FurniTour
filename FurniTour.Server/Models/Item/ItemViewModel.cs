using FurniTour.Server.Data.Entities;

namespace FurniTour.Server.Models.Item
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public string? Master { get; set; }
        public string? Manufacturer { get; set; }
        public List<FurnitureReviewModel> Reviews { get; set; } = new List<FurnitureReviewModel>();
        public List<FurnitureAdditionalPhotoModel> AdditionalPhotos { get; set; } = new List<FurnitureAdditionalPhotoModel>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
