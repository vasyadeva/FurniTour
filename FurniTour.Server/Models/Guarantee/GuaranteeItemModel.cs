namespace FurniTour.Server.Models.Guarantee
{
    public class GuaranteeItemModel
    {
        public int Id { get; set; }
        public int FurnitureId { get; set; }
        public required string FurnitureName { get; set; }
        public int Quantity { get; set; }
    }
}