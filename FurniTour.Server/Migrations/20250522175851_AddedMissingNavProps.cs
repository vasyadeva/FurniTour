using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissingNavProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRecomendationStates",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CachedRecommendations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e5edb0f-8f10-477f-b5f7-34f2f8acac6d", "AQAAAAIAAYagAAAAEL+iG2gs8sAzw2U7Tb95XIo8ohu9WFhYKWB6lijn+vJyCorsVfhack1hzt6fHFKZsQ==", "7754de7d-4413-49c8-93c2-b8ad702dda27" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab2f87b5-0bbe-4e5c-bc43-85e8695ac40d", "AQAAAAIAAYagAAAAEFrNj7EslX7kcL0ok1IIXf+M0wdQhNYUUt6IMNJBeJSTC7b5NSb4LI553IvNuIChPQ==", "63aa563d-6939-456f-9ec8-d5549b080747" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7f42d759-7ae3-44ae-be31-9cf82740dcaf", "AQAAAAIAAYagAAAAEPWL0ps1DwW0OpU5fupd7tLR2+jQCaXXmn8/w63KVMxn7Ox6yEfocdg0hguZrhp5EQ==", "f6b007a0-4e79-489f-8aad-c8cad033a82b" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRecomendationStates_UserId",
                table: "UserRecomendationStates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedRecommendations_UserId",
                table: "CachedRecommendations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CachedRecommendations_AspNetUsers_UserId",
                table: "CachedRecommendations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRecomendationStates_AspNetUsers_UserId",
                table: "UserRecomendationStates",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CachedRecommendations_AspNetUsers_UserId",
                table: "CachedRecommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRecomendationStates_AspNetUsers_UserId",
                table: "UserRecomendationStates");

            migrationBuilder.DropIndex(
                name: "IX_UserRecomendationStates_UserId",
                table: "UserRecomendationStates");

            migrationBuilder.DropIndex(
                name: "IX_CachedRecommendations_UserId",
                table: "CachedRecommendations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRecomendationStates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CachedRecommendations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fc76f41f-1ca0-4baf-a4a7-d6beb17b09b3", "AQAAAAIAAYagAAAAEOPGTMeQ3hnrCXl588jrNI2/eTBbbIZWEkOUBCdobXeRdewOSrdnXYma5ahqz9NlJA==", "d6733b1d-7fc4-4517-b987-40afbbf0cf47" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfdbea47-a753-48e9-9051-2562fc67fa3f", "AQAAAAIAAYagAAAAEHRoy5IUe0og3u5IaMKN8oZYxg0MXjn/rYKBX8DhTIJvdRPwtKnaNtK+ccWTWbQVWw==", "da87a51b-3183-4e5b-9d76-62dc2787f5b6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b61c611-b99b-4ab4-ac78-8f72ebe89e11", "AQAAAAIAAYagAAAAEJvYSVw41644VM6MyMvqJMtAZuSJ+6Rpphg8nJr9ApVBVhYDDq0N9RBsgtq9FO6yUA==", "5da27e81-d24e-4bc8-a5fb-9bda5b65eaa2" });
        }
    }
}
