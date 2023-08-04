using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Db.Migrations
{
    public partial class RemoveFkInstructionsFromCommonDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_instructions_closures_instructions_instruction_id",
                table: "instructions_closures");

            migrationBuilder.DropForeignKey(
                name: "fk_instructions_closures_instructions_instruction_id1",
                table: "instructions_closures");

            migrationBuilder.DropIndex(
                name: "ix_instructions_closures_id",
                table: "instructions_closures");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_instructions_closures_id",
                table: "instructions_closures",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_instructions_closures_instructions_instruction_id",
                table: "instructions_closures",
                column: "id",
                principalTable: "instructions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_instructions_closures_instructions_instruction_id1",
                table: "instructions_closures",
                column: "parent_id",
                principalTable: "instructions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
