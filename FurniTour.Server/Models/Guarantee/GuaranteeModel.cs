namespace FurniTour.Server.Models.Guarantee
{
    public class GuaranteeModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<GuaranteeItemModel> Items { get; set; }
    }
}
