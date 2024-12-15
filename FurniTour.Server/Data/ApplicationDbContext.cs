using FurniTour.Configurations.Entities;
using FurniTour.Server.Configurations.Entities;
using FurniTour.Server.Data.Entities;
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
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MasterReview>()
       .HasOne(mr => mr.Master)
       .WithMany()  
       .HasForeignKey(mr => mr.MasterId)
       .OnDelete(DeleteBehavior.NoAction); 

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
    }
}
