using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedOrderStatesToUkrainian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "30ac8016-5b1a-4d1a-a3bb-5cc251ac60a4", "AQAAAAIAAYagAAAAEP9xFC0oP19FXeBV2h55VnkbLXokQyHZrwQzibuRWakSdOApEgFjUgflIIK48acZ8w==", "0f102c2b-183b-4c75-abc8-6159268b630d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "990fb617-1150-4df1-9250-e8144b623e9a", "AQAAAAIAAYagAAAAEBFKqmaJqYp+tS6u2xh/nYo1i9+oUYEMiwU9dEIkYzCu+j78HgTk8Qxm00ffDr456Q==", "088f2d57-a568-4c95-86ff-2b30020dcc35" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "536ad272-ff55-4595-977e-e141402908d3", "AQAAAAIAAYagAAAAEF84/PrSqk6tn1KKHe87K0W0epp/tSfWwt1zo9p5bSpvCMiS552lPX8HTxQuYlNXIg==", "979b23e5-3f85-47ec-b346-70706a5388dc" });

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Нове замовлення");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Скасовано користувачем");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Скасовано адміністратором");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Підтверджено");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "В дорозі");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Доставлено");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Доставка підтверджена користувачем");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf3c56d3-0da6-4a6e-8df9-cd4b5421f4e2", "AQAAAAIAAYagAAAAEOY8UDrgXUPSly3ct1d1383ilml/pKrDFtYrACP2vScRBAovnTVyraEkyhYaAIaaXw==", "8c1e14ce-d7a7-4d1c-9982-27adb4af1c99" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fa1bada5-ba14-463c-919c-7c2ced9d35a8", "AQAAAAIAAYagAAAAENkfJrgfSvRl93BYmUl3za1P3vs3CBwXt928Zx79GdPhm7Eu/0SNxfYp2Kp6jZ5G7w==", "58791a70-34b1-4823-a58a-383f5a5decc9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c71cf030-0cfd-4435-98bc-fb2b9bae8df1", "AQAAAAIAAYagAAAAEEtIuYma09bSiJ+5TsvCG8JtMDcESdawzTD84C1tt4f0yQ6RCOAxAWY9iuaH/DDnxw==", "6f6a4a60-5023-427b-aa46-f16199edfb3a" });

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "New Order");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Cancelled by User");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Cancelled by Administrator");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Confirmed");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "In Delivery");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Delivered");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Delivery Confirmed by User");
        }
    }
}
