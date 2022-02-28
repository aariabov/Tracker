using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Web.Migrations
{
    public partial class AddFKToInstruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Instructions_CreatorId",
                table: "Instructions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructions_ExecutorId",
                table: "Instructions",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructions_OrgStruct_CreatorId",
                table: "Instructions",
                column: "CreatorId",
                principalTable: "OrgStruct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructions_OrgStruct_ExecutorId",
                table: "Instructions",
                column: "ExecutorId",
                principalTable: "OrgStruct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_OrgStruct_CreatorId",
                table: "Instructions");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_OrgStruct_ExecutorId",
                table: "Instructions");

            migrationBuilder.DropIndex(
                name: "IX_Instructions_CreatorId",
                table: "Instructions");

            migrationBuilder.DropIndex(
                name: "IX_Instructions_ExecutorId",
                table: "Instructions");
        }
    }
}
