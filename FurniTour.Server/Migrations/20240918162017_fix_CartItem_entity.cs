using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class fix_CartItem_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fdc4a2b3-6ddc-46f0-b702-ca30a44a6fc3", "AQAAAAIAAYagAAAAEGA/SgAIlezrnFuNHCxUU1MDM//OWWbawnLxVBNCSne5JQcMgOLDxq7rweKJ/Pa0Lg==", "61274f0a-ee1f-4471-9774-ac80aa284e44" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e061ffa1-9e50-44f8-ad25-0e711f571636", "AQAAAAIAAYagAAAAEH7J74A7uAptmeC5l3TQv9MurcD1VWMQ6Y96I5L0XPtVU+YEMJDKUBJV790vQWMsoA==", "01094c80-16a8-4a69-8dec-71909f37eb5b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2602262c-0cf8-4e44-9b3c-949dca5e8c2f", "AQAAAAIAAYagAAAAECs1Fh6Xo6wzgOtzYlJeREFy1IG2QrBmvEZ2DsuIbpMTBUpf9Rw2aeGS6o1Zb7cAsQ==", "a95f2231-2580-4931-a369-a3ce0dadc6f5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
