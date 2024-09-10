
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniTour.Configurations.Entities
{
    public class UserSeedConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();  
            builder.HasData(
                new IdentityUser
                {
                    Id= "ccf9bfb8-47d6-41bf-9c5d-502",
                    Email = "admin@test.com",
                    NormalizedEmail = "ADMIN@TEST.COM",
                    NormalizedUserName = "ADMIN",
                    UserName = "admin",
                    
                    PasswordHash = hasher.HashPassword(null, "Hello1234-"),
                    EmailConfirmed = true

                },
                new IdentityUser
                {
                    Id = "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                    Email = "user1@test.com",
                    NormalizedEmail = "USER1@TEST.COM",
                    NormalizedUserName = "USER1",
                    UserName = "user1",
                   
                    PasswordHash = hasher.HashPassword(null, "Hello1234-"),
                    EmailConfirmed = true

                },
                new IdentityUser
                {
                    Id = "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                    Email = "master1@test.com",
                    NormalizedEmail = "MASTER1@TEST.COM",
                    NormalizedUserName = "MASTER1",
                    UserName = "master1",
                    PasswordHash = hasher.HashPassword(null, "Hello1234-"),
                    EmailConfirmed = true

                }

                );
        }
    }
}