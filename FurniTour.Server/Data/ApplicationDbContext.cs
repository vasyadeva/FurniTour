using FurniTour.Configurations.Entities;
using FurniTour.Server.Configurations.Entities;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Chat;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FurniTour.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleSeedConfiguration());
            builder.ApplyConfiguration(new UserSeedConfiguration());
            builder.ApplyConfiguration(new UserRoleSeedConfiguration());
            builder.ApplyConfiguration(new OrderStateSeedConfiguration());    
            builder.ApplyConfiguration(new CategorySeedConfiguration());
            builder.ApplyConfiguration(new ColorSeedConfiguration());

            builder.Entity<CartItem>()
           .HasOne(ci => ci.Furniture)
           .WithMany()
           .HasForeignKey(ci => ci.FurnitureId)
           .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<OrderItem>()
    .HasOne(oi => oi.Order)
    .WithMany(o => o.OrderItems)
    .HasForeignKey(oi => oi.OrderId)
    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Furniture)
                .WithMany()
                .HasForeignKey(oi => oi.FurnitureId)
                .OnDelete(DeleteBehavior.Cascade);            builder.Entity<MasterReview>()
       .HasOne(mr => mr.Master)
       .WithMany()
       .HasForeignKey(mr => mr.MasterId)
       .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FurnitureReview>()
       .HasOne(fr => fr.Furniture)
       .WithMany(f => f.Reviews)
       .HasForeignKey(fr => fr.FurnitureId)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FurnitureReview>()
       .HasOne(fr => fr.User)
       .WithMany()
       .HasForeignKey(fr => fr.UserId)
       .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FurnitureAdditionalPhoto>()
       .HasOne(fap => fap.Furniture)
       .WithMany(f => f.AdditionalPhotos)
       .HasForeignKey(fap => fap.FurnitureId)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MasterReview>()
                .HasOne(mr => mr.User)
                .WithMany()  
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ManufacturerReview>()
                .HasOne(mr => mr.User)
                .WithMany()
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Guarantee>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Guarantee>()
                .HasOne(g => g.Order)
                .WithMany()
                .HasForeignKey(g => g.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GuaranteeItems>()
                .HasOne(gi => gi.Guarantee)
                .WithMany(g => g.GuaranteeItems)
                .HasForeignKey(gi => gi.GuaranteeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GuaranteeItems>()
                .HasOne(gi => gi.OrderItem)
                .WithMany()
                .HasForeignKey(gi => gi.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Налаштування зв'язку для IndividualOrder -> User
            builder.Entity<IndividualOrder>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Налаштування зв'язку для IndividualOrder -> Master, з відключенням каскадного видалення
            builder.Entity<IndividualOrder>()
                .HasOne(o => o.Master)
                .WithMany()
                .HasForeignKey(o => o.MasterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for Chat entities
            builder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IndividualOrderStatus>().HasData(
      new IndividualOrderStatus { Id = 1, Name = "Нове індивідуальне замовлення" },
      new IndividualOrderStatus { Id = 2, Name = "Скасовано користувачем" },
      new IndividualOrderStatus { Id = 3, Name = "Скасовано майстром" },
      new IndividualOrderStatus { Id = 4, Name = "Підтверджено" },
      new IndividualOrderStatus { Id = 5, Name = "У виробництві" },
      new IndividualOrderStatus { Id = 6, Name = "В дорозі" },
      new IndividualOrderStatus { Id = 7, Name = "Доставлено" },
      new IndividualOrderStatus { Id = 8, Name = "Доставка підтверджена користувачем" }
  );

            builder.Entity<PriceCategory>().HasData(
                new PriceCategory
                {
                    Id = 1,
                    Name = "Економ",
                    Description = "Доступні матеріали"
                },
                new PriceCategory
                {
                    Id = 2,
                    Name = "Стандарт",
                    Description = "Якісні матеріали"
                },
                new PriceCategory
                {
                    Id = 3,
                    Name = "Преміум",
                    Description = "Елітні матеріали"
                }
            );

            // Configure CachedRecommendation to store List<int> as JSON
            builder.Entity<CachedRecommendation>()
                .Property(e => e.RecommendedFurnitureIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );
        }

        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ManufacturerReview> ManufacturerReviews { get; set; }
        public DbSet<MasterReview> MasterReviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Clicks> Clicks { get; set; }
        public DbSet<CachedRecommendation> CachedRecommendations { get; set; }
        public DbSet<UserRecomendationState> UserRecomendationStates { get; set; }        
        public DbSet<Guarantee> Guarantees { get; set; }
        public DbSet<GuaranteeItems> GuaranteeItems { get; set; }
        public DbSet<GuaranteePhoto> GuaranteePhotos { get; set; }
        public DbSet<IndividualOrderStatus> IndividualOrderStatuses { get; set; }
        public DbSet<PriceCategory> PriceCategories { get; set; }
        public DbSet<IndividualOrder> IndividualOrders { get; set; }
        public DbSet<FurnitureReview> FurnitureReviews { get; set; }
        public DbSet<FurnitureAdditionalPhoto> FurnitureAdditionalPhotos { get; set; }
        public DbSet<UserLoyalty> UserLoyalties { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
