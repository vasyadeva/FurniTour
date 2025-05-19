using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    IndividualOrderId = table.Column<int>(type: "int", nullable: true),
                    GuaranteeId = table.Column<int>(type: "int", nullable: true),
                    RedirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

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
    }
}
