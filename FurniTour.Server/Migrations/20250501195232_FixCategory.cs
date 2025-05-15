using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

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

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Кухні");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93c9c23b-0201-4fde-bac3-00777715e2a8", "AQAAAAIAAYagAAAAEFDvOnPSuyjyVPJucJ5aRC1Jgoox61AiXOj4AL85afwbLiX3NBn+4fQBXja/IedUkA==", "4c5534ee-7827-408e-9c55-117eb1b958c6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "57e45a9b-aca0-4cd0-9d16-abb0279175d6", "AQAAAAIAAYagAAAAEMSL2gNPSly9yy5oVo4Yc5evKsbRSliEHs29XRdomX/m78vvWTStglSUEkoi3oxXtg==", "50b89553-b96c-481f-9bd0-6f867b3e2234" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a2c5df67-6d3c-4efa-85b4-a45008933107", "AQAAAAIAAYagAAAAEF8/k9l17uNfpBZEbABQK3m5ek1+LUpa2/cUR7TjT3SqD5zFQSSLtOHavFwOLj7Lkw==", "777e5c1b-487d-4b53-bf8a-14c3cc550fee" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Кухонні гарнітури");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 12, "Кухні" });
        }
    }
}
