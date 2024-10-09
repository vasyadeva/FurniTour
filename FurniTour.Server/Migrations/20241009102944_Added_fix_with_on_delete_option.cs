using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class Added_fix_with_on_delete_option : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManufacturerReviews_AspNetUsers_UserId",
                table: "ManufacturerReviews");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9809b4c8-9107-468d-8379-c6400c0d8cb3", "AQAAAAIAAYagAAAAEB8li4g79bGCP/fhDq/iJcODLYJWyD8XlyExiub6BwO8u2+d0oOChH9KfE0JBV/nuw==", "85e9002a-129a-4dd1-811d-97c7361660e5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "876558d7-ddfb-4a28-abe3-ec684c4b36cb", "AQAAAAIAAYagAAAAEKjUElx0CNO2XiPgZs9AcF6KAVx1IJ9cu367uNaoHNleG4ZeU0kpwPmBaipYDXtanQ==", "83ca6c07-fc28-4919-b590-2610f221f67b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b27be7a3-faab-417e-8e7b-cef9b12fb9d6", "AQAAAAIAAYagAAAAEOeJlRPS0KjhRerNU13RNW1QB1lkvKIDmOZlnwexXuVWAwmtVGETtM38dTYLzkA1tw==", "eabc9f43-856c-40de-9ec3-2e36f5e6fd2e" });

            migrationBuilder.AddForeignKey(
                name: "FK_ManufacturerReviews_AspNetUsers_UserId",
                table: "ManufacturerReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManufacturerReviews_AspNetUsers_UserId",
                table: "ManufacturerReviews");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ManufacturerReviews_AspNetUsers_UserId",
                table: "ManufacturerReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
