using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrderStateConfigToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b88072ca-29c1-4530-88bf-187d7d48bfc6", "AQAAAAIAAYagAAAAECfG3myaDzNroiWo/S25hiFgr91Ms8FQctC+5MGn/vw+VESwtOm9AZ62ChzQQL/OLA==", "0c8761cc-50dd-4017-a56b-287f1989f115" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d0890e3c-42f6-4396-bfb5-f4a962c56e02", "AQAAAAIAAYagAAAAEOFu2wGTIx6wNxo2JMXHVoC9/e/zxc45woSenymfsBCK9egyYLQwH+NIUMnLhqVWDw==", "00a6ae5b-f653-4414-9f9c-f880f2804b95" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a1957f18-0c1c-4c68-b053-e9ed972f849f", "AQAAAAIAAYagAAAAEAa7g9INVcVG+xacEJaqEcwH2KfIxw1UHgCPWGeE/gc+nMpvygTuMtpo+3uf/JuIRw==", "a71fc32f-dc89-409d-b8fe-bad19dee01dc" });

            migrationBuilder.InsertData(
                table: "OrderStates",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "New Order" },
                    { 2, "Cancelled by User" },
                    { 3, "Cancelled by Administrator" },
                    { 4, "Confirmed" },
                    { 5, "In Delivery" },
                    { 6, "Delivered" },
                    { 7, "Delivery Confirmed by User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 7);

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
    }
}
