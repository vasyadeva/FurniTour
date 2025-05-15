using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedConfigurationToIndividualOrderStatusesAndPriceCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "738db1af-fdfd-4843-94cc-f5ef17ef2d7a", "AQAAAAIAAYagAAAAEPDfDeqo51wML+Lo9BG2fenrrsFFhf8b9JDQbqOSxMLvKpAvEWd+X7gWwpX1K8ZcmA==", "16e9e8c2-9402-4982-95b9-da7801daba81" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "75328537-6ba2-4ed0-98c5-e2395945292d", "AQAAAAIAAYagAAAAEKSWYZMIuXKesq0QNlVqGxreWWsXn2EzyKLbIXC5mx32cBC9goPDc4Be06K4u/wQEQ==", "67553606-7ab9-47f0-981d-60d0314a872d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "86cb9eeb-5721-4478-afa6-ddfdfdd9e396", "AQAAAAIAAYagAAAAEJLtHoMLMXrOcD2Qj/z3BPkB0TPKPiHnKyLXcZkbifLOCocbBOGZRYfKfjrkIXGmpw==", "12ceb4e4-1489-4b3c-93c6-be884b036e33" });

            migrationBuilder.InsertData(
                table: "IndividualOrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Нове індивідуальне замовлення" },
                    { 2, "Скасовано користувачем" },
                    { 3, "Скасовано майстром" },
                    { 4, "Підтверджено" },
                    { 5, "У виробництві" },
                    { 6, "В дорозі" },
                    { 7, "Доставлено" },
                    { 8, "Доставка підтверджена користувачем" }
                });

            migrationBuilder.InsertData(
                table: "PriceCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Доступні матеріали", "Економ" },
                    { 2, "Якісні матеріали", "Стандарт" },
                    { 3, "Елітні матеріали", "Преміум" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "IndividualOrderStatuses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PriceCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PriceCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PriceCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "169d08d2-7ca7-4701-9be8-d6fdaa330e8a", "AQAAAAIAAYagAAAAEB7zVXDZp/Gx41i+THQcyP7QrXLNRXFspponyG/QYyBLSfFs7ahiG4ae5pvrfufkQA==", "63a32774-3185-419a-a881-cbe328c60c79" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22a8859e-950c-469c-ba73-e7d58455137a", "AQAAAAIAAYagAAAAEIzwgN1+IID/T1/aqNPx+pQDbwbQPMIEDD9xFV4tYkxu+zUTMcJiZYipB7fAxLV+Jg==", "cc9bb622-0b0f-4c67-8da3-85c2445681a2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e423dbc-764b-4abe-9278-f314c787ea57", "AQAAAAIAAYagAAAAEN1YQ1x5ZDYb580F62dthRBicYAhOF0yF0grbahNqkSyizAqwyORPe4+K5EOMMJOzQ==", "356d647c-e083-4da6-b7e4-5f209daf9544" });
        }
    }
}
