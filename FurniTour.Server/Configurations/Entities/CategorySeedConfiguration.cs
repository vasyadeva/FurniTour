using FurniTour.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Configurations.Entities
{
    public class CategorySeedConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
               new Category { Id = 1, Name = "Beds" },
               new Category { Id = 2, Name = "Chairs" },
               new Category { Id = 3, Name = "Tables" },
               new Category { Id = 4, Name = "Sofas" },
               new Category { Id = 5, Name = "Cupboards" },
               new Category { Id = 6, Name = "Shelves" },
               new Category { Id = 7, Name = "Dressers" },
               new Category { Id = 8, Name = "Wardrobes" }
           );



        }
    }
}
