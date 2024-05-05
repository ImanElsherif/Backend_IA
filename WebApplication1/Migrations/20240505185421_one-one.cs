using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class oneone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_User_Id",
                table: "Employers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekers_User_Id",
                table: "JobSeekers",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
