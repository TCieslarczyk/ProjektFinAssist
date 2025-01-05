using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektFinAssist.Data.Migrations
{
    /// <inheritdoc />
    public partial class Next : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "OperationsModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationsModel_IdentityUserId",
                table: "OperationsModel",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsModel_AspNetUsers_IdentityUserId",
                table: "OperationsModel",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationsModel_AspNetUsers_IdentityUserId",
                table: "OperationsModel");

            migrationBuilder.DropIndex(
                name: "IX_OperationsModel_IdentityUserId",
                table: "OperationsModel");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "OperationsModel");
        }
    }
}
