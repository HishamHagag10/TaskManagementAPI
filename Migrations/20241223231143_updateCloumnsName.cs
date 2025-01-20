using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateCloumnsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedUserEmail",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.RenameColumn(
                name: "CreatedUserEmail",
                table: "Projects",
                newName: "ProjectManagerEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CreatedUserEmail",
                table: "Projects",
                newName: "IX_Projects_ProjectManagerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ProjectManagerEmail",
                table: "Projects",
                column: "ProjectManagerEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ProjectManagerEmail",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectManagerEmail",
                table: "Projects",
                newName: "CreatedUserEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ProjectManagerEmail",
                table: "Projects",
                newName: "IX_Projects_CreatedUserEmail");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientUserEmail = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_RecipientUserEmail",
                        column: x => x.RecipientUserEmail,
                        principalTable: "AspNetUsers",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientUserEmail",
                table: "Notifications",
                column: "RecipientUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TaskId",
                table: "Notifications",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedUserEmail",
                table: "Projects",
                column: "CreatedUserEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
