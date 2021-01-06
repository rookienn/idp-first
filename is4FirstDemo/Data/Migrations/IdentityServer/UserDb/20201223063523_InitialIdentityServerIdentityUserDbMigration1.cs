using Microsoft.EntityFrameworkCore.Migrations;

namespace is4FirstDemo.Data.Migrations.IdentityServer.UserDb
{
    public partial class InitialIdentityServerIdentityUserDbMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserClaims",
                table: "IdentityUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserClaims",
                table: "IdentityUser",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
