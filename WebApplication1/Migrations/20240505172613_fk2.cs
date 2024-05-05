using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class fk2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employers_User_UserId",
                table: "Employers");

            migrationBuilder.DropIndex(
                name: "IX_Employers_UserId",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employers");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employers_UserId",
                table: "Employers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_User_UserId",
                table: "Employers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
