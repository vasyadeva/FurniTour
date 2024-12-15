using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedGuaranteePhotoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuaranteePhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuaranteeId = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuaranteePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuaranteePhotos_Guarantees_GuaranteeId",
                        column: x => x.GuaranteeId,
                        principalTable: "Guarantees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ec15c3e8-c3af-4957-8d6f-bf9df4a7d715", "AQAAAAIAAYagAAAAEFqNHXxscnJZUHB739OgCkYdT4Tq3YS3MsEuOt82exQ4rE9Omq3FlXpB2ycrslItcQ==", "5e399f7b-7211-4d0d-9bca-7bd39de92f9a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dcbbf191-9d74-4abb-a92f-5f988c8f469d", "AQAAAAIAAYagAAAAEJEBeqErWool9SK0gonPr4Dga2AxlVHMKwuY4mKM5NiSPip8ZPJELTmAiW7hYTQ8xw==", "28768899-0e1c-403f-9f73-ade7733bd063" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d6f14b3-c685-420b-82fb-ded13bfd5f54", "AQAAAAIAAYagAAAAEPfOC5TvNgDQr76i/VgViH6dpQ7y12L18UMx2kBUGjW9GZb3abwSuKnvqRGbiBuUHw==", "d7b25e4e-15dc-4ead-9fa5-51e67d27a1f0" });

            migrationBuilder.CreateIndex(
                name: "IX_GuaranteePhotos_GuaranteeId",
                table: "GuaranteePhotos",
                column: "GuaranteeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuaranteePhotos");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "299c9234-c32e-46ca-b0a4-0c14723b540d", "AQAAAAIAAYagAAAAEBZC0B4xivsuFyq2NOKKXyVm0wZNMxrsXCHkRuO8qRcX0HAT56J9iHi37e/XiHxzLw==", "66f46c7f-c783-4784-bc03-b0042a4dee2b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe938b9b-f613-47f8-a39e-2626c7901f88", "AQAAAAIAAYagAAAAECvmoGQROTZpya/VUcd1lzyHjpdMBOM36ahdEjAjUJaGxnPbzJJ6EAjlIe36RghuCw==", "e0a9e96a-778b-419f-ab72-d8ac50d49d27" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "04aba3d2-f591-4400-b0c4-b046a064982f", "AQAAAAIAAYagAAAAEMO7VkvIK+J5qEN0GLY+v8KMDlBXIr8XTl3V9qZTMxrghDjAkveqvd9RdR9ISyNlAw==", "4b482c3b-21b8-48eb-9c90-315a866aabe8" });
        }
    }
}
