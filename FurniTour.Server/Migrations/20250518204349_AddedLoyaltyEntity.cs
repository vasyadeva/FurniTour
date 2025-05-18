using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedLoyaltyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AppliedDiscount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "UserLoyalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalSpent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoyaltyLevel = table.Column<int>(type: "int", nullable: false),
                    CurrentDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoyalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoyalties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b50e9e71-8ebd-42d7-81e9-561a4635102c", "AQAAAAIAAYagAAAAEIHAxq4Nyo0bq06ZeiS/9sr9RSMBE56YU/DkhEKbvvXcUYcuqfpoPY6iDhjZs/gskA==", "a55f5b94-7c2e-41ee-82d9-23f801ead16f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d8a7fb36-61d9-4639-98fc-f64da91aee2c", "AQAAAAIAAYagAAAAEFY7yV5zLH5NDRJKdVMCbcRvIOpReWAM87LfnHnYbViO7oXvKKf5FStO/TjpD6Me2g==", "3cede150-4188-41da-b8a5-7150b4c8df99" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10af60f5-b9a2-4424-bc3b-551bb8ff8e2d", "AQAAAAIAAYagAAAAEJJRhwe94tT/udS5IJQb01FqZa+MKp7WlQE9VLsZaj/wLrojM7PsQP4sNCq19dHSPw==", "4433cef3-97f0-4bc1-8f95-6812886df250" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoyalties_UserId",
                table: "UserLoyalties",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLoyalties");

            migrationBuilder.DropColumn(
                name: "AppliedDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Orders");

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
        }
    }
}
