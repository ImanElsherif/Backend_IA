using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class propfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal");

            migrationBuilder.RenameTable(
                name: "Proposal",
                newName: "Proposals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_EmployerId",
                table: "Proposals",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_JobId",
                table: "Proposals",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_JobSeekerId",
                table: "Proposals",
                column: "JobSeekerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Employers_EmployerId",
                table: "Proposals",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_JobSeekers_JobSeekerId",
                table: "Proposals",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Jobs_JobId",
                table: "Proposals",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Employers_EmployerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_JobSeekers_JobSeekerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Jobs_JobId",
                table: "Proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_EmployerId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_JobId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_JobSeekerId",
                table: "Proposals");

            migrationBuilder.RenameTable(
                name: "Proposals",
                newName: "Proposal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal",
                column: "ProposalId");
        }
    }
}
