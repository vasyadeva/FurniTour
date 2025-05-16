using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddeIndividualOrderPropToGuarantee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Guarantees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IndividualOrderId",
                table: "Guarantees",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d50bb3d9-125c-4102-802d-8d53df6713fa", "AQAAAAIAAYagAAAAED/lbYpwxwbdyjMdliek5MrXqTzX9g0H3mKgoXaDhwubW7Jt8t1FgyjVAo7W8eG4IA==", "b6e114e1-2f57-4fca-b0df-dae6f52bfd0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3ac8d303-4d17-42ea-b028-8c871882e70a", "AQAAAAIAAYagAAAAEJwIjJH5E21MJqTJK3e+lVKDMEWALWrz8XOX++6wJ7TXwt0Na7ihFV3kgXVUTimo9w==", "6e2e0acd-3c52-4ed2-a123-d4156826e70b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bdebeaf5-cfba-44f6-ab57-14cc5f9423f4", "AQAAAAIAAYagAAAAEDISyGO+sZ6PVgwdxNr4LJ8hugR1I0dKMw/1TzFyDhatx5eK7ZkyrKvHfreTmVNHJQ==", "9efe56ee-2996-4805-8855-d211dedd5eef" });

            migrationBuilder.CreateIndex(
                name: "IX_Guarantees_IndividualOrderId",
                table: "Guarantees",
                column: "IndividualOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guarantees_IndividualOrders_IndividualOrderId",
                table: "Guarantees",
                column: "IndividualOrderId",
                principalTable: "IndividualOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guarantees_IndividualOrders_IndividualOrderId",
                table: "Guarantees");

            migrationBuilder.DropIndex(
                name: "IX_Guarantees_IndividualOrderId",
                table: "Guarantees");

            migrationBuilder.DropColumn(
                name: "IndividualOrderId",
                table: "Guarantees");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Guarantees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
        }
    }
}
