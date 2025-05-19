using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniTour.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedChatEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    User2Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_User1Id",
                        column: x => x.User1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_User2Id",
                        column: x => x.User2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReceiverId",
                table: "ChatMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User1Id",
                table: "Conversations",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User2Id",
                table: "Conversations",
                column: "User2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a087",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b50e9e71-8ebd-42d7-81e9-561a4635102c", "AQAAAAIAAYagAAAAEIHAxq4Nyo0bq06ZeiS/9sr9RSMBE56YU/DkhEKbvvXcUYcuqfpoPY6iDhjZs/gskA==", "a55f5b94-7c2e-41ee-82d9-23f801ead16f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "37bb0930-ee5b-483a-88a9-9fc2dab9a903",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d8a7fb36-61d9-4639-98fc-f64da91aee2c", "AQAAAAIAAYagAAAAEFY7yV5zLH5NDRJKdVMCbcRvIOpReWAM87LfnHnYbViO7oXvKKf5FStO/TjpD6Me2g==", "3cede150-4188-41da-b8a5-7150b4c8df99" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ccf9bfb8-47d6-41bf-9c5d-502",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10af60f5-b9a2-4424-bc3b-551bb8ff8e2d", "AQAAAAIAAYagAAAAEJJRhwe94tT/udS5IJQb01FqZa+MKp7WlQE9VLsZaj/wLrojM7PsQP4sNCq19dHSPw==", "4433cef3-97f0-4bc1-8f95-6812886df250" });
        }
    }
}
