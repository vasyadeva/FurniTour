using FurniTour.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Configurations.Entities
{
    public class ColorSeedConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.HasData(
                new Color { Id = 1, Name = "Black" },
                new Color { Id = 2, Name = "White" },
                new Color { Id = 3, Name = "Red" },
                new Color { Id = 4, Name = "Yellow"},
                new Color { Id = 5, Name = "Green"},
                new Color { Id = 6, Name = "Blue" },
                new Color { Id = 7, Name = "Purple" },
                new Color { Id = 8, Name = "Orange" },
                new Color { Id = 9, Name = "Brown" },
                new Color { Id = 10, Name = "Grey" },
                new Color { Id = 11, Name = "Pink" },
                new Color { Id = 12, Name = "Beige" },
                new Color { Id = 13, Name = "Cyan" },
                new Color { Id = 14, Name = "Magenta" },
                new Color { Id = 15, Name = "Lime" },
                new Color { Id = 16, Name = "Teal" },
                new Color { Id = 17, Name = "Maroon" },
                new Color { Id = 18, Name = "Navy" },
                new Color { Id = 19, Name = "Olive" },
                new Color { Id = 20, Name = "Silver" }
           );

        }
    
    }
}
