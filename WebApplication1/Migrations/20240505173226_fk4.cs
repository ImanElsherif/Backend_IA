using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class fk4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekers_User_UserId",
                table: "JobSeekers");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekers_UserId",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JobSeekers");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "JobSeekers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_UserId",
                table: "JobSeekers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekers_User_UserId",
                table: "JobSeekers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
