namespace FurniTour.Server.Models.Item
{
    public class FurnitureReviewModel
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddFurnitureReviewModel
    {
        public int FurnitureId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
