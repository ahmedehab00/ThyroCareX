using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThyroCareX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralAiSessionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralAiSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralAiSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralAiSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralAiChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralAiChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralAiChatMessages_GeneralAiSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "GeneralAiSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralAiChatMessages_SessionId",
                table: "GeneralAiChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralAiSessions_UserId",
                table: "GeneralAiSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralAiChatMessages");

            migrationBuilder.DropTable(
                name: "GeneralAiSessions");
        }
    }
}
