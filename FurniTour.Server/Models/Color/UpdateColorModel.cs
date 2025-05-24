using System.ComponentModel.DataAnnotations;

namespace FurniTour.Server.Models.Color
{
    public class UpdateColorModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва кольору є обов'язковою")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва кольору повинна містити від 2 до 50 символів")]
        public string Name { get; set; } = string.Empty;
    }
}
