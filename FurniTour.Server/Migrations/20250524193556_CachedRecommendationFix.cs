using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class CachedRecommendationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0e9e5e81-0482-427d-9132-359240e78f5e", "AQAAAAIAAYagAAAAEI7CNJl3wqIvN8b++kSIlc0PuMlcx8cvhrHoi6aE1xytWof0C+8XgB4vkW+NXiGaiA==", "8fae43aa-f4bf-489e-890f-50df469b0d3c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "62e70297-3d85-4044-baa3-153e99e446c6", "AQAAAAIAAYagAAAAEO0W30RXODuHHMlAHbnyhlFt2/GBO427XntJpwTknd5aJJ6WjaIC6LccN8DDw3jGDw==", "1789f84f-f6bb-4230-b473-7add3fcd68be" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a44089d-8f20-4a38-85a8-67a6ecbee098", "AQAAAAIAAYagAAAAED/Y8pHS9EnyqWNW5JdtEgyytbVpP3y7M+av0OnDkIP7E6o/drNEOcYujpDdn8bTPg==", "2f5d6faf-6dd5-4ae3-87d0-54f6f0b0fad5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e5edb0f-8f10-477f-b5f7-34f2f8acac6d", "AQAAAAIAAYagAAAAEL+iG2gs8sAzw2U7Tb95XIo8ohu9WFhYKWB6lijn+vJyCorsVfhack1hzt6fHFKZsQ==", "7754de7d-4413-49c8-93c2-b8ad702dda27" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab2f87b5-0bbe-4e5c-bc43-85e8695ac40d", "AQAAAAIAAYagAAAAEFrNj7EslX7kcL0ok1IIXf+M0wdQhNYUUt6IMNJBeJSTC7b5NSb4LI553IvNuIChPQ==", "63aa563d-6939-456f-9ec8-d5549b080747" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7f42d759-7ae3-44ae-be31-9cf82740dcaf", "AQAAAAIAAYagAAAAEPWL0ps1DwW0OpU5fupd7tLR2+jQCaXXmn8/w63KVMxn7Ox6yEfocdg0hguZrhp5EQ==", "f6b007a0-4e79-489f-8aad-c8cad033a82b" });
        }
    }
}
