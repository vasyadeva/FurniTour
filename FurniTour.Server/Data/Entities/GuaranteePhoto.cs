using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class GuaranteePhoto
    {
        public int Id { get; set; }
        public int GuaranteeId { get; set; }
        [ForeignKey(nameof(GuaranteeId))]
        public Guarantee Guarantee { get; set; }
        public byte[] Photo { get; set; }
    }
}
