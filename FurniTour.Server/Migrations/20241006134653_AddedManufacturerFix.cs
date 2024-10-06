using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedManufacturerFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_Manufacturer_ManufacturerId",
                table: "Furnitures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manufacturer",
                table: "Manufacturer");

            migrationBuilder.RenameTable(
                name: "Manufacturer",
                newName: "Manufacturers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b7a9a41-f866-4229-ac89-ac5bd44500aa", "AQAAAAIAAYagAAAAEJmYv4XMfTSRWTlHtLlwwoRQjI5tc8iYxv0kjukxEp1p3y+hBaS8FY3bfaRyOpW3sQ==", "bf839b76-2211-4791-a6ad-97532dca35d7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99eda655-6f93-4a89-99bb-7e41cdbd2159", "AQAAAAIAAYagAAAAEG+H5n9Vw+guxTfTyrzWUKUJIlsUyytzuP62EWjkoWSMSoa+RHN24DaoPUiZRLbzgg==", "a0e58c02-ac1d-4bdd-9192-4c8c6b902efe" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3264df0-be26-44f1-9417-c8a0aa8aff21", "AQAAAAIAAYagAAAAEP8brJ5I/Vj7NWdUyMBRux8HoefbTxv0CVFLZGpDB+vU+d/ZAVBYvJKN8QWkSOtjEA==", "16ac8afd-945c-4e0a-a0e9-05d5081dd0f1" });

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_Manufacturers_ManufacturerId",
                table: "Furnitures",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_Manufacturers_ManufacturerId",
                table: "Furnitures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers");

            migrationBuilder.RenameTable(
                name: "Manufacturers",
                newName: "Manufacturer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manufacturer",
                table: "Manufacturer",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "18e2321c-2830-48e4-81f5-58ed7c1f015c", "AQAAAAIAAYagAAAAEATxzhp4TRxszNF1StnIFR5Cz0BVTXS47nXB88sFCIYG9wtNO909ShUoOi67NhkKPQ==", "0a91833b-617d-4f06-b3be-a45e846679a5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "31102271-178e-4abd-a14c-699138877b18", "AQAAAAIAAYagAAAAEKbgfXxpRupfu62VKMSerLpDFlHoGVjHMhqz292v/pTyh3nwgg0//lDIyGifw+mQug==", "276baabe-e139-4c2e-9401-87e653e15cbd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fd2b445-7ff6-4819-8577-46f0e6303607", "AQAAAAIAAYagAAAAEF1UyxABGHSjJQN4Vzulk+ZFo1h97kFWd1bc1pmOPZByhXj3/N1WxxCkCOpYuYsM5A==", "6a9cb0a0-e3e9-4f65-bf52-e3c3f90ab338" });

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_Manufacturer_ManufacturerId",
                table: "Furnitures",
                column: "ManufacturerId",
                principalTable: "Manufacturer",
                principalColumn: "Id");
        }
    }
}
