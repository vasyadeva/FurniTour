using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotosAndReviewTablesForItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FurnitureAdditionalPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FurnitureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureAdditionalPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FurnitureAdditionalPhotos_Furnitures_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furnitures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FurnitureReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    FurnitureId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FurnitureReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FurnitureReviews_Furnitures_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furnitures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4144ca75-3d7b-4354-be65-1673b08f173d", "AQAAAAIAAYagAAAAEKgH4fELUTXYxXPrRlfe6XS7wogVoU0REulXkukATKySe5qPkrai+VN/YKy3EkvumQ==", "7ff10c46-558f-4ec2-86ee-006fc51c2705" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3079ed66-5a84-4405-b126-c157ad95800a", "AQAAAAIAAYagAAAAEOOTkvjhCS7g7U6ggqe8NnRsXd8fzWFa9+n3RuTwHV4ft7ZFSUPb9TPVhnk8iEA2zw==", "1c05ff1d-8ca0-46f2-a147-7655d8877a15" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "49b44d80-f088-489b-946b-8ee60056db5c", "AQAAAAIAAYagAAAAEB9ZQ3Gdc2ZC8xFAbvlpsi0I5UfEQQWts9ouX1/bLYkNc4weyJZ7iuSYBGa0LBsB+A==", "3f5b5a08-3de8-48b4-a8fc-bd2e5835a795" });

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureAdditionalPhotos_FurnitureId",
                table: "FurnitureAdditionalPhotos",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureReviews_FurnitureId",
                table: "FurnitureReviews",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureReviews_UserId",
                table: "FurnitureReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FurnitureAdditionalPhotos");

            migrationBuilder.DropTable(
                name: "FurnitureReviews");

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
        }
    }
}
