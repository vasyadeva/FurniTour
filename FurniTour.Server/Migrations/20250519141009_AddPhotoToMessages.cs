using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoContentType",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "ChatMessages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b2f3ad6-c8f3-4e60-8908-055987f83cc3", "AQAAAAIAAYagAAAAEEtns6rT66bUxY0r1KFbJ9LgLiY+MTMv9U7Q0C/OpdVcdSY3/OnfFvbLoiwBLj6zpg==", "4d75e706-36d2-43e1-a835-8ee2c8e6213b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ddcce115-b948-4f31-8d99-be6fd36ceddf", "AQAAAAIAAYagAAAAEMprPP8/ABEpVs/BCt4PmlF935S8WJXUSeHbBmGKFNaSQu/v3Dofuo0Gl8859aNYyw==", "a8b7a253-8542-4039-a14c-3a52ec6e58d2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "59e55a34-5d93-41e1-9f4b-e7dd83a33a5b", "AQAAAAIAAYagAAAAEJQx+zhNjBfzve9WeKhgHBgtDGtiwchyNjii2utfAZQ6sGFzns1AQcAfXYodK66RwQ==", "f6af1c83-2c39-40ac-94c3-133db9c4e1ee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoContentType",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "ChatMessages");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0f63fc4a-26f8-4e1b-bfa7-4c0d743b79c8", "AQAAAAIAAYagAAAAECzeyFgvuSjlVe/su9ZFC3TmiVxj0LYbI4Bmmy2S97wtLvBa+QMQBDIgfqXKyxMa+A==", "a47fdbcf-bc7b-48cd-ac9b-9073531249c7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a92a5273-7daa-4c81-b490-60377657d643", "AQAAAAIAAYagAAAAEAWkJYYRDjva0TIXlrpUltAuHhi4nOZQ+Mr9HKJUAI0KUMG9DWKIwe82hC9k2A/cnA==", "14a70544-9238-4ad7-988a-813a7be67fa3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "113b9a2a-1531-48ee-9914-676c720db1c1", "AQAAAAIAAYagAAAAEEoyY7Fk/hwqyvJKeQdbBdXSKAatjFAeW69LHMfHt9PpD4z42Zdnl/puCMQ14mn9Kg==", "c1d52166-1929-4d98-a6b0-9ba80de76d03" });
        }
    }
}
