using FurniTour.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace FurniTour.Server.Models.Guarantee
{    public class GuaranteeAddModel
    {
        public int? OrderId { get; set; }
        
        public int? IndividualOrderId { get; set; }
        
        public bool IsIndividualOrder { get; set; }
        
        [Required]
        [MinLength(10)]
        public string Comment { get; set; } = string.Empty;
        
        public List<string> Photos { get; set; } = new List<string>();
        
        [Required]
        public List<int> Items { get; set; } = new List<int>();
    }
}
