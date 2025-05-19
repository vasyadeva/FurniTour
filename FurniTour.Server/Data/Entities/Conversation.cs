using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Data.Entities
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string User1Id { get; set; } = string.Empty;
        
        [ForeignKey(nameof(User1Id))]
        public IdentityUser User1 { get; set; } = null!;
        
        [Required]
        public string User2Id { get; set; } = string.Empty;
        
        [ForeignKey(nameof(User2Id))]
        public IdentityUser User2 { get; set; } = null!;
        
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
} 