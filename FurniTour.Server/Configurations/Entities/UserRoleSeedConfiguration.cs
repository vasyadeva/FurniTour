using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniTour.Configurations.Entities
{
    public class UserRoleSeedConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
                    UserId = "ccf9bfb8-47d6-41bf-9c5d-502"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "cac43a7e-f7cb-4148-baaf-1acb431eabbf",
                    UserId = "37bb0930-ee5b-483a-88a9-9fc2dab9a087"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "cac43a7e-f7cb-4148-baaf-1acb431eabbf",
                    UserId = "37bb0930-ee5b-483a-88a9-9fc2dab9a903"
                }
                );
        }
    }
}