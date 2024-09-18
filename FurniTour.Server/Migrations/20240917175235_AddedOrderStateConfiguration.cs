using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrderStateConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b3b18f68-b8ca-4ebd-996c-f060bf4772ce", "AQAAAAIAAYagAAAAEMvt2dgk2/A5QyEwxWsWXoV5VvxBFx3SPT/hZbYDtmMbgF6bPRWBhgVQfrsAQuj6Iw==", "e65afba8-3ba8-453b-a939-d6b550dff442" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "015b3be0-e69b-45b4-ae64-28a87eb31b20", "AQAAAAIAAYagAAAAEDVinOLtkK/muPL3nokdWnukWne8F17SyXtgImwRyW63pWpTugzKB+C2spzaShs4RQ==", "553e28f4-2f84-4d98-acf5-ef8b5fc62972" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf478e09-3ac1-4872-a70c-e9eb858b69d4", "AQAAAAIAAYagAAAAELwzjOCXdFYOQnoHzDX9Jw/F2CVE1vhepoUm/3QagUKDeWOzfA46LkSvqtIMhcVZZQ==", "0204d8cb-4a6d-4001-9ca5-7acca0d5b67f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b08acd47-3567-4090-bf2b-606e8c0561e2", "AQAAAAIAAYagAAAAEDjDb8e3WJlreFBHhu5U24vU+pk3uMYPn+5g29tslYpgRKduOy1PEO+Z+WYR7TE1tg==", "520c3338-69e6-4663-a386-35ae06ef65ba" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3ba490c0-4478-4298-a5da-aee5b3875b9f", "AQAAAAIAAYagAAAAEKgp3PuhBX6WwZLU5Su6Lu1QJFj4lzdZ1+nric/B1ueu2NC3XW2btyFSRgLkg6p5WQ==", "c2b9970f-3048-47b4-8c0b-114f77b8e128" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ddfb45f6-21fe-4da7-ad93-e4e0afc5c593", "AQAAAAIAAYagAAAAEHv+NER6VzmxQGGxZOvVdo0U695aVtJy9DaNLEnApEDP5F7N5/7tON+OBcYocm74Lw==", "ca0986a6-0e50-4a0d-9b36-99e42b01d23e" });
        }
    }
}
