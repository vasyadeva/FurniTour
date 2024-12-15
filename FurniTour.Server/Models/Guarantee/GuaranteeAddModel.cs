using FurniTour.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace FurniTour.Server.Models.Guarantee
{
    public class GuaranteeAddModel
    {
        public int OrderId { get; set; }
        public string Comment { get; set; }
        public List<string> Photos { get; set; }
        public List<int> Items { get; set; }
    }
}
