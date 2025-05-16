namespace FurniTour.Server.Models.Guarantee
{
    public class GuaranteeModel
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public int? OrderId { get; set; }
        public int? IndividualOrderId { get; set; }
        public bool IsIndividualOrder { get; set; }
        public required string Status { get; set; }
        public required string Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public required List<GuaranteeItemModel> Items { get; set; }
        public required List<string> Photos { get; set; }
    }
}
