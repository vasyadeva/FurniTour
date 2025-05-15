using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class IndividualOrder
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        //посилання на майстра
        public string MasterId { get; set; }
        [ForeignKey(nameof(MasterId))]
        public IdentityUser Master { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public byte[]? Photo { get; set; }
        [Precision(18, 2)]
        public decimal? EstimatedPrice { get; set; }
        [Precision(18, 2)]
        public decimal? FinalPrice { get; set; }
        public string? MasterNotes { get; set; }
        public int PriceCategoryId { get; set; }
        [ForeignKey(nameof(PriceCategoryId))]
        public PriceCategory PriceCategory { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int IndividualOrderStatusId{ get; set; }
        [ForeignKey(nameof(IndividualOrderStatusId))]
        public IndividualOrderStatus IndividualOrderStatus { get; set; }

    }
}
