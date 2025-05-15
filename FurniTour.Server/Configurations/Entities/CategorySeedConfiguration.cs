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
               new Category { Id = 1, Name = "Ліжка" },
                new Category { Id = 2, Name = "Крісла" },
                new Category { Id = 3, Name = "Столи" },
                new Category { Id = 4, Name = "Дивани" },
                new Category { Id = 5, Name = "Шафи" },
                new Category { Id = 6, Name = "Комоди" },
                new Category { Id = 7, Name = "Тумби" },
                new Category { Id = 8, Name = "Письмові столи" },
                new Category { Id = 9, Name = "Кухні" },
                new Category { Id = 10, Name = "Журнальні столики" },
                new Category { Id = 11, Name = "Полиці" }
           );



        }
    }
}
