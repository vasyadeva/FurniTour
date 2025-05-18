namespace FurniTour.Server.Models.Item
{
    public class FurnitureAdditionalPhotoModel
    {
        public int Id { get; set; }
        public string PhotoData { get; set; } // Base64 encoded
        public string Description { get; set; }
    }
}
