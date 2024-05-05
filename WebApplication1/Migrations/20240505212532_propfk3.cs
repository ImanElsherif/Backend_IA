using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class propfk3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Employers_EmployerId",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "EmployerId",
                table: "Proposals",
                newName: "EmpId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_EmployerId",
                table: "Proposals",
                newName: "IX_Proposals_EmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Employers_EmpId",
                table: "Proposals",
                column: "EmpId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Employers_EmpId",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "EmpId",
                table: "Proposals",
                newName: "EmployerId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_EmpId",
                table: "Proposals",
                newName: "IX_Proposals_EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Employers_EmployerId",
                table: "Proposals",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
