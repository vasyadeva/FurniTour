using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndividualOrderEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndividualOrderStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualOrderStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndividualOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MasterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EstimatedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MasterNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceCategoryId = table.Column<int>(type: "int", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IndividualOrderStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndividualOrders_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndividualOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndividualOrders_IndividualOrderStatuses_IndividualOrderStatusId",
                        column: x => x.IndividualOrderStatusId,
                        principalTable: "IndividualOrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndividualOrders_PriceCategories_PriceCategoryId",
                        column: x => x.PriceCategoryId,
                        principalTable: "PriceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_IndividualOrders_IndividualOrderStatusId",
                table: "IndividualOrders",
                column: "IndividualOrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualOrders_MasterId",
                table: "IndividualOrders",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualOrders_PriceCategoryId",
                table: "IndividualOrders",
                column: "PriceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualOrders_UserId",
                table: "IndividualOrders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndividualOrders");

            migrationBuilder.DropTable(
                name: "IndividualOrderStatuses");

            migrationBuilder.DropTable(
                name: "PriceCategories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6c2b70c-aabb-499e-88d6-e0dbd4c066ef", "AQAAAAIAAYagAAAAECCG5YyPMhQazdYQuydUMjq1tm+bWOqnqDfL4xL1aStw+y+fX+zdALbHMP/9RzDV2w==", "6607f887-f811-4ee4-8694-2dd7e31bd072" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8927f84f-8c66-4e3d-9e5d-f93679b2aa09", "AQAAAAIAAYagAAAAEAjM5YZjpF2sR4qU5nhYSWeOJPIrWC+hw5wIL6uaNAmIUhes+r4opYJcgazdC8ueGw==", "6570bce9-dfc7-439c-86b6-9ca97c808d7c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b35b3277-557c-438b-b64f-9fca8cbeaca9", "AQAAAAIAAYagAAAAEFydw05W/ii0P7cf35gVWM+FfAIcZWyLcaN1BgGa4lozEALBCrJgaNwiOAq4669kZQ==", "45dc4134-f915-4cb7-8529-0191d92b3fb4" });
        }
    }
}
