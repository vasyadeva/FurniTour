
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniTour.Configurations.Entities
{
    public class RoleSeedConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                },
                new IdentityRole
                {
                    Id = "cac43a7e-f7cb-4148-baaf-1acb431eabbf",
                    Name = "User",
                    NormalizedName = "USER",
                },
                new IdentityRole
                {
                    Id = "cac43a7e-f7cb-4528-baaf-256gj73dfbdf",
                    Name = "Master",
                    NormalizedName = "MASTER",
                }



                );
        }
    }

    
}