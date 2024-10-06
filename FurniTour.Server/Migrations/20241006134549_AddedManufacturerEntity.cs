using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedManufacturerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_AspNetUsers_MasterId",
                table: "Furnitures");

            migrationBuilder.AlterColumn<string>(
                name: "MasterId",
                table: "Furnitures",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Furnitures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_ManufacturerId",
                table: "Furnitures",
                column: "ManufacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_AspNetUsers_MasterId",
                table: "Furnitures",
                column: "MasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_Manufacturer_ManufacturerId",
                table: "Furnitures",
                column: "ManufacturerId",
                principalTable: "Manufacturer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_AspNetUsers_MasterId",
                table: "Furnitures");

            migrationBuilder.DropForeignKey(
                name: "FK_Furnitures_Manufacturer_ManufacturerId",
                table: "Furnitures");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropIndex(
                name: "IX_Furnitures_ManufacturerId",
                table: "Furnitures");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Furnitures");

            migrationBuilder.AlterColumn<string>(
                name: "MasterId",
                table: "Furnitures",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b88072ca-29c1-4530-88bf-187d7d48bfc6", "AQAAAAIAAYagAAAAECfG3myaDzNroiWo/S25hiFgr91Ms8FQctC+5MGn/vw+VESwtOm9AZ62ChzQQL/OLA==", "0c8761cc-50dd-4017-a56b-287f1989f115" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d0890e3c-42f6-4396-bfb5-f4a962c56e02", "AQAAAAIAAYagAAAAEOFu2wGTIx6wNxo2JMXHVoC9/e/zxc45woSenymfsBCK9egyYLQwH+NIUMnLhqVWDw==", "00a6ae5b-f653-4414-9f9c-f880f2804b95" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a1957f18-0c1c-4c68-b053-e9ed972f849f", "AQAAAAIAAYagAAAAEAa7g9INVcVG+xacEJaqEcwH2KfIxw1UHgCPWGeE/gc+nMpvygTuMtpo+3uf/JuIRw==", "a71fc32f-dc89-409d-b8fe-bad19dee01dc" });

            migrationBuilder.AddForeignKey(
                name: "FK_Furnitures_AspNetUsers_MasterId",
                table: "Furnitures",
                column: "MasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
