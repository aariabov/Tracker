using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Db.Migrations
{
    public partial class AddInstructionsClosures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "instructions_closures",
                columns: table => new
                {
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false),
                    depth = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructions_closures", x => new { x.parent_id, x.id });
                    table.ForeignKey(
                        name: "fk_instructions_closures_instructions_instruction_id",
                        column: x => x.id,
                        principalTable: "instructions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_instructions_closures_instructions_instruction_id1",
                        column: x => x.parent_id,
                        principalTable: "instructions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_instructions_closures_id",
                table: "instructions_closures",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instructions_closures");
        }
    }
}
