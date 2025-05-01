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
                new Color { Id = 1, Name = "Чорний" },
                new Color { Id = 2, Name = "Білий" },
                new Color { Id = 3, Name = "Червоний" },
                new Color { Id = 4, Name = "Жовтий" },
                new Color { Id = 5, Name = "Зелений" },
                new Color { Id = 6, Name = "Синій" },
                new Color { Id = 7, Name = "Фіолетовий" },
                new Color { Id = 8, Name = "Оранжевий" },
                new Color { Id = 9, Name = "Коричневий" },
                new Color { Id = 10, Name = "Сірий" },
                new Color { Id = 11, Name = "Рожевий" },
                new Color { Id = 12, Name = "Бежевий" },
                new Color { Id = 13, Name = "Бірюзовий" },
                new Color { Id = 14, Name = "Пурпуровий" },
                new Color { Id = 15, Name = "Лаймовий" },
                new Color { Id = 16, Name = "Морська хвиля" },
                new Color { Id = 17, Name = "Бордовий" },
                new Color { Id = 18, Name = "Темно-синій" },
                new Color { Id = 19, Name = "Оливковий" },
                new Color { Id = 20, Name = "Сріблястий" },

                // Меблеві текстури
                new Color { Id = 21, Name = "Дуб" },
                new Color { Id = 22, Name = "Горіх" },
                new Color { Id = 23, Name = "Венге" },
                new Color { Id = 24, Name = "Бук" },
                new Color { Id = 25, Name = "Сосна" },
                new Color { Id = 26, Name = "Вільха" },
                new Color { Id = 27, Name = "Ясен" },
                new Color { Id = 28, Name = "Клен" },
                new Color { Id = 29, Name = "Махонь" },
                new Color { Id = 30, Name = "Чорний дуб" },
                new Color { Id = 31, Name = "Дуб сонома" },
                new Color { Id = 32, Name = "Дуб молочний" }
            );
        }


    }
}
