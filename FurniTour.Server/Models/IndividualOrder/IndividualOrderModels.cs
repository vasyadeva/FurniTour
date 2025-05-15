namespace FurniTour.Server.Models.IndividualOrder
{
    public class IndividualOrderModel
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
        public int PriceCategoryId { get; set; }
        public string MasterId { get; set; } = string.Empty;
    }

    public class IndividualOrderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public decimal? EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string MasterNotes { get; set; } = string.Empty;
        public string PriceCategory { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DateCompleted { get; set; }
        public string MasterName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
