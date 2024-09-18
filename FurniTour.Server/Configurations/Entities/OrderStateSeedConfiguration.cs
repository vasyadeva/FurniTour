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
                new OrderState { Id = 1, Name = "New Order" },
                new OrderState { Id = 2, Name = "Cancelled by User" },
                new OrderState { Id = 3, Name = "Cancelled by Administrator" },
                new OrderState { Id = 4, Name = "Confirmed" },
                new OrderState { Id = 5, Name = "In Delivery" },
                new OrderState { Id = 6, Name = "Delivered" },
                new OrderState { Id = 7, Name = "Delivery Confirmed by User" }
            );

        }
    }
}
