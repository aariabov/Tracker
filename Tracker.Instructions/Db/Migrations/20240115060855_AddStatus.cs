using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tracker.Instructions.Db.Migrations
{
    public partial class AddStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_status", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "status",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "В работе" },
                    { 2, "В работе просрочено" },
                    { 3, "Выполнено в срок" },
                    { 4, "Выполнено с нарушением срока" }
                });

            migrationBuilder.AddColumn<int>(
                name: "status_id",
                table: "instructions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_instructions_status_id",
                table: "instructions",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_instructions_status_status_id",
                table: "instructions");

            migrationBuilder.DropTable(
                name: "status");

            migrationBuilder.DropIndex(
                name: "ix_instructions_status_id",
                table: "instructions");

            migrationBuilder.DropColumn(
                name: "status_id",
                table: "instructions");
        }
    }
}
