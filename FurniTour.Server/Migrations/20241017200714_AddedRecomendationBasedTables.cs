using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedRecomendationBasedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachedRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CachedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecommendedFurnitureIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedRecommendations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FurnitureId = table.Column<int>(type: "int", nullable: false),
                    InteractionCount = table.Column<int>(type: "int", nullable: false),
                    LastInteractionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clicks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clicks_Furnitures_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furnitures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRecomendationStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastRecommendationCheck = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecomendationStates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7bae8d17-ec55-4a39-a0a8-531509075112", "AQAAAAIAAYagAAAAELc2QonSKsLi1j088+ZlgXTIYpWv3jW53FlK1gZV1v6ZDS/XDpZSRRBNB4FbQc3pVw==", "140e740a-7c48-4ea6-899b-7c3f53a4e326" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f06c148-8cef-4803-9d1d-f3ae6fee20f2", "AQAAAAIAAYagAAAAEJLOyg5oII0N/JsZCtNv+nIIhJMaZDzfm4+7TkHNkLoEIGOvNL0epv3qKWMBodz+GQ==", "9fe01991-9692-460b-8e17-23b856f03d98" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc5bf0c0-8d1b-402f-bd09-8d6eb80ed083", "AQAAAAIAAYagAAAAEMMV2WvTr+UNm7WEGta4AXZLpHbBsZN85KHvowymNTt3WvL7RDgFHykBeAmn+F+oFw==", "fdb45e1b-0d26-46ae-9abb-70a86b5d77c7" });

            migrationBuilder.CreateIndex(
                name: "IX_Clicks_FurnitureId",
                table: "Clicks",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_Clicks_UserId",
                table: "Clicks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedRecommendations");

            migrationBuilder.DropTable(
                name: "Clicks");

            migrationBuilder.DropTable(
                name: "UserRecomendationStates");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab1e7612-2715-4b35-a14b-4be824f72885", "AQAAAAIAAYagAAAAEBM4zwiHSlx9/eDDXGfAjdqqXMZJvKEf1lhr/o8D8moAM43EqH3OT/8TCuprH5r8Bg==", "48366a95-a75d-4324-a13d-8bb423f4df8b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d604eb7d-c289-4059-8609-2ed4e619a3f9", "AQAAAAIAAYagAAAAEAYEVjzj1UWOdFmdpMmbRb/xrA/Yf3IyEKBb2qPj+BYkAi0mLNzwCYa+p+9uuFONEg==", "2dc3e172-1f61-44c3-8888-3d6acc4484f7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d92c0b42-bbe7-4133-8f3d-6348a2dad54d", "AQAAAAIAAYagAAAAEEwtF7Az3WGl8mPMAngcTglvggCTbDKR3waUpN6FJN996WfJ6fiEeaT8Ewlc55z1Hw==", "d1bcd5f5-da87-4b8d-b009-4c85a72cd199" });
        }
    }
}
