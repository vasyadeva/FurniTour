using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedColorAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Furnitures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Furnitures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Beds" },
                    { 2, "Chairs" },
                    { 3, "Tables" },
                    { 4, "Sofas" },
                    { 5, "Cupboards" },
                    { 6, "Shelves" },
                    { 7, "Dressers" },
                    { 8, "Wardrobes" }
                });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Black" },
                    { 2, "White" },
                    { 3, "Red" },
                    { 4, "Yellow" },
                    { 5, "Green" },
                    { 6, "Blue" },
                    { 7, "Purple" },
                    { 8, "Orange" },
                    { 9, "Brown" },
                    { 10, "Grey" },
                    { 11, "Pink" },
                    { 12, "Beige" },
                    { 13, "Cyan" },
                    { 14, "Magenta" },
                    { 15, "Lime" },
                    { 16, "Teal" },
                    { 17, "Maroon" },
                    { 18, "Navy" },
                    { 19, "Olive" },
                    { 20, "Silver" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_CategoryId",
                table: "Furnitures",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_ColorId",
                table: "Furnitures",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_Categories_CategoryId",
                table: "Furnitures",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_Colors_ColorId",
                table: "Furnitures",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_Categories_CategoryId",
                table: "Furnitures");

            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_Colors_ColorId",
                table: "Furnitures");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Furnitures_CategoryId",
                table: "Furnitures");

            migrationBuilder.DropIndex(
                name: "IX_Furnitures_ColorId",
                table: "Furnitures");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Furnitures");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Furnitures");

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
        }
    }
}
