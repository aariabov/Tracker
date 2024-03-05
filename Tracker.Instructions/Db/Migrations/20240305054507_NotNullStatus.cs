using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Instructions.Db.Migrations
{
    public partial class NotNullStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "instructions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "instructions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id");
        }
    }
}
