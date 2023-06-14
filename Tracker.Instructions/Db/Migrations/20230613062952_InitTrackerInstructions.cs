using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tracker.Instructions.Db.Migrations
{
    public partial class InitTrackerInstructions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    boss_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_users_boss_id",
                        column: x => x.boss_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "instructions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    tree_path = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<string>(type: "text", nullable: false),
                    executor_id = table.Column<string>(type: "text", nullable: false),
                    deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    exec_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructions", x => x.id);
                    table.ForeignKey(
                        name: "fk_instructions_instructions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "instructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_instructions_users_creator_id",
                        column: x => x.creator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_instructions_users_executor_id",
                        column: x => x.executor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "ix_instructions_creator_id",
                table: "instructions",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_instructions_executor_id",
                table: "instructions",
                column: "executor_id");

            migrationBuilder.CreateIndex(
                name: "ix_instructions_parent_id",
                table: "instructions",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_instructions_closures_id",
                table: "instructions_closures",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_users_boss_id",
                table: "users",
                column: "boss_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instructions_closures");

            migrationBuilder.DropTable(
                name: "instructions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
