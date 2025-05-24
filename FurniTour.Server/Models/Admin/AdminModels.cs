using System.ComponentModel.DataAnnotations;

namespace FurniTour.Server.Models.Admin
{
    public class UserAdminModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
    
    public class ColorCreateModel
    {
        [Required(ErrorMessage = "Назва кольору обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва кольору не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
    
    public class ColorUpdateModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Назва кольору обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва кольору не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
    
    public class CategoryCreateModel
    {
        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва категорії не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
    
    public class CategoryUpdateModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва категорії не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
    
    public class UserRoleUpdateModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string RoleId { get; set; } = string.Empty;
    }
    
    public class ManufacturerCreateModel
    {
        [Required(ErrorMessage = "Назва виробника обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва виробника не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
    
    public class ManufacturerUpdateModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Назва виробника обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва виробника не може бути довшою за 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
}
