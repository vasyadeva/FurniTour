using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedGuaranteeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guarantees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guarantees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guarantees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Guarantees_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuaranteeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuaranteeId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuaranteeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuaranteeItems_Guarantees_GuaranteeId",
                        column: x => x.GuaranteeId,
                        principalTable: "Guarantees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuaranteeItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_GuaranteeItems_GuaranteeId",
                table: "GuaranteeItems",
                column: "GuaranteeId");

            migrationBuilder.CreateIndex(
                name: "IX_GuaranteeItems_OrderItemId",
                table: "GuaranteeItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantees_OrderId",
                table: "Guarantees",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantees_UserId",
                table: "Guarantees",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuaranteeItems");

            migrationBuilder.DropTable(
                name: "Guarantees");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7bae8d17-ec55-4a39-a0a8-531509075112", "AQAAAAIAAYagAAAAELc2QonSKsLi1j088+ZlgXTIYpWv3jW53FlK1gZV1v6ZDS/XDpZSRRBNB4FbQc3pVw==", "140e740a-7c48-4ea6-899b-7c3f53a4e326" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f06c148-8cef-4803-9d1d-f3ae6fee20f2", "AQAAAAIAAYagAAAAEJLOyg5oII0N/JsZCtNv+nIIhJMaZDzfm4+7TkHNkLoEIGOvNL0epv3qKWMBodz+GQ==", "9fe01991-9692-460b-8e17-23b856f03d98" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc5bf0c0-8d1b-402f-bd09-8d6eb80ed083", "AQAAAAIAAYagAAAAEMMV2WvTr+UNm7WEGta4AXZLpHbBsZN85KHvowymNTt3WvL7RDgFHykBeAmn+F+oFw==", "fdb45e1b-0d26-46ae-9abb-70a86b5d77c7" });
        }
    }
}
