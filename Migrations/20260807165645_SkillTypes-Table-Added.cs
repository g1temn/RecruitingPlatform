using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class SkillTypesTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillTypeId",
                table: "skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "skill_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill_types", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_skills_SkillTypeId",
                table: "skills",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_skill_types_Name",
                table: "skill_types",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_skill_types_SkillTypeId",
                table: "skills",
                column: "SkillTypeId",
                principalTable: "skill_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skills_skill_types_SkillTypeId",
                table: "skills");

            migrationBuilder.DropTable(
                name: "skill_types");

            migrationBuilder.DropIndex(
                name: "IX_skills_SkillTypeId",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "SkillTypeId",
                table: "skills");
        }
    }
}
