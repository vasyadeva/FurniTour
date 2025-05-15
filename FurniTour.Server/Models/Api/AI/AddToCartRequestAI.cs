namespace FurniTour.Server.Models.Api.AI
{
    public class AddToCartRequestAI
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public int Quantity { get; set; }
    }
}
