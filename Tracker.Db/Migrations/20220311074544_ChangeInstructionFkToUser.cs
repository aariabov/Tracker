using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Web.Migrations
{
    public partial class ChangeInstructionFkToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_OrgStruct_CreatorId",
                table: "Instructions");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_OrgStruct_ExecutorId",
                table: "Instructions");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutorId",
                table: "Instructions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Instructions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructions_AspNetUsers_CreatorId",
                table: "Instructions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructions_AspNetUsers_ExecutorId",
                table: "Instructions",
                column: "ExecutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_AspNetUsers_CreatorId",
                table: "Instructions");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructions_AspNetUsers_ExecutorId",
                table: "Instructions");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutorId",
                table: "Instructions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CreatorId",
                table: "Instructions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

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
    }
}
