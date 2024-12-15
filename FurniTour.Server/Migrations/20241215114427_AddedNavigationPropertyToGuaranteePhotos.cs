using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedNavigationPropertyToGuaranteePhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
