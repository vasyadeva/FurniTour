using FurniTour.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniTour.Server.Configurations.Entities
{
    public class OrderStateSeedConfiguration:IEntityTypeConfiguration<OrderState>
    {
        public void Configure(EntityTypeBuilder<OrderState> builder)
        {
             builder.HasData(
                new OrderState { Id = 1, Name = "Нове замовлення" },
                new OrderState { Id = 2, Name = "Скасовано користувачем" },
                new OrderState { Id = 3, Name = "Скасовано адміністратором" },
                new OrderState { Id = 4, Name = "Підтверджено" },
                new OrderState { Id = 5, Name = "В дорозі" },
                new OrderState { Id = 6, Name = "Доставлено" },
                new OrderState { Id = 7, Name = "Доставка підтверджена користувачем" }
            );

        }
    }
}
