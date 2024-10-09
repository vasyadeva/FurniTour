using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedManufacturerReview_And_MasterReview_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManufacturerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManufacturerReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManufacturerReviews_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterReviews_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6c4a8ad-1192-4d4d-9339-0a82e338c170", "AQAAAAIAAYagAAAAEGgoIlSlMZ0RQOlSA4WU25Cdk3bot3mxY/X3insWbEUQX1SICt5hUhJyHgiPqrI0Jg==", "33d80518-8d6f-4edc-ac2c-f8e1312b2783" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f1b48f1-b4ac-449a-aa05-031435f61be2", "AQAAAAIAAYagAAAAEGihpDzPcuGnwCTzXErln6Rf/IyJvA/rKF54f2C066M2jbrnhn6wDtYt4ruTgVdjoA==", "905198ab-8ff2-46cb-89cb-783f16fb6b5a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3b477443-13d4-42e7-9b6a-6ee865d8b7e9", "AQAAAAIAAYagAAAAEAKnQGZoVyFRqdd1O0MXyJRfBNczLZZVeNJaN3j6ockMkJzdHHEt5i7PBbZigxJJnA==", "69d36336-d04b-4249-a789-8a8d34e31ba6" });

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerReviews_ManufacturerId",
                table: "ManufacturerReviews",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerReviews_UserId",
                table: "ManufacturerReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterReviews_MasterId",
                table: "MasterReviews",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterReviews_UserId",
                table: "MasterReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManufacturerReviews");

            migrationBuilder.DropTable(
                name: "MasterReviews");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b7a9a41-f866-4229-ac89-ac5bd44500aa", "AQAAAAIAAYagAAAAEJmYv4XMfTSRWTlHtLlwwoRQjI5tc8iYxv0kjukxEp1p3y+hBaS8FY3bfaRyOpW3sQ==", "bf839b76-2211-4791-a6ad-97532dca35d7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99eda655-6f93-4a89-99bb-7e41cdbd2159", "AQAAAAIAAYagAAAAEG+H5n9Vw+guxTfTyrzWUKUJIlsUyytzuP62EWjkoWSMSoa+RHN24DaoPUiZRLbzgg==", "a0e58c02-ac1d-4bdd-9192-4c8c6b902efe" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3264df0-be26-44f1-9417-c8a0aa8aff21", "AQAAAAIAAYagAAAAEP8brJ5I/Vj7NWdUyMBRux8HoefbTxv0CVFLZGpDB+vU+d/ZAVBYvJKN8QWkSOtjEA==", "16ac8afd-945c-4e0a-a0e9-05d5081dd0f1" });
        }
    }
}
