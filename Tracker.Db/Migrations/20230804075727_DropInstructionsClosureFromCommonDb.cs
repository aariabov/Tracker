using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Db.Migrations
{
    public partial class DropInstructionsClosureFromCommonDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "instructions_closures");
            migrationBuilder.DropTable(name: "instructions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
