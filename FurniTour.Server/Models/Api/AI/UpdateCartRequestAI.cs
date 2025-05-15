namespace FurniTour.Server.Models.Api.AI
{
    public class UpdateCartRequestAI
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public int Quantity { get; set; }
    }
}